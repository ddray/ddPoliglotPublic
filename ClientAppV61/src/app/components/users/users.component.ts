import { Component, ViewChild, AfterViewInit, EventEmitter, OnDestroy } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, Observable, of as observableOf, Subject } from 'rxjs';
import { catchError, map, startWith, switchMap, takeUntil } from 'rxjs/operators';
import { IListArg } from '../../shared/interfaces/IListArg';
import { ToastrService } from 'ngx-toastr';
import { AppSettingsService } from '../../shared/services/app-settings.service';
import { MatDialog } from '@angular/material/dialog';
import { UpdateWordDialogComponent } from '../../shared/components/update-word-dialog/update-word-dialog.component';
import { IUser } from 'src/api-authorization/authorize.service';
import { UserService } from 'src/app/shared/services/user.service';
import { UpdateUserDialogComponent } from 'src/app/shared/components/update-user-dialog/update-user-dialog.component';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements AfterViewInit, OnDestroy {
  displayedColumns: string[] = ['name', 'roles', 'isLockedOut', 'act'];
  data: IUser[] = [];

  resultsLength = 0;
  public isLoading = true;
  refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  constructor(
    private userService: UserService,
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
          this.isLoading = true;
          return this.userService.getAll(
            {
              sort: this.sort.active === undefined ? '' : this.sort.active,
              order: this.sort.direction,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize
            } as IListArg
          );
        }),
        map(result => {
          this.isLoading = false;
          this.resultsLength = result.count;
          return result.data;
        }),
        catchError(() => {
          this.isLoading = false;
          return observableOf([]);
        })
      ).subscribe(data => {
          console.log('this.userService.getAll', data);
          this.data = data;
        }
      );
  }

  update(user: IUser) {
    const dialogRef = this.dialog.open(
      UpdateUserDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: { ...user },
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IUser) => {
        if (dialogResult && dialogResult !== null) {
          this.toastr.success('User roles updated', 'successful');
          this.refreshPage.emit(1);
        }
      });
  }

  delete(user: IUser) {
    if (window.confirm('Are sure you want to delete this item ?')) {
     this.isLoading = true;
     this.userService.Delete(user)
        .pipe(takeUntil(this.ngUnsubscribe$))
        .subscribe((result) => {
          this.toastr.success('User delete', 'deleted successful');
          this.isLoading = false;
          this.refreshPage.emit(1);
        });
    }
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
