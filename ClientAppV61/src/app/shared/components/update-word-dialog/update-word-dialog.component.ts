import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { IWord } from '../../interfaces/IWord';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { WordService } from '../../services/word.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-update-word-dialog',
  templateUrl: './update-word-dialog.component.html',
  styleUrls: ['./update-word-dialog.component.scss']
})
export class UpdateWordDialogComponent implements OnInit {
  public formGr: FormGroup;
  isLoading = false;
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;

  constructor(@Inject(MAT_DIALOG_DATA) public data: IWord,
              private dialogRef: MatDialogRef<UpdateWordDialogComponent>,
              private fb: FormBuilder,
              private wordService: WordService,
  ) { }

  ngOnInit() {
    this.formGr = this.fb.group({
      text: [this.data.text, [Validators.required]],
      rate: [this.data.rate, [Validators.required]],
      pronunciation: [this.data.pronunciation],
    });
  }

  onSubmit() {
    if (this.formGr.invalid) {
      alert('something i wrong with form');
      return;
    }

    this.isLoading = true;
    this.wordService.Save({
        wordID: this.data.wordID,
        languageID: this.data.languageID,
        text: this.formGr.get('text').value,
        rate: this.formGr.get('rate').value,
        pronunciation: this.formGr.get('pronunciation').value,
        hashCode: 0,
        textSpeechFileName: '',
        speachDuration: 0,
        hashCodeSpeed1: 0,
        textSpeechFileNameSpeed1: '',
        speachDurationSpeed1: 0,
        hashCodeSpeed2: 0,
        textSpeechFileNameSpeed2: '',
        speachDurationSpeed2: 0,
      } as IWord
    ).toPromise()
      .then((x) => {
        this.dialogRef.close(x);
      }).finally(() => {
        this.isLoading = false;
      });
  }

  cancel() {
    this.isLoading = false;
    this.dialogRef.close(null);
  }
}
