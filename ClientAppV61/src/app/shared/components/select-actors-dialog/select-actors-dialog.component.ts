import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IArticleActor } from '../../interfaces/IArticleActor';

@Component({
  selector: 'app-select-actors-dialog',
  templateUrl: './select-actors-dialog.component.html',
  styleUrls: ['./select-actors-dialog.component.scss']
})
export class SelectActorsDialogComponent implements OnInit {
  public selectedActorKeyGuid: string;
  public selectedTrActorKeyGuid: string;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<SelectActorsDialogComponent>,
  ) { }

  ngOnInit() {
    this.selectedActorKeyGuid = this.data.actor.keyGuid;
    this.selectedTrActorKeyGuid = this.data.trActor.keyGuid;
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    this.dialogRef.close({ key: this.selectedActorKeyGuid, trKey: this.selectedTrActorKeyGuid });
  }
}
