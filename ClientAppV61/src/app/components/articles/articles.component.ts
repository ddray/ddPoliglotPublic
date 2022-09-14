import { Component, ViewChild, AfterViewInit, EventEmitter, OnDestroy } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, Observable, of as observableOf, Subject, combineLatest } from 'rxjs';
import { catchError, map, startWith, switchMap, takeUntil } from 'rxjs/operators';
import { ArticleService } from '../../shared/services/article.service';
import { IArticle } from '../../shared/interfaces/IArticle';
import { IArtParamsGenerationArg, IListArg } from '../../shared/interfaces/IListArg';
import { ToastrService } from 'ngx-toastr';
import { AppSettingsService } from 'src/app/shared/services/app-settings.service';
import { RunArtparamGenerationDialogComponent } from 'src/app/shared/components/run-artparam-generation-dialog/run-artparam-generation-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-articles',
  templateUrl: './articles.component.html',
  styleUrls: ['./articles.component.scss']
})
export class ArticlesComponent implements AfterViewInit, OnDestroy {
  displayedColumns: string[] = ['articleID', 'name', 'language', 'languageTranslation', 'act'];
  data: IArticle[] = [];

  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  constructor(
    private articleService: ArticleService,
    private toastr: ToastrService,
    private appSettingsService: AppSettingsService,
    private dialog: MatDialog,
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
          this.isLoadingResults = true;
          return this.articleService.getAll(
            {
              sort: this.sort.active === undefined ? '' : this.sort.active,
              order: this.sort.direction,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize
            } as IListArg
          );
        }),
        map(result => {
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          this.resultsLength = result.count;

          return result.data;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          this.isRateLimitReached = true;
          return observableOf([]);
        })
      ).subscribe(data =>
        this.data = data
      );
  }

  editRow(article: IArticle) {
    console.log(article);
  }

  delete(id) {
    if (window.confirm('Are sure you want to delete this item ?')) {
      this.isLoadingResults = true;
      this.articleService.Delete(id)
        .pipe(takeUntil(this.ngUnsubscribe$))
        .subscribe((result) => {
          this.toastr.success('Article delete', 'deleted successful');
          this.refreshPage.emit(1);
        }, () => {
          this.isLoadingResults = false;
        });
    }
  }

  generate() {
    this.isLoadingResults = true;

    const dialogRef = this.dialog.open(
      RunArtparamGenerationDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: { 
          articleByParamID: -1,
           startWordRate: -1,
           startLessonNum: 1,
           endLessonNum: 1,
           wordsByLesson: -1,
           baseName: 'en_v1_lesson',
           maxWordsForRepetition: -1,   
        } as IArtParamsGenerationArg,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IArtParamsGenerationArg) => {
        if (dialogResult && dialogResult !== null) {
          this.isLoadingResults = true;
          this.articleService.generateVideoAndAudio(dialogResult)
          .pipe(takeUntil(this.ngUnsubscribe$))
          .subscribe((result) => {
            this.toastr.success('Lesson generation', 'generation successful');
            this.isLoadingResults = false;
            this.refreshPage.emit(1);
          });
        }
      });
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
