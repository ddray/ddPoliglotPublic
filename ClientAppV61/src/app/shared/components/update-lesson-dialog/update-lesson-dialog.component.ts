import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IUser } from 'src/api-authorization/authorize.service';
import { ILesson } from '../../interfaces/ILesson';

@Component({
  selector: 'app-update-lesson-dialog',
  templateUrl: './update-lesson-dialog.component.html',
  styleUrls: ['./update-lesson-dialog.component.scss']
})
export class UpdateLessonDialogComponent implements OnInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: ILesson,
    private dialogRef: MatDialogRef<UpdateLessonDialogComponent>
  ) { }

  ngOnInit() {
  }

  onClose(event) {
    this.dialogRef.close(event);
  }
}
