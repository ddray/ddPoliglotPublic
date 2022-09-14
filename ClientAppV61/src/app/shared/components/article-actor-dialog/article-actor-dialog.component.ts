import { Component, OnInit, Inject } from '@angular/core';
import { IArticleActor } from '../../interfaces/IArticleActor';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-article-actor-dialog',
  templateUrl: './article-actor-dialog.component.html',
  styleUrls: ['./article-actor-dialog.component.scss']
})
export class ArticleActorDialogComponent implements OnInit {
  public dataItem: IArticleActor;
  public defaultInRoleDisabled = false;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
              private dialogRef: MatDialogRef<ArticleActorDialogComponent>,
              private toasterService: ToastrService,
  ) { }

  ngOnInit() {
    this.dataItem = this.data.source;
    this.defaultInRoleDisabled = this.dataItem.defaultInRole;
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    if (!this.dataItem.name) {
      this.toasterService.error('Please enter actor name', '', { positionClass: 'toast-bottom-center' });
      return;
    }

    if (!this.dataItem.voiceName) {
      this.toasterService.error('Please select voice name', '', { positionClass: 'toast-bottom-center' });
      return;
    }

    this.dialogRef.close(this.dataItem);
  }
}
