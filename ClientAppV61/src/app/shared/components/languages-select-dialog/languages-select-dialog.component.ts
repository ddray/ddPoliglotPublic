import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ILanguage } from '../../interfaces/ILanguage';
import { GlobalService } from '../../services/global.service';
import { FormControl, Validators, FormBuilder, FormGroup } from '@angular/forms';
import { MyErrorStateMatcher } from '../../models/System';
import { IAppSettings } from '../../interfaces/IAppSettings';
import { AppSettingsService } from '../../services/app-settings.service';

@Component({
  selector: 'app-languages-select-dialog',
  templateUrl: './languages-select-dialog.component.html',
  styleUrls: ['./languages-select-dialog.component.scss']
})
export class LanguagesSelectDialogComponent implements OnInit {
  public languageN: string;
  public languageL: string;
  public languages: Array<ILanguage>;
  public onlyOk = false;

  public matcher = new MyErrorStateMatcher();

  public formGr = this.fb.group({
    languageL: [null, [Validators.required]],
    languageN: [null, [Validators.required]],
  }, {validator: this.checkLng });

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<LanguagesSelectDialogComponent>,
    private globalService: GlobalService,
    private fb: FormBuilder,
    private appSettingsService: AppSettingsService,
  ) {
  }

  async ngOnInit() {
    this.languages = await this.globalService.getLanguages();
    const currAppSettings = this.appSettingsService.getCurrent();
    if (currAppSettings) {
      this.formGr.setValue({
        languageL: currAppSettings.LearnLanguage.code,
        languageN: currAppSettings.NativeLanguage.code
      });
    }

    this.onlyOk = this.data.onlyOk;
  }

  cancel() {
    this.dialogRef.close(null);
  }

  onSubmit() {
    if (this.formGr.invalid) {
      return;
    }

    const res = {
      NativeLanguage: this.languages.find(x => x.code === this.formGr.value.languageN),
      LearnLanguage: this.languages.find(x => x.code === this.formGr.value.languageL),
    } as IAppSettings;

    this.dialogRef.close(res);
  }

  checkLng(group: FormGroup) {
    const lngN = group.get('languageL').value;
    const lngL = group.get('languageN').value;

    return lngN === lngL ? { isSame: true } : null;
  }
}
