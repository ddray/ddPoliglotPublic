import { Component, OnInit, OnDestroy, AfterViewInit, Inject, EventEmitter, ViewChild } from '@angular/core';
import { IWordPhrase } from '../../interfaces/IWordPhrase';
import { Subject, merge, of } from 'rxjs';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { SelectWordPhraseDialogComponent } from '../select-word-phrase-dialog/select-word-phrase-dialog.component';
import { WordService } from '../../services/word.service';
import { AppSettingsService } from '../../services/app-settings.service';
import { ArticleByParamService } from '../../services/articleByParam.service';
import { ToastrService } from 'ngx-toastr';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { IArticleByParam } from '../../interfaces/IArticleByParam';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { takeUntil, startWith, switchMap, map, catchError } from 'rxjs/operators';
import { ISearchListArg } from '../../interfaces/IListArg';
import { ResolveEnd } from '@angular/router';
import { IWord } from '../../interfaces/IWord';
import { IWordPhraseTranslation } from '../../interfaces/IWordPhraseTranslation';

@Component({
  selector: 'app-select-repeat-words-phrases-dialog',
  templateUrl: './select-repeat-words-phrases-dialog.component.html',
  styleUrls: ['./select-repeat-words-phrases-dialog.component.css']
})
export class SelectRepeatWordsPhrasesDialogComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  displayedColumns: string[] = ['id', 'name', 'act'];
  data: IArticleByParam[] = [];
  resultsLength: number;

  public selectedWords = new Array<IWord>();
  public selectedPhrases = new Array<IWordPhrase>();

  public isLoading = true;
  refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  constructor(
    @Inject(MAT_DIALOG_DATA) public dialodData: any,
    private dialogRef: MatDialogRef<SelectWordPhraseDialogComponent>,
    private articleByParamService: ArticleByParamService,
    private toastr: ToastrService,
    public authorizeService: AuthorizeService,
    private appSettingsService: AppSettingsService,
  ) { }

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
          return this.articleByParamService.getFiltered(
            {
              sort: this.sort.active === undefined ? '' : this.sort.active,
              order: this.sort.direction,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
              int1: 0,
              searchText: '',
              str1: this.dialodData.baseName
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
      ).subscribe(resEnd => {
        this.data = resEnd;
        this.data.forEach((articleByParam: IArticleByParam) => {
          articleByParam.articleByParamData = JSON.parse(articleByParam.dataJson);
        });
        console.log('this.data', this.data);
      });
  }

  addWord(word: IWord) {
    if (this.selectedWords.some(x => x.wordID === word.wordID)) {
      this.selectedWords = this.selectedWords.filter(x => x.wordID !== word.wordID);
    } else {
      this.selectedWords.push(word);
    }
  }

  addPhrase(phrase: IWordPhrase) {
    if (this.selectedPhrases.some(x => x.wordPhraseID === phrase.wordPhraseID)) {
      this.selectedPhrases = this.selectedPhrases.filter(x => x.wordPhraseID !== phrase.wordPhraseID);
    } else {
      this.selectedPhrases.push(phrase);
    }
  }

  ngOnInit(): void {
    this.selectedWords = this.dialodData.selectedWords;
    this.selectedPhrases = this.dialodData.selectedPhrases;
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    this.dialogRef.close({selectedWords: this.selectedWords, selectedPhrases: this.selectedPhrases});
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
