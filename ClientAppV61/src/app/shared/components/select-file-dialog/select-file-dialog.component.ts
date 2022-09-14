import { Component, OnInit, Inject, ViewChild, EventEmitter, AfterViewInit, OnDestroy } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { LessonService } from '../../services/lesson.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Subject, merge, of } from 'rxjs';
import { takeUntil, startWith, switchMap, map, catchError } from 'rxjs/operators';
import { ISearchListArg } from '../../interfaces/IListArg';
import { HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-select-file-dialog',
  templateUrl: './select-file-dialog.component.html',
  styleUrls: ['./select-file-dialog.component.scss']
})
export class SelectFileDialogComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  public displayedColumns: string[] = ['Name', 'act'];
  public data: string[] = [];
  public resultsLength = 0;
  public isLoading = true;
  public progress: number;
  public message: string;

  private refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  constructor(
    @Inject(MAT_DIALOG_DATA) public dataArgs: {type: string, maxWidthForImage?: number},
    private dialogRef: MatDialogRef<SelectFileDialogComponent>,
    private lessonService: LessonService,
  ) { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    merge(this.paginator.page, this.refreshPage)
      .pipe(takeUntil(this.ngUnsubscribe$))
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoading = true;
          return this.lessonService.getAudioFileList(
            {
              sort: '',
              order: '',
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
              str1: this.dataArgs.type,
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
      });
  }

  uploadNew() {

  }

  uploadFile(files){
    this.isLoading = true;

    if (files.length === 0) {
      return;
    }

    const fileToUpload = files[0] as File;
    const formData = new FormData();
    const sData = {
      sort: '',
      order: '',
      page: this.paginator.pageIndex,
      pagesize: this.paginator.pageSize,
      str1: this.dataArgs.type,
      int1: this.dataArgs.maxWidthForImage ?? 0
    } as ISearchListArg;

    formData.append('file', fileToUpload, fileToUpload.name);
    this.lessonService.FileUpload(formData, sData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          this.progress = Math.round(100 * event.loaded / event.total);
        }
        else if (event.type === HttpEventType.Response) {
          this.message = 'Upload success.';
          this.refreshPage.emit(1);
        }
      });
  }

  select(name: string) {
    this.dialogRef.close(name);
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
