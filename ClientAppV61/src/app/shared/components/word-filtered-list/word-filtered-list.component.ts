import { Component, OnInit, ViewChild, EventEmitter, OnDestroy, AfterViewInit, Output, Input } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { IWord } from '../../interfaces/IWord';
import { Subject, merge, of } from 'rxjs';
import { IUser, AuthorizeService } from 'src/api-authorization/authorize.service';
import { IWordUser } from '../../interfaces/IWordUser';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { WordService } from '../../services/word.service';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { GlobalService } from '../../services/global.service';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { AppSettingsService } from '../../services/app-settings.service';
import { IWordSelected } from '../../interfaces/IWordSelected';
import { takeUntil, concatMap, startWith, switchMap, map, catchError } from 'rxjs/operators';
import { ISearchListArg } from '../../interfaces/IListArg';
import { IRatingControlValue } from '../rating/rating.component';
import { UpdateWordTranslationDialogComponent } from '../update-word-translation-dialog/update-word-translation-dialog.component';
import { IWordTranslation } from '../../interfaces/IWordTranslation';
import { UpdateWordDialogComponent } from '../update-word-dialog/update-word-dialog.component';
import { SelectWordPhraseDialogComponent } from '../select-word-phrase-dialog/select-word-phrase-dialog.component';
import { IWordPhrase } from '../../interfaces/IWordPhrase';
import { environment } from 'src/environments/environment';
import { UpdateTemplateJsonDialogComponent } from '../update-template-json-dialog/update-template-json-dialog.component';

@Component({
  selector: 'app-word-filtered-list',
  templateUrl: './word-filtered-list.component.html',
  styleUrls: ['./word-filtered-list.component.scss']
})
export class WordFilteredListComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() selectedItems: Array<IWordSelected>;
  @Input() wordIDColumnVisibile: boolean;
  @Input() translateWordButtonVisibile: boolean;
  @Input() selectWordButtonVisibile: boolean;
  @Input() managePhrasiesButtonVisibile: boolean;
  @Input() editWordButtonVisibile: boolean;
  @Input() deleteWordButtonVisibile: boolean;
  @Input() addWordButtonVisibile: boolean;
  @Input() audioColumnVisibile = true;

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  @Output() public selectRow = new EventEmitter();
  @Output() public selectRows = new EventEmitter();

  public searchForm = this.fb.group({
    searchText: [''],
    rateFrom: [null],
    rateTo: [null],
    gradeFrom: [null],
    gradeTo: [null],
  });

  public displayedColumns: string[] = ['rate', 'wordID', 'text', 'grade', 'wordPhraseCountSelected', 'act', 'audio'];
  public data: IWord[] = [];
  public resultsLength = 0;
  public isLoading = true;
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;

  private refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();
  private wordUserUpdate$ = new Subject<IWordUser>();

  public formGr: FormGroup;

  constructor(
    private wordService: WordService,
    private toasterService: ToastrService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private appSettingsService: AppSettingsService,
  ) {
  }

  ngOnInit() {
    if (!this.wordIDColumnVisibile) {
      this.displayedColumns = this.displayedColumns.filter(x => x !== 'wordID');
    }

    if (!this.audioColumnVisibile) {
      this.displayedColumns = this.displayedColumns.filter(x => x !== 'audio');
    }

    this.wordUserUpdate$
    .pipe(
      takeUntil(this.ngUnsubscribe$),
      concatMap((wordUser: IWordUser) => {
        console.log('update grade', wordUser);
        return this.wordService.updateWordUser(wordUser);
      })
    ).subscribe(
      (result: IWordUser) => {
        const word = this.data.find(x => x.wordID === result.wordID);
        word.wordUser = result;
        this.toasterService
        .success(`Grade of "${word ? word.text : result.wordID}" updated to "${result.grade}"`, 'Grade Updated', { closeButton: true });
      },
      error => {
        console.log('Save error:', error);
        this.toasterService.error('Word grade update:', 'Error');
      },
    );
  }

  ngAfterViewInit() {
    this.sort.sortChange
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page, this.refreshPage, this.appSettingsService.currentAppSetting)
      .pipe(takeUntil(this.ngUnsubscribe$))
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoading = true;
          return this.wordService.getFiltered(
            {
              sort: this.sort.active === undefined ? '' : this.sort.active,
              order: this.sort.direction,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
              searchText: this.searchForm.get('searchText').value || '',
              rateFrom: this.searchForm.get('rateFrom').value || 0,
              rateTo: this.searchForm.get('rateTo').value || 0,
              gradeFrom: this.searchForm.get('gradeFrom').value || 0,
              gradeTo: this.searchForm.get('gradeTo').value || 0,
              recomended: false,
            } as ISearchListArg
          );
        }),
        map(result => {
          this.isLoading = false;
          this.resultsLength = result.count;
          return result.data;
        }),
        catchError(() => {
          this.isLoading = false;
          return of([]);
        })
      ).subscribe(data => {
        this.data = data;
        if (this.selectedItems) {
          this.selectedItems.forEach((x) => {
            const word = this.data.find(y => y.wordID === x.word.wordID);
            if (word) {
              word.selected = true;
            }
          });
        }

        this.data.forEach((word) => {
          let isReady = word.wordPhraseSelected && word.wordPhraseSelected.length > 2;
          if (isReady) {
            let isReadyCnt = 0;
            word.wordPhraseSelected.forEach(x => {
              if(!(!x.textSpeechFileName || !x.wordPhraseTranslation || !x.wordPhraseTranslation.textSpeechFileName)){
                isReadyCnt++;
              }              
            });
            isReady = isReadyCnt > 2;
          }

          word.wordPhraseSelectedIsRedyForLesson = isReady;
        });
      });
  }

  update(word: IWord) {
    const dialogRef = this.dialog.open(
      UpdateWordDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: { ...word },
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IWord) => {
        if (dialogResult && dialogResult !== null) {
          this.toasterService.success('Word updated', 'successful');
          this.refreshPage.emit(1);
        }
      });
  }

  addNew() {
    this.update({ wordID: 0, rate: 0, text: '' } as IWord);
  }

  delete(word: IWord) {
    if (window.confirm('Are sure you want to delete this item ?')) {
     this.isLoading = true;
     this.wordService.Delete(word)
     .toPromise()
     .then((result) => {
          this.toasterService.success('Word delete', 'deleted successful');
          this.refreshPage.emit(1);
        }).finally(() => {
          this.isLoading = false;
        });
    }
  }

  makeSpeeches(word: IWord) {
    this.wordService.makeSpeeches(word)
    .toPromise()
    .then((result) => {
         this.toasterService.success('Speeches updated', 'successful');
         this.refreshPage.emit(1);
       }).finally(() => {
         this.isLoading = false;
       });
 }

  managePhrases(word: IWord) {
    const wordSelected = {
      word,
      phraseWords: [],
      phraseWordsSelected: []
    } as IWordSelected;

    this.openManageWordPhraseDialog(wordSelected);
  }

  openManageWordPhraseDialog(wordSelected: IWordSelected) {
    const dialogRef = this.dialog.open(
      SelectWordPhraseDialogComponent,
      {
        disableClose: true,
        width: '800px',
        data:
        {
          selectionVisibile: false,
          wordSelected
        }
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: Array<IWordPhrase>) => {
        // console.log('dialogResult:', dialogResult);

        // if (dialogResult === null) { return; }

        // if (!wordSelected.phraseWordsSelected) {
        //   wordSelected.phraseWordsSelected = Array<IWordPhrase>();
        // }

        // dialogResult.forEach((x) => {
        //   if (!wordSelected.phraseWordsSelected.some(e => e.wordPhraseID === x.wordPhraseID)) {
        //     wordSelected.phraseWordsSelected.push(x);
        //   }
        // });

        // console.log('wordSelected.phraseWordsSelected:', wordSelected.phraseWordsSelected);
      }).finally(() => {

      });
  }

  editWordTranslation(word: IWord) {
    const dialogRef = this.dialog.open(
      UpdateWordTranslationDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data:
        {
          itemData: {
            ...word,
            wordTranslation: word.wordTranslation ?
             {...word.wordTranslation} :
             {
               wordTranslationID: 0,
                wordID: word.wordID,
                languageID: this.appSettingsService.getCurrent().NativeLanguage.languageID,
                text: ''
              } as IWordTranslation
          }
        }
      }
    );

    dialogRef.afterClosed().toPromise()
    .then((dialogResult: IWord) => {
      if (dialogResult == null) { return; }
      this.setLoader(true);
      this.wordService.updateWordTranslation(dialogResult.wordTranslation)
      .toPromise()
      .then((result: any) => {
        word.wordTranslation = dialogResult.wordTranslation;
      });
    }).finally(() => {
      this.setLoader(false);
    });
  }

  setLoader(value) {
    this.isLoading = value;
  }

  onSubmitSearch() {
    this.paginator.pageIndex = 0;
    this.refreshPage.emit();
  }

  addAll() {
    this.selectRows.emit(this.data);
  }

  changeWordGrade(event: IRatingControlValue) {
    const item = {
      wordID: event.itemId,
      languageID: event.item.languageID,
      grade: event.value,
    } as IWordUser;

    console.log('rating', event);
    this.wordUserUpdate$.next(item);
  }

  addNewFromList()
  {
    const jsonStr = '';
    const dialogRef = this.dialog.open(
      UpdateTemplateJsonDialogComponent,
      {
        disableClose: true,
        width: '800px',
        data: jsonStr,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: ISearchListArg) => {
        if (dialogResult) {
          this.setLoader(true);
          this.wordService.addListFromTextWithPhrases(dialogResult)
          .toPromise()
          .then((result: any) => {
            this.onSubmitSearch();
          });
        }
      });
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
