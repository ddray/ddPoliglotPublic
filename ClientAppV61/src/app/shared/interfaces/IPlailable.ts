import { IWord } from './IWord';
import { IWordPhrase } from './IWordPhrase';
import { IWordTranslation } from './IWordTranslation';
import { IWordPhraseTranslation } from './IWordPhraseTranslation';

export interface IPlailable {
  playListItem?: IPlayListItem; // item with audio object
  playListItemSpeed1?: IPlayListItem;
  playListItemSpeed2?: IPlayListItem;
}

export interface IPlailablePare {
  source: IPlailable;
  translation: IPlailable;
}

export enum PlayListItemTypes {
  word,
  wordSpeed1,
  wordSpeed2,
  wordTranslation,
  wordPhrase,
  wordPhraseSpeed1,
  wordPhraseSpeed2,
  wordPhraseTranslation,
  dictorPhrase,
  dialogOrWordPhrase,
}

export enum ScreenTextTypes {
  Text,
  Pron,
  Transl
}

export interface ScreenClass {
  level0: any;
  level1: any;
  level2: any;
}

export interface ScreenTextWithClass {
  value: string;
  screenClass: ScreenClass;
}

export interface CurrentScreen {
  сurrentScreenType: CurrentScreenTypes;
  activityType: PlayListActivityTypes;
  text: ScreenTextWithClass;
  pron: ScreenTextWithClass;
  transl: ScreenTextWithClass;
}

export enum CurrentScreenTypes {
  TextYesPronYesTranslNo_TextFirst = 0,
  TextYesPronYesTranslYes_TranslFirst = 1,
  TextYesPronYesTranslYes_TextFirst = 2,
  TextNoPronNoTranslYes_TranslFirst = 3,
  TextYesTranslNo_TextFirst = 4,
  TextYesTranslYes_TranslFirst = 5,
  TextYesTranslYes_TextFirst = 6,
  TextNoTranslYes_TranslFirst = 7,
  Dictor_TextFirst = 8,
}

export enum PlayListActivityTypes {
  textFirst,
  translationFirst,
}
export enum TextLen {
  Short,
  Middle,
  Long
}

export enum AudioSPlayerState {
  stoped,
  plaing,
  paused,
}

export interface IPlayListItem {
  itemID: number;
  itemParentID: number;
  itemRef: IWord | IWordPhrase | IWordTranslation | IWordPhraseTranslation;
  itemParentRef: IWord | IWordPhrase;
  text: string;
  pronunciation: string;
  textSpeechFileName: string;
  speachDuration: number;
  pause: number;
  type: number; // PlayListItemTypes..
  activityType: number; // PlayListActivityTypes
  audio?: HTMLAudioElement;
  order?: number;
  isCurrent?: boolean;
  fromSecond?: number;
}

export class PlayList {
  guid: string;
  name: string;
  active: boolean;
  isCurrent?: boolean;
  private items: IPlayListItem[];

  constructor(
    guid: string,
    name: string,
    active: boolean,
    isCurrent: boolean,
    items: IPlayListItem[],
  ) {
    this.active = active;
    this.guid = guid;
    this.isCurrent = isCurrent;
    this.name = name;
    this.items = items;
    this.recalculate();
  }

  recalculate() {
    let second = 0;
    this.items.forEach((item: IPlayListItem, index) => {
      item.order = index;
      item.fromSecond = second;
      second += (item.speachDuration + item.pause);
    });
  }

  getByIndex(index): IPlayListItem {
    return this.items[index];
  }

  length() {
    return this.items.length;
  }

  getBySeconds(seconds): IPlayListItem {
    const arr = [...this.items].reverse();
    return arr.find(x => x.fromSecond <= seconds);
  }

  getRest(audioCurrentIndex: number): IPlayListItem[] {
    const list = new Array<IPlayListItem>();
    this.items.forEach((item, index) => {
      if (index >= audioCurrentIndex) {
        list.push(item);
      }
    });

    return list;
  }

  getItems() {
    return this.items;
  }

  setCurrent(index: number) {
    this.items.forEach((item) => {
        item.isCurrent = false;
    });

    this.getByIndex(index).isCurrent = true;
  }

  getDurationInSeconds(): number {
    let res = 0;
    this.items.forEach((item) => {
      res += (item.speachDuration + item.pause);
    });

    return res;
  }
}
