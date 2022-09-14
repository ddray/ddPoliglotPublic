import { Component, OnInit, Inject, EventEmitter, AfterViewInit, OnDestroy, ViewChild, Input } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { IWordPhrase } from '../../interfaces/IWordPhrase';
import { IWordUsed } from '../../interfaces/IWordUsed';
import { WordService } from '../../services/word.service';
import { IListResult } from '../../interfaces/IListResult';
import { merge, Subject, of } from 'rxjs';
import { takeUntil, startWith, switchMap, map, catchError } from 'rxjs/operators';
import { ISearchListArg } from '../../interfaces/IListArg';
import { WordphraseDialogComponent } from '../wordphrase-dialog/wordphrase-dialog.component';
import { IWord } from '../../interfaces/IWord';
import { IWordPhraseTranslation } from '../../interfaces/IWordPhraseTranslation';
import { AppSettingsService } from '../../services/app-settings.service';
import { MatSort } from '@angular/material/sort';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-select-word-phrase-dialog',
  templateUrl: './select-word-phrase-dialog.component.html',
  styleUrls: ['./select-word-phrase-dialog.component.scss']
})
export class SelectWordPhraseDialogComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  public itemData: Array<IWordPhrase>;
  public wordPhraseSelected: Array<IWordPhrase>;
  public resultsLength = 0;
  public displayedColumnsSel: string[] = ['phraseOrderInCurrentWord', 'text', 'translation', 'act'];
  public displayedColumns: string[] = ['phraseOrderInCurrentWord', 'text', 'translation', 'act', 'audio'];
  private refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();
  public isLoading = true;
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<SelectWordPhraseDialogComponent>,
    private wordService: WordService,
    private dialog: MatDialog,
    private appSettingsService: AppSettingsService,
  ) { }

  ngOnInit() {
    this.wordPhraseSelected = new Array<IWordPhrase>();
  }

  ngAfterViewInit() {
    // this.paginator.pageIndex = 0;
    // this.refreshPage.emit();

    this.sort.sortChange
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page, this.refreshPage)
      .pipe(takeUntil(this.ngUnsubscribe$))
      .pipe(
        startWith({}),
        switchMap(() => {
          setTimeout(() => {
            this.isLoading = true;
          }, 0);
          return this.wordService.getWordPhrases(
            {
              parentID: this.data.wordSelected.word.wordID,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
              sort: this.sort.active === undefined ? '' : this.sort.active,
              order: this.sort.direction,
            } as ISearchListArg
          );
        }),
        map((result: IListResult) => {
          this.resultsLength = result.count;
          return result.data;
        }),
        catchError((e) => {
          setTimeout(() => { this.isLoading = false; }, 0);
          return of([]);
        })
      ).subscribe(data => {
        this.itemData = data;

        this.itemData.forEach((wordPhrase: IWordPhrase) => {
          // prepare for show
          this.prepareWordPhrase(wordPhrase);

          // restore selected
          if (this.wordPhraseSelected.find(x => x.wordPhraseID === wordPhrase.wordPhraseID)) {
            wordPhrase.selected = true;
          }
        });

        setTimeout(() => { this.isLoading = false; }, 0);
     });
  }

  prepareWordPhrase(wordPhrase: IWordPhrase) {
    wordPhrase.wordsUsedList = this.splitUsedPhrases(wordPhrase.wordsUsed);
    const ar = wordPhrase.text.split(/( )/g);
    wordPhrase.wordsUsedList.forEach((wordUsed: IWordUsed) => {
      if (wordUsed.text.length > 2) {
        const className = this.data.wordSelected.word.rate > wordUsed.rate
          ? 'past'
          : this.data.wordSelected.word.rate < wordUsed.rate
            ? 'feature'
            : 'present';

        ar.forEach((s, index) => {
          if (
            s
            && s.length > 2
            && Math.abs(s.length - wordUsed.text.length) < 4
            && s.search('<span') === -1
          ) {
            ar[index] = ar[index].replace(wordUsed.text, `<span class="${className}">${wordUsed.text}</span>`);
          }
        });
      }
    });

    wordPhrase.textHtml = '';
    ar.forEach((s) => {
      wordPhrase.textHtml = wordPhrase.textHtml + s;
    });
  }

  splitUsedPhrases(wordsUsed: string): IWordUsed[] {
    const result = new Array<IWordUsed>();
    if (!wordsUsed) {
      return result;
    }

    const arr = wordsUsed.split(',');
    arr.forEach(x => {
      if (x) {
        const ar2 = x.split(';');
        if (ar2.length > 1) {
          result.push(
            {
              text: ar2[0],
              rate: parseInt(ar2[1], 10)
            } as IWordUsed
          );
        }
      }
    });

    return result;
  }

  editPhrase(wordPhrase: IWordPhrase) {
    this.openWordPhraseDialog(wordPhrase);
  }

  openWordPhraseDialog(wordPhrase: IWordPhrase) {

    const self = this;
    const dialogRef = this.dialog.open(
      WordphraseDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data:
        {
          wordPhrase
        }
      }
    );


    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IWordPhrase) => {
        if (dialogResult === null) { return; }

        this.isLoading = true;

        // save or update phrase
        this.wordService.SaveWordPhrase(dialogResult)
          .toPromise()
          .then((result: IWordPhrase) => {
            const oldWordPhrase = this.itemData.find(x => x.wordPhraseID === result.wordPhraseID);
            if (oldWordPhrase) {
              oldWordPhrase.text = result.text;
              oldWordPhrase.wordPhraseTranslation = result.wordPhraseTranslation;
              oldWordPhrase.phraseOrderInCurrentWord = result.phraseOrderInCurrentWord;
              oldWordPhrase.textSpeechFileName = result.textSpeechFileName;
              this.prepareWordPhrase(oldWordPhrase);
            } else {
              this.prepareWordPhrase(result);
              this.itemData = this.itemData.splice(0, 0, result);
              this.paginator._changePageSize(this.paginator.pageSize);
            }
          }).finally(() => {
            this.isLoading = false;
          });
      }).finally(() => {
        // setTimeout(() => {
        //   self.isLoading = false;
        // }, 0);
      });
  }

  addNewPhrase() {
    const wordPhrase = {
      wordPhraseID: 0,
      languageID: this.appSettingsService.getCurrent().LearnLanguage.languageID,
      text: '',
      hashCode: 0,
      selected: false,
      wordsUsed: `${(this.data.wordSelected.word as IWord).text};${(this.data.wordSelected.word as IWord).wordID}`,
      wordPhraseTranslation: {
        wordPhraseTranslationID: 0,
        wordPhraseID: 0,
        languageID: this.appSettingsService.getCurrent().NativeLanguage.languageID,
        text: ''
      } as IWordPhraseTranslation,
      wordsUsedList: {},
      textHtml: '',
      currentWordID: (this.data.wordSelected.word as IWord).wordID,
      phraseOrderInCurrentWord: 0,
    } as IWordPhrase;

    this.openWordPhraseDialog(wordPhrase);
  }

  selectPhrase(wordPhrase: IWordPhrase) {
    wordPhrase.selected = true;
    this.wordPhraseSelected.push(wordPhrase);
    this.wordPhraseSelected = this.wordPhraseSelected.filter(x => x.wordPhraseID !== -1);
  }

  deleteRowFromList(wordPhrase: IWordPhrase) {
    this.wordPhraseSelected = this.wordPhraseSelected.filter(x => x.wordPhraseID !== wordPhrase.wordPhraseID);
    wordPhrase.selected = false;
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    this.dialogRef.close(this.wordPhraseSelected);
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
