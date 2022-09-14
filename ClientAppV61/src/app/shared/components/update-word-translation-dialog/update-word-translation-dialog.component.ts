import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IWord } from '../../interfaces/IWord';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-update-word-translation-dialog',
  templateUrl: './update-word-translation-dialog.component.html',
  styleUrls: ['./update-word-translation-dialog.component.scss']
})
export class UpdateWordTranslationDialogComponent implements OnInit {
  public itemData: IWord;
  isLoading = false;
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<UpdateWordTranslationDialogComponent>,
  ) { }

  ngOnInit() {
    this.itemData = this.data.itemData;
    console.log('this.data.item:', this.data);
  }

  cancel() {
    this.dialogRef.close(null);
  }

  save() {
    console.log('this.data.item result:', this.itemData);
    this.dialogRef.close(this.itemData);
  }
}

