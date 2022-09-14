import { Component, OnInit, Inject } from '@angular/core';
import { IWordPhrase } from '../../interfaces/IWordPhrase';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-update-wordphrase-translation-dialog',
  templateUrl: './update-wordphrase-translation-dialog.component.html',
  styleUrls: ['./update-wordphrase-translation-dialog.component.scss']
})
export class UpdateWordphraseTranslationDialogComponent implements OnInit {
  public itemData: IWordPhrase;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<UpdateWordphraseTranslationDialogComponent>,
  ) { }

  ngOnInit() {
    this.itemData = this.data.itemData;
     console.log('this.data.item:', this.data);
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    console.log('this.data.item result:', this.itemData);
    this.dialogRef.close(this.itemData);
  }
}