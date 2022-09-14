import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { IMixParams } from '../../interfaces/IMixItem';
import { PhrasesMixType } from '../../enums/Enums';
import { AppSettingsService } from '../../services/app-settings.service';
import { IAppSettings } from '../../interfaces/IAppSettings';
import { MatDialog } from '@angular/material/dialog';
import { SelectTemplateTextDialogComponent } from '../select-template-text-dialog/select-template-text-dialog.component';
import { IMixParamTextTemp } from '../../interfaces/IMixParamTextTemp';

@Component({
  selector: 'app-mix-parameters',
  templateUrl: './mix-parameters.component.html',
  styleUrls: ['./mix-parameters.component.scss']
})
export class MixParametersComponent implements OnInit {
  @Input() public resultData: IMixParams;
  @Input() public showGlobals: boolean;

  @Output() public resultDataChange = new EventEmitter<IMixParams>();

  public appSetting: IAppSettings;

  public phrasesMixType = PhrasesMixType;

  constructor(private appSettingsService: AppSettingsService, private dialog: MatDialog,
) { }

  ngOnInit() {
    this.appSetting = this.appSettingsService.getCurrent();
  }

  modelChanged(event) {
    console.log('resultData:', this.resultData);
    this.resultDataChange.emit(this.resultData);
  }

  getDictorTextFromTemplate(tempKey, formControlName) {
    const dialogRef = this.dialog.open(
      SelectTemplateTextDialogComponent,
      {
        disableClose: true,
        width: '800px',
        data: { tempKey },
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: IMixParamTextTemp[]) => {
        if (dialogResult) {
          const val = this.resultData[formControlName];
          let res = '';
          dialogResult.forEach(x => {
            res = (res ? (res + '\n') : '') + x.text;
          })

          res = (val ? val + '\n' : '') + res;
          this.resultData[formControlName]= res;
        }
      });
  }

}
