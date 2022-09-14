import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { IUser, AuthorizeService } from 'src/api-authorization/authorize.service';
import { ToastrService } from 'ngx-toastr';
import { GlobalService } from 'src/app/shared/services/global.service';
import { AppSettingsService } from 'src/app/shared/services/app-settings.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { takeUntil, concatMap } from 'rxjs/operators';
import { Utils, Guid } from 'src/app/shared/models/System';
import { ILesson } from 'src/app/shared/interfaces/ILesson';
import { LessonService } from 'src/app/shared/services/lesson.service';
import { SelectFileDialogComponent } from 'src/app/shared/components/select-file-dialog/select-file-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { SelectArticleByParamDialogComponent } from 'src/app/shared/components/select-article-by-param-dialog/select-article-by-param-dialog.component';
import { IArticleByParam } from 'src/app/shared/interfaces/IArticleByParam';

@Component({
  selector: 'app-lesson',
  templateUrl: './lesson.component.html',
  styleUrls: ['./lesson.component.scss']
})
export class LessonComponent  implements OnInit, OnDestroy {
  @Input() paramData: ILesson;
  @Output() closeDialog = new EventEmitter();

  public isLoadingResults = false;
  public formGr: FormGroup;
  public currentData: ILesson;
  public submited = false;

  private ngUnsubscribe$ = new Subject();
  private currentUser: IUser;

  constructor(
    private toasterService: ToastrService,
    private fb: FormBuilder,
    private globalService: GlobalService,
    public authorizeService: AuthorizeService,
    private appSettingsService: AppSettingsService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private lessonService: LessonService,
    private dialog: MatDialog,
  ) {
  }

  ngOnInit() {
    if (!this.paramData) {
      // colled as separate page
      this.activatedRoute.paramMap
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe((params: ParamMap) => {
        this.authorizeService.getCurrentUser()
          .pipe(takeUntil(this.ngUnsubscribe$))
          .subscribe((user) => {
            this.currentUser = user;
            this.getData(params.get('id'));
          });
      });
    } else {
      // called inside dialog to update child record
      this.getData(this.paramData.lessonID);
    }

    this.formGr = this.fb.group({
      name: [null, [Validators.required]],
      pageName: [null, [Validators.required]],
      description: [null],
      content: [null],
      order: [null],
      video1: [null],
      audio1: [null],
      description1: [null],
      video2: [null],
      audio2: [null],
      description2: [null],
      video3: [null],
      audio3: [null],
      description3: [null],
      video4: [null],
      audio4: [null],
      description4: [null],
      video5: [null],
      audio5: [null],
      description5: [null],
      image1: [null],
      image2: [null],
      image3: [null],
      image4: [null],
      image5: [null],
      pageMetaTitle: [null],
      pageMetaDescription: [null],
  });
  }

  getData(id) {
    if (id > 0) {
      this.setLoader(true);
      this.lessonService.getById(id)
        .toPromise()
        .then(result => {
          console.log('lesson.getById: ', result);
          this.currentData = result;

          this.formGr.get('name').setValue(this.currentData.name);
          this.formGr.get('pageName').setValue(this.currentData.pageName);
          this.formGr.get('description').setValue(this.currentData.description);
          this.formGr.get('content').setValue(this.currentData.content);
          this.formGr.get('order').setValue(this.currentData.order);
          this.formGr.get('video1').setValue(this.currentData.video1);
          this.formGr.get('audio1').setValue(this.currentData.audio1);
          this.formGr.get('description1').setValue(this.currentData.description1);
          this.formGr.get('video2').setValue(this.currentData.video2);
          this.formGr.get('audio2').setValue(this.currentData.audio2);
          this.formGr.get('description2').setValue(this.currentData.description2);
          this.formGr.get('video3').setValue(this.currentData.video3);
          this.formGr.get('audio3').setValue(this.currentData.audio3);
          this.formGr.get('description3').setValue(this.currentData.description3);
          this.formGr.get('video4').setValue(this.currentData.video4);
          this.formGr.get('audio4').setValue(this.currentData.audio4);
          this.formGr.get('description4').setValue(this.currentData.description4);
          this.formGr.get('video5').setValue(this.currentData.video5);
          this.formGr.get('audio5').setValue(this.currentData.audio5);
          this.formGr.get('description5').setValue(this.currentData.description5);
          this.formGr.get('image1').setValue(this.currentData.image1);
          this.formGr.get('image2').setValue(this.currentData.image2);
          this.formGr.get('image3').setValue(this.currentData.image3);
          this.formGr.get('image4').setValue(this.currentData.image4);
          this.formGr.get('image5').setValue(this.currentData.image5);
          this.formGr.get('pageMetaTitle').setValue(this.currentData.pageMetaTitle);
          this.formGr.get('pageMetaDescription').setValue(this.currentData.pageMetaDescription);
            }).catch(err => {
          console.log('err: ', err);
        }).finally(() => {
          this.setLoader(false);
        });
    } else {
      const currAppSettings = this.appSettingsService.getCurrent();
      this.currentData = {
        lessonID: 0,
        parentID: this.paramData?.parentID ?? 0,
        languageID: currAppSettings.LearnLanguage.languageID,
        nativeLanguageID: currAppSettings.NativeLanguage.languageID,
        name: 'urok- pagename',
        pageName: 'urok-',
        description: 'Description test',
        content: 'Content test',
        order: 0,
        video1: '',
        audio1: '',
        description1: '',
        video2: '',
        audio2: '',
        description2: '',
        video3: '',
        audio3: '',
        description3: '',
        video4: '',
        audio4: '',
        description4: '',
        video5: '',
        audio5: '',
        description5: '',
        image1: '',
        image2: '',
        image3: '',
        image4: '',
        image5: '',
        articleByParamID: 0,
        articleByParam: {} as IArticleByParam,
        pageMetaTitle: '',
        pageMetaDescription: '',
      } as ILesson;
    }
  }

  onSubmit() {
    this.submited = true;

    if (this.formGr.invalid) {
      return;
    }

    this.currentData.name = this.formGr.get('name').value;
    this.currentData.pageName = this.formGr.get('pageName').value;
    this.currentData.description = this.formGr.get('description').value;
    this.currentData.content = this.formGr.get('content').value;
    this.currentData.video1 = this.formGr.get('video1').value;
    this.currentData.audio1 = this.formGr.get('audio1').value;
    this.currentData.description1 = this.formGr.get('description1').value;
    this.currentData.video2 = this.formGr.get('video2').value;
    this.currentData.audio2 = this.formGr.get('audio2').value;
    this.currentData.description2 = this.formGr.get('description2').value;
    this.currentData.video3 = this.formGr.get('video3').value;
    this.currentData.audio3 = this.formGr.get('audio3').value;
    this.currentData.description3 = this.formGr.get('description3').value;
    this.currentData.video4 = this.formGr.get('video4').value;
    this.currentData.audio4 = this.formGr.get('audio4').value;
    this.currentData.description4 = this.formGr.get('description4').value;
    this.currentData.video5 = this.formGr.get('video5').value;
    this.currentData.audio5 = this.formGr.get('audio5').value;
    this.currentData.description5 = this.formGr.get('description5').value;
    this.currentData.image1 = this.formGr.get('image1').value;
    this.currentData.image2 = this.formGr.get('image2').value;
    this.currentData.image3 = this.formGr.get('image3').value;
    this.currentData.image4 = this.formGr.get('image4').value;
    this.currentData.image5 = this.formGr.get('image5').value;
    this.currentData.pageMetaTitle = this.formGr.get('pageMetaTitle').value;
    this.currentData.pageMetaDescription = this.formGr.get('pageMetaDescription').value;

    if (this.currentData.lessonID !== 0) {
      this.currentData.order = this.formGr.get('order').value;
    }

    this.setLoader(true);
    this.lessonService.Save(this.currentData)
      .toPromise()
      .then(result => {
        this.toasterService.success('Lesson updated', 'successful');
        if (this.paramData) {
          this.closeDialog.emit(result);
        }

        if (this.currentData.lessonID) {
          this.currentData.lessonID = result.lessonID;
          this.currentData.order = result.order;
          this.setLoader(false);
        } else {
          this.router.navigate([`/lesson/${result.lessonID}`]);
        }
      });
  }

  openSelectFileDialog(fNum: number, fType: string, maxWidthForImage = 0) {
    const dialogRef = this.dialog.open(
      SelectFileDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: {type: fType, maxWidthForImage},
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: ILesson) => {
        if (dialogResult && dialogResult !== null) {
          this.formGr.get(fType + fNum.toString()).setValue(dialogResult);
        }
      });
  }

  setLoader(value) {
    this.isLoadingResults = value;
  }

  selectArticleByParam() {
    const dialogRef = this.dialog.open(
      SelectArticleByParamDialogComponent,
      {
        disableClose: true,
        width: '800px',
        data: this.currentData.articleByParam,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IArticleByParam) => {
        if (dialogResult && dialogResult !== null) {
          this.currentData.articleByParam = dialogResult;
          this.currentData.articleByParamID = dialogResult.articleByParamID;
        }
      });
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
