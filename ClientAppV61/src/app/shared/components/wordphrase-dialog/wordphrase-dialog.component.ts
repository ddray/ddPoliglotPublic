import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { GlobalService } from '../../services/global.service';
import { FormControl, Validators, FormBuilder, FormGroup } from '@angular/forms';
import { IAppSettings } from '../../interfaces/IAppSettings';
import { AppSettingsService } from '../../services/app-settings.service';
import { IWordPhrase } from '../../interfaces/IWordPhrase';
import { WordService } from '../../services/word.service';
import { IWordPhraseTranslation } from '../../interfaces/IWordPhraseTranslation';
import { TranslateService } from '../../services/translate.service';
import { ITranslateArg } from '../../interfaces/ITranslateArg';
import { ITranslateResult } from '../../interfaces/ITranslateResult';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-wordphrase-dialog',
  templateUrl: './wordphrase-dialog.component.html',
  styleUrls: ['./wordphrase-dialog.component.scss']
})
export class WordphraseDialogComponent implements OnInit {
  public formGr: FormGroup;
  public dataItem: IWordPhrase;
  public isTranslateLoader = false;
  isLoading = false;
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<WordphraseDialogComponent>,
    private globalService: GlobalService,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private appSettingsService: AppSettingsService,
  ) {
  }

  async ngOnInit() {
    this.dataItem = this.data.wordPhrase;
    this.formGr = this.fb.group({
      phraseText: [this.dataItem.text, [Validators.required]],
      phraseTranslationText: [this.dataItem.wordPhraseTranslation ? this.dataItem.wordPhraseTranslation.text : null],
      phraseOrderInCurrentWord: [this.dataItem.phraseOrderInCurrentWord,
        [Validators.required, Validators.pattern('^[0-9]*$'), Validators.min(0), Validators.max(1001)]]
    });
  }

  translatePhrases() {
    this.isTranslateLoader = true;
    this.translateService
      .translate({ sourceText: this.formGr.get('phraseText').value } as ITranslateArg)
      .toPromise()
      .then((result: ITranslateResult) => {
        this.formGr.get('phraseTranslationText').setValue(result.message);
      }).finally(() => {
        this.isTranslateLoader = false;
      });
  }

  cancel() {
    this.dialogRef.close(null);
  }

  onSubmit() {
    if (this.formGr.invalid) {
      return;
    }

    const result: IWordPhrase = { ...this.dataItem };
    result.text = this.formGr.get('phraseText').value;

    if (!result.wordPhraseTranslation) {
      result.wordPhraseTranslation = {
        wordPhraseTranslationID: 0,
        wordPhraseID: result.wordPhraseID,
        languageID: this.appSettingsService.getCurrent().NativeLanguage.languageID,
       } as IWordPhraseTranslation;
    }

    result.wordPhraseTranslation.text = this.formGr.get('phraseTranslationText').value;
    result.phraseOrderInCurrentWord = this.formGr.get('phraseOrderInCurrentWord').value,

    this.dialogRef.close(result);
  }
}
