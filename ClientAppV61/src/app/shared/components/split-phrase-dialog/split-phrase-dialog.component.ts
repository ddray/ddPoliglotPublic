import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { IMixItem, IMixParams } from '../../interfaces/IMixItem';
import { TranslateService } from '../../services/translate.service';
import { Guid, Utils } from '../../models/System';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { ArticleService } from '../../services/article.service';
import { MixParamService } from '../../services/mix-param.service';
import { PhrasesSplitType, PhrasesMixType } from '../../enums/Enums';

@Component({
  selector: 'app-split-phrase-dialog',
  templateUrl: './split-phrase-dialog.component.html',
  styleUrls: ['./split-phrase-dialog.component.scss']
})
export class SplitPhraseDialogComponent implements OnInit {
  public isLoading = false;
  public sources: Array<IMixItem> = [];
  public translations: Array<IMixItem> = [];
  public resultData: IMixParams;
  public phrasesSplitType = PhrasesSplitType;
  public phrasesMixType = PhrasesMixType;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<SplitPhraseDialogComponent>,
    private translateService: TranslateService,
    private mixParamService: MixParamService
  ) { }

  ngOnInit() {

    this.sources = this.spltAuto(this.data.articlePhrase.text);
    this.translations = this.spltAuto(this.data.articlePhrase.trText);

    this.resultData = {
      articlePhraseKeyGuid: this.data.articlePhrase.keyGuid,
      textHashCode: Utils.getHashCode(this.data.articlePhrase.text),
      trTextHashCode: Utils.getHashCode(this.data.articlePhrase.trText),
      mixItems: [],
      trFirst: false,
      active: false,
      m0: true,
      m0_repeat: false,
      m01: true,
      m01_repeat: false,
      m012: true,
      m012_repeat: true,
      m0123: true,
      m0123_repeat: true,
      trActive: true,
      trM0: true,
      trM0_repeat: true,
      trM01: true,
      trM01_repeat: true,
      trM012: true,
      trM012_repeat: true,
      trM0123: true,
      trM0123_repeat: true,
      phrasesMixType: this.data.phrasesMixType,
      repeat: 3,
      trRepeat: 3,
      repeatOrder: 3,
      trRepeatOrder: 3,
    } as IMixParams;

    if (this.data.phrasesMixType === PhrasesMixType.random) {
      this.resultData.active = true;
      this.resultData.repeatOrder = 2;
      this.resultData.addSlow2InRepeatOrder = true;
      this.resultData.addSlowInRepeatOrder = true;
      this.resultData.repeatBaseWord = 0;
      this.resultData.repeat = 0;

      this.resultData.trRepeatOrder = 0;
      this.resultData.trAddSlow2InRepeatOrder = false;
      this.resultData.trAddSlowInRepeatOrder = false;
      this.resultData.trRepeatBaseWord = 4;
      this.resultData.trRepeat = 4;
    }

    this.isLoading = true;

    this.mixParamService.getByArticlePhraseKeyGuid(this.data.articlePhrase.keyGuid)
      .toPromise()
      .then(x => {
        if (!x) {
          return; // there is no saved before
        }

        if (x.textHashCode !== this.resultData.textHashCode
          || x.trTextHashCode !== this.resultData.trTextHashCode
        ) {
          if (!window.confirm('There is old params already. But text was changed. Press (Ok) to use old one. Press (Cancel) to generate new')) {
            return;
          }
        }

        this.resultData = x;
        this.resultData.mixParamID = 0;
        this.resultData.mixItems.forEach(x => {
          x.mixItemID = 0;
          x.mixParamID = 0;
        });

        this.sources = [];
        this.translations = [];

        const mixItems = x.mixItems.sort((z, y) => z.orderNum - y.orderNum);
        mixItems.forEach((y) => {
          this.sources.push(this.getEmptySplitPhraseItem(y.source, y));
          this.translations.push(this.getEmptySplitPhraseItem(y.inContext, y));
        });

        this.sources.forEach(y => {
          y.inContext = '';
          y.inDict = '';
        });

        this.translations.forEach(y => {
          y.inContext = '';
        });
      })
      .finally(() => {
        this.isLoading = false;
      });
  }

  spltAuto(text: string): Array<IMixItem> {

    let separators = [' '];
    if (this.data.phrasesSplitType === PhrasesSplitType.dot) {
      separators = ['\\\.', '\\\!', '\\\?'];
    } else if (this.data.phrasesSplitType === PhrasesSplitType.NL) {
      separators = ['\r?\n'];
    }

    let ar1 = text.split(new RegExp(separators.join('|'), 'g'));
    let ar2: Array<string> = ar1.filter((x) => { return x.length > 0 });
    return ar2.map((x) => {
      let s = x.trim();
      if (this.data.phrasesSplitType !== PhrasesSplitType.NL) {
        s = s.replace(',', '').replace('.', '')
      }

      return this.getEmptySplitPhraseItem(s);
    });
  }

  getEmptySplitPhraseItem(str: string = '', mixItem: IMixItem = undefined): IMixItem {
    return {
      source: str,
      keyGuid: mixItem ? mixItem.keyGuid : Guid.newGuid().ToString(),
      inContext: mixItem ? mixItem.inContext : '',
      inDict: mixItem ? mixItem.inDict : '',
      endPhrase: mixItem ? mixItem.endPhrase : false,
      childrenType: '',
      pretext: mixItem ? mixItem.pretext : false,
      baseWord: mixItem ? mixItem.baseWord : false,
    } as IMixItem;
  }

  splitRow(item: IMixItem, control, type) {
    const message = control.value;
    let selStart = control.selectionStart;
    const selEnd = control.selectionEnd;

    if (selStart === 0 && selEnd === message.length) {
      return;
    }

    if (!selStart || selStart === message.length)
      return;

    if (selStart === 0 && selEnd !== message.length) {
      selStart = selEnd;
    }

    const items = type == 0 ? this.sources : this.translations;

    if (selStart === selEnd) {
      const str1 = message.substring(0, selStart);
      const str2 = message.substring(selStart, message.length);

      let index = items.findIndex((x) => { return x.keyGuid === item.keyGuid });
      if (index >= 0) {
        let oldItem = items[index];
        oldItem.source = str1.trim();

        items.splice(index, 1, oldItem);

        let newItem = this.getEmptySplitPhraseItem(str2.trim());
        items.splice(index + 1, 0, newItem);
      }
    } else {
      const str1 = message.substring(0, selStart).trim();
      const str2 = message.substring(selStart, selEnd).trim();
      const str3 = message.substring(selEnd, message.length).trim();

      let index = items.findIndex((x) => { return x.keyGuid === item.keyGuid });
      if (index >= 0) {
        let oldItem = items[index];
        oldItem.source = str1;

        items.splice(index, 1, oldItem);

        let newItem = this.getEmptySplitPhraseItem(str2);
        items.splice(index + 1, 0, newItem);

        if (selEnd < message.length) {
          let newItem2 = this.getEmptySplitPhraseItem(str3);
          items.splice(index + 2, 0, newItem2);
        }
      }
    }
  }

  mergeWithNextRow(item: IMixItem, type: number) {
    const items = type == 0 ? this.sources : this.translations;
    let index = items.findIndex((x) => { return x.keyGuid === item.keyGuid });
    if (index > 0) {
      // items[index].inContext = items[index].inContext + ' ' + items[index + 1].inContext;
      // items[index].inDict = items[index].inDict + ' ' + items[index + 1].inDict;
      items[index].source = items[index - 1].source + ' ' + items[index].source;
      items.splice(index - 1, 1);
    }
  }

  deleteRow(item: IMixItem) {
    let index = this.translations.findIndex((x) => { return x.keyGuid === item.keyGuid });
    this.translations.splice(index, 1);
  }

  drop(event: CdkDragDrop<string[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex);
    }
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {

    if (this.sources.length !== this.translations.length) {
      alert("left and right row qty should be equal!!!!");
      return;
    }

    this.resultData.mixItems = [];
    for (var i = 0; i < this.sources.length; i++) {
      this.resultData.mixItems.push({
        keyGuid: this.sources[i].keyGuid,
        inContext: this.translations[i].source.trim(),
        inDict: this.translations[i].inDict.trim(),
        source: this.sources[i].source,
        endPhrase: this.sources[i].endPhrase,
        childrenType: this.sources[i].childrenType,
        pretext: this.sources[i].pretext,
        orderNum: i,
        baseWord: this.sources[i].baseWord,
      } as IMixItem
      );
    }

    // save mixParams
    this.mixParamService.saveMixParam(this.resultData)
      .toPromise()
      .then(result => {
        // result
      }).finally(() => {
        this.dialogRef.close(this.resultData);
      });
  }
}
