import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ISearchListArg } from '../../interfaces/IListArg';
import { UpdateWordTranslationDialogComponent } from '../update-word-translation-dialog/update-word-translation-dialog.component';

@Component({
  selector: 'app-update-template-json-dialog',
  templateUrl: './update-template-json-dialog.component.html',
  styleUrls: ['./update-template-json-dialog.component.scss']
})
export class UpdateTemplateJsonDialogComponent implements OnInit {
  public itemData = {} as ISearchListArg;
  public isV2 = false;
  public isV3 = false;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<UpdateWordTranslationDialogComponent>,
  ) { }

  ngOnInit() {
    this.itemData.str1 = this.data;
    console.log('this.data:', this.data);
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    this.itemData.int1 = this.isV3 
      ? 3 
      : this.isV2 
        ? 1 
        : 0;
    // console.log('this.data.item result:', this.itemData);
    this.dialogRef.close(this.itemData);
  }
}


