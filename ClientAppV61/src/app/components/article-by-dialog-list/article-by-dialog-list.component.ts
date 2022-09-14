import { Component, OnInit, AfterViewInit, OnDestroy, EventEmitter, ViewChild } from '@angular/core';
import { IArticleByParam } from '../../shared/interfaces/IArticleByParam';
import { Subject, merge, of } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ArticleByParamService } from '../../shared/services/articleByParam.service';
import { ToastrService } from 'ngx-toastr';
import { AppSettingsService } from '../../shared/services/app-settings.service';
import { takeUntil, startWith, switchMap, map, catchError } from 'rxjs/operators';
import { ISearchListArg } from '../../shared/interfaces/IListArg';
import { AuthorizeService } from 'src/api-authorization/authorize.service';

@Component({
  selector: 'app-article-by-dialog-list',
  templateUrl: './article-by-dialog-list.component.html',
  styleUrls: ['./article-by-dialog-list.component.scss']
})
export class ArticleByDialogListComponent implements AfterViewInit, OnDestroy {
  displayedColumns: string[] = ['id', 'name', 'act'];
  data: IArticleByParam[] = [];

  public isLoading = true;
  refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;
  resultsLength: number;

  constructor(
    private articleByParamService: ArticleByParamService,
    private toastr: ToastrService,
    private appSettingsService: AppSettingsService,
    public authorizeService: AuthorizeService,
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
              sort: this.sort.active === undefined ? "" : this.sort.active,
              order: this.sort.direction,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
              int1: 1,
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
      })
      .finally(() => {
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
    this.isLoading = true;
    this.articleByParamService.generate(item)
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

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
