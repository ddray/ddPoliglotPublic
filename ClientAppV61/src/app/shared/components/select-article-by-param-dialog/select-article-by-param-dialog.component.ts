import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { IArticleByParam } from '../../interfaces/IArticleByParam';

@Component({
  selector: 'app-select-article-by-param-dialog',
  templateUrl: './select-article-by-param-dialog.component.html',
  styleUrls: ['./select-article-by-param-dialog.component.css']
})
export class SelectArticleByParamDialogComponent implements OnInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<SelectArticleByParamDialogComponent>
  ) { }

  ngOnInit() {

  }

  cancel() {
    this.dialogRef.close(null);
  }

  selectArticleByParam(item: IArticleByParam) {
    this.dialogRef.close(item);
  }
}

