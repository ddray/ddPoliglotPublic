import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ToastrService } from 'ngx-toastr';
import { merge, of, Subject } from 'rxjs';
import { catchError, map, startWith, switchMap, takeUntil } from 'rxjs/operators';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { IArticleByParam } from '../../interfaces/IArticleByParam';
import { ILesson } from '../../interfaces/ILesson';
import { IArtParamsGenerationArg, ISearchListArg } from '../../interfaces/IListArg';
import { AppSettingsService } from '../../services/app-settings.service';
import { ArticleByParamService } from '../../services/articleByParam.service';
import { RunArtparamGenerationDialogComponent } from '../run-artparam-generation-dialog/run-artparam-generation-dialog.component';

@Component({
  selector: 'app-article-by-param-list',
  templateUrl: './article-by-param-list.component.html',
  styleUrls: ['./article-by-param-list.component.scss']
})
export class ArticleByParamListComponent implements OnInit, AfterViewInit, OnDestroy {
  @Input() canSelect = false;

  @Output() public selectRow = new EventEmitter<IArticleByParam>();

  displayedColumns: string[] = ['id', 'name', 'act'];
  data: IArticleByParam[] = [];

  public pagingSizes = [];
  public isLoading = true;
  refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
    resultsLength: number;

  constructor(
    private articleByParamService: ArticleByParamService,
    private toastr: ToastrService,
    public authorizeService: AuthorizeService,
    private appSettingsService: AppSettingsService,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {
    if (this.canSelect) {
      this.pagingSizes = [5, 10];
    } else {
      this.pagingSizes = [50, 100];
    }
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
          return this.articleByParamService.getFiltered(
            {
              sort: this.sort.active === undefined ? '' : this.sort.active,
              order: this.sort.direction,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
              int1: 0,
              searchText: ''
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
      ).subscribe(data =>
        this.data = data
      );
  }

  delete(item: IArticleByParam) {
    let confirm = false;
    if (!window.confirm('Are sure you want to delete this item ?')) {
      return;
    }

    if (item.isTemplate) {
      confirm = window.confirm('THIS IS TEMPLATE!!!! Are sure you want to delete this item ?');
    } else {
      confirm = true;
    }

    if (!confirm) {
      return;
    }

    this.isLoading = true;
    this.articleByParamService.delete(item)
      .toPromise()
      .then(x => {
        this.toastr.success('Params delete', 'deleted successful');
        this.paginator.pageIndex = 0;
        this.refreshPage.next();
      })      .finally(() => {
        this.isLoading = false;
      });
  }

  copy(item: IArticleByParam) {
    this.isLoading = true;
    this.articleByParamService.copy(item)
      .toPromise()
      .then(x => {
        this.toastr.success('Params dublicated', 'dublication successful');
        this.paginator.pageIndex = 0;
        this.refreshPage.next();
      })
      .finally(() => {
        this.isLoading = false;
      });
  }

  generate(item: IArticleByParam) {

    const dialogRef = this.dialog.open(
      RunArtparamGenerationDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: { 
          articleByParamID: item.articleByParamID,
           startWordRate: 1,
           startLessonNum: 1,
           endLessonNum: 11,
           wordsByLesson: 5,
           baseName: 'en_v1_lesson',
           maxWordsForRepetition: 25,   
        } as IArtParamsGenerationArg,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IArtParamsGenerationArg) => {
        if (dialogResult && dialogResult !== null) {
          this.isLoading = true;
          this.articleByParamService.generate(dialogResult)
            .toPromise()
            .then(x => {
              this.toastr.success('Generation', 'Generation successful');
              this.paginator.pageIndex = 0;
              this.refreshPage.next();
            })
            .finally(() => {
              this.isLoading = false;
            });
        }
      });



  }

  select(item: IArticleByParam) {
    this.selectRow.emit(item);
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
