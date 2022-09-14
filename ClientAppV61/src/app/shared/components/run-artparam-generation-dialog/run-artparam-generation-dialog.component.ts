import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IArticleActor } from '../../interfaces/IArticleActor';
import { IArtParamsGenerationArg } from '../../interfaces/IListArg';

@Component({
  selector: 'app-run-artparam-generation-dialog',
  templateUrl: './run-artparam-generation-dialog.component.html',
  styleUrls: ['./run-artparam-generation-dialog.component.scss']
})
export class RunArtparamGenerationDialogComponent implements OnInit {
  public selectedActorKeyGuid: string;
  public selectedTrActorKeyGuid: string;

  constructor(@Inject(MAT_DIALOG_DATA) public data: IArtParamsGenerationArg,
    private dialogRef: MatDialogRef<RunArtparamGenerationDialogComponent>,
  ) { }

  ngOnInit() {
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    this.dialogRef.close({...this.data});
  }
}
