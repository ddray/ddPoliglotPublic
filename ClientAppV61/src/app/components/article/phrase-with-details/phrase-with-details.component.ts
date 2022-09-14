// tslint:disable-next-line:max-line-length
import { Component, OnInit, Input, Output, EventEmitter, OnChanges, AfterViewInit, ViewChild, ElementRef, SimpleChanges } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { TranslateService } from '../../../shared/services/translate.service';
import { IArticlePhrase } from '../../../shared/interfaces/IArticlePhrase';
import { IArticle } from '../../../shared/interfaces/IArticle';
import { environment } from '../../../../environments/environment';
import { Utils } from 'src/app/shared/models/System';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';

@Component({
  selector: 'app-phrase-with-details',
  templateUrl: './phrase-with-details.component.html',
  styleUrls: ['./phrase-with-details.component.scss']
})
export class PhraseWithDetailsComponent implements OnInit, AfterViewInit, OnChanges {
  @Input() item: IArticlePhrase;
  @Input() parent: IArticle;
  @Input() public articleChanged: boolean;
  @Input() public showToolbox: boolean;

  @Output() public onInsertRowBefore = new EventEmitter();
  @Output() public onInsertRowAfter = new EventEmitter();
  @Output() public onDeleteRow = new EventEmitter();
  @Output() public onMoveUpRow = new EventEmitter();
  @Output() public onMoveDownRow = new EventEmitter();
  @Output() public onSplitRow = new EventEmitter();
  @Output() public onDublicateRow = new EventEmitter();
  @Output() public onAddDictorSpeechRow = new EventEmitter();
  @Output() public onTextToSpeachArticlePhrase = new EventEmitter();
  @Output() public onChangedRow = new EventEmitter();
  //@Output() public onDublicateWithHideRow = new EventEmitter();
  @Output() public onSplitByPhrasesRow = new EventEmitter();
  @Output() public onSplitBySubPhrasesRow = new EventEmitter();
  @Output() public onChangeRowActors = new EventEmitter();
  @Output() public onSetSilentRow = new EventEmitter();
  @Output() public onUnsetSilentRow = new EventEmitter();
  @Output() public onSplitForRandomGenerationRow = new EventEmitter();

  @ViewChild('tareaText', { static: false }) tareaText: ElementRef;
  @ViewChild('tareaTranslation', { static: false }) tareaTranslation: ElementRef;

  public processed = false;
  public isMousePresent = false;
  public phraseFileError = false;
  public phraseTranslationFileError = false;
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;

  private sourceTextInitFlag = false;
  private sourceTextInitValue = '';
  private translationTextInitFlag = false;
  private translationTextInitValue = '';

  private needCorrectTextHeight = false;

  get activityType(): string {
    return (this.item.activityType).toString();
  }

  set activityType(newVal: string) {
    this.item.activityType = (+newVal);
    // this.onChangedRow.emit(this.item);
  }

  constructor(
    private formBuilder: FormBuilder,
    private translateService: TranslateService,
  ) { }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges) {
    // console.log(changes);
    // if (changes && changes.item && changes.item.firstChange) {
    //   this.needCorrectTextHeight = true;
    // }
  }

  ngAfterViewInit() {
    console.log('ngAfterViewInit');
    // for adjast textaria height on load
    this.sourceTextInitValue = (<HTMLInputElement>this.tareaText.nativeElement).value;
    this.sourceTextInitFlag = false;
    this.translationTextInitValue = (<HTMLInputElement>this.tareaTranslation.nativeElement).value;
    this.translationTextInitFlag = false;

    // if (!this.needCorrectTextHeight) {
    //   this.tareaText.nativeElement.style.height = '1px';
    //   this.tareaText.nativeElement.style.height = (2 + this.tareaText.nativeElement.scrollHeight) + 'px';
    //   this.tareaTranslation.nativeElement.style.height = '1px';
    //   this.tareaTranslation.nativeElement.style.height = (2 + this.tareaTranslation.nativeElement.scrollHeight) + 'px';
    // }
  }

  ngAfterViewChecked() {
    // adjast textaria height on load
    if (!this.sourceTextInitFlag && this.sourceTextInitValue !== (<HTMLInputElement>this.tareaText.nativeElement).value) {
      this.sourceTextInitFlag = true;
      (<HTMLElement>this.tareaText.nativeElement).style.height = '1px';
      (<HTMLElement>this.tareaText.nativeElement).style.height = (2 + (<HTMLElement>this.tareaText.nativeElement).scrollHeight) + 'px';
    }
    if (!this.translationTextInitFlag && this.translationTextInitValue
  !== (<HTMLInputElement>this.tareaTranslation.nativeElement).value) {
      this.translationTextInitFlag = true;
      (<HTMLElement>this.tareaTranslation.nativeElement).style.height = '1px';
      (<HTMLElement>this.tareaTranslation.nativeElement).style.height
      = (2 + (<HTMLElement>this.tareaTranslation.nativeElement).scrollHeight) + 'px';
    }
  }

  Changed(event: any) {
    this.onChangedRow.emit(this.item);
    // this.textAreaAdjust(event);
    // console.log();
        // this.phraseFileError = this.isHashCorrect(this.item.phrase.textSpeechFileName, this.item.phrase.text)
  }

  textAreaAdjust(event: any) {
    // adjast textaria height on change
    event.srcElement.style.height = '1px';
    event.srcElement.style.height = (2 + event.srcElement.scrollHeight) + 'px';
  }

  isHashCorrect(textSpeechFileName, text) {
    if (textSpeechFileName) {
      const hash = Utils.getHashCode(text);
      const ar = textSpeechFileName.split('_');
      const fileHash = ar.length > 0 ? ar[0] : '';
      if (hash.toString() !== fileHash) {
        return true;
      }
    }

    return false;
  }

  translate() {
    this.setProcessed(true);
    const value = this.item.text;
    this.translateService
      .translate({ sourceText: value, sourceLanguage: this.parent.language, targetLanguage: this.parent.languageTranslation })
      .toPromise()
      .then((result) => {
        this.item.trText = result.message;
        this.setProcessed(false);
        this.onChangedRow.emit(this.item);
      });
  }

  splitByPhrasesRow() {
    this.onSplitByPhrasesRow.emit(this.item);
  }

  splitBySubPhrasesRow() {
    this.onSplitBySubPhrasesRow.emit(this.item);
  }

  splitForRandomGenerationRow() {
    this.onSplitForRandomGenerationRow.emit(this.item);
  }

  textToSpeechRow() {
    this.onTextToSpeachArticlePhrase.emit(this.item);
  }

  setProcessed(value: boolean) {
    this.processed = value;
  }

  insertRowBefore() {
    this.onInsertRowBefore.emit(this.item);
  }

  insertRowAfter() {
    this.onInsertRowAfter.emit(this.item);
  }

  deleteRow() {
    this.onDeleteRow.emit(this.item);
  }

  moveUpRow() {
    this.onMoveUpRow.emit(this.item);
  }

  moveDownRow() {
    this.onMoveDownRow.emit(this.item);
  }

  splitSelection(sourceTextArea, translatedTextArea) {
    this.onSplitRow.emit({ item: this.item, control: sourceTextArea, controlTranslated: translatedTextArea });
  }

  dublicateRow() {
    this.onDublicateRow.emit(this.item);
  }

  // dublicateWithHideRow() {
  //     this.onDublicateWithHideRow.emit(this.item);
  // }

  mouseOver() {
    this.isMousePresent = true;
  }

  mouseOut() {
    this.isMousePresent = false;
  }

  setSilentRow() {
    this.onSetSilentRow.emit(this.item);
  }

  unsetSilentRow() {
    this.onUnsetSilentRow.emit(this.item);
  }

  addDictorSpeechRow() {
    this.onAddDictorSpeechRow.emit(this.item);
  }

  changeRowActors() {
    this.onChangeRowActors.emit(this.item);
  }
}
