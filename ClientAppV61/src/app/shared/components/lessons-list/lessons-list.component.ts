import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { merge, Observable, of as observableOf, Subject } from 'rxjs';
import { catchError, map, startWith, switchMap, takeUntil } from 'rxjs/operators';
import { ILesson } from '../../interfaces/ILesson';
import { IArtParamsGenerationArg, ISearchListArg } from '../../interfaces/IListArg';
import { AppSettingsService } from '../../services/app-settings.service';
import { LessonService } from '../../services/lesson.service';
import { RunArtparamGenerationDialogComponent } from '../run-artparam-generation-dialog/run-artparam-generation-dialog.component';
import { UpdateLessonDialogComponent } from '../update-lesson-dialog/update-lesson-dialog.component';

@Component({
  selector: 'app-lessons-list',
  templateUrl: './lessons-list.component.html',
  styleUrls: ['./lessons-list.component.css']
})
export class LessonsListComponent implements OnInit, AfterViewInit, OnDestroy {
  @Input() canSelect = true;

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: false }) sort: MatSort;

  public displayedColumns: string[] = ['LessonID', 'Order', 'Name', 'Description', 'act'];
  public data: ILesson[] = [];
  public resultsLength = 0;
  public isLoading = true;

  private refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  constructor(
    private lessonService: LessonService,
    private toastr: ToastrService,
    private appSettingsService: AppSettingsService,
    private dialog: MatDialog,
    private router: Router
  ) {
    }

  ngOnInit(): void {
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
          return this.lessonService.getAll(
            {
              sort: this.sort.active === undefined ? '' : this.sort.active,
              order: this.sort.direction,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
              parentID: 0,
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
          return observableOf([]);
        })
      ).subscribe(data => {
        this.data = data;
      });
  }

  addNew() {
    this.router.navigate(['/lesson/0']);
  }

  update(lesson: ILesson) {
    this.router.navigate([`/lesson/${lesson.lessonID}`]);
  }

  openUpdateDialog(lesson: ILesson) {
    const dialogRef = this.dialog.open(
      UpdateLessonDialogComponent,
      {
        disableClose: true,
        width: '1000px',
        data: lesson,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: ILesson) => {
        if (dialogResult && dialogResult !== null) {
          this.toastr.success('Lesson updated', 'successful');
          this.refreshPage.emit(1);
        }
      });
  }

  delete(lesson: ILesson) {
    if (window.confirm('Are sure you want to delete this item ?')) {
     this.isLoading = true;
     this.lessonService.Delete(lesson)
        .pipe(takeUntil(this.ngUnsubscribe$))
        .subscribe((result) => {
          this.toastr.success('Lesson delete', 'deleted successful');
          this.isLoading = false;
          this.refreshPage.emit(1);
        });
    }
  }

  uploadVideosToYoutube()
  {
    this.isLoading = true;

    const dialogRef = this.dialog.open(
      RunArtparamGenerationDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: { 
          articleByParamID: -1,
           startWordRate: -1,
           startLessonNum: 1,
           lessonQty: 2,
           wordsByLesson: -1,
           baseName: 'en_v1_lesson',
           maxWordsForRepetition: -1,   
        } as IArtParamsGenerationArg,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IArtParamsGenerationArg) => {
        if (dialogResult && dialogResult !== null) {
          this.isLoading = true;
          this.lessonService.uploadVideosToYoutube(dialogResult)
          .pipe(takeUntil(this.ngUnsubscribe$))
          .subscribe((result) => {
            this.toastr.success('Videos uploaded', 'upload successful');
            this.isLoading = false;
            this.refreshPage.emit(1);
          });
        }
      });
  }

  generate() {
    this.isLoading = true;

    const dialogRef = this.dialog.open(
      RunArtparamGenerationDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: { 
          articleByParamID: -1,
           startWordRate: -1,
           startLessonNum: 1,
           lessonQty: 2,
           wordsByLesson: -1,
           baseName: 'en_v1_lesson',
           maxWordsForRepetition: -1,   
        } as IArtParamsGenerationArg,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IArtParamsGenerationArg) => {
        if (dialogResult && dialogResult !== null) {
          this.isLoading = true;
          this.lessonService.generateFromArticles(dialogResult)
          .pipe(takeUntil(this.ngUnsubscribe$))
          .subscribe((result) => {
            this.toastr.success('Lesson generation', 'generation successful');
            this.isLoading = false;
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
