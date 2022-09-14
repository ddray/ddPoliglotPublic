import { IArticleActor } from './IArticleActor';
import { IPlayListItem } from './IPlailable';

export interface IArticlePhrase {
  articlePhraseID: number;
  articleID: number;
  orderNum: number;
  keyGuid: string;
  activityType: number; // 0 - text first; 1 - trText first
  type: number; // 0 - text speach; 2 - Dictor

  text: string;
  language: string;
  textSpeechFileName: string;
  hashCode: number;
  speachDuration: number;
  articleActorID: number;
  articleActor: IArticleActor;
  pause: number; // how many seconds pause before

  trText: string;
  trLanguage: string;
  trTextSpeechFileName: string;
  trHashCode: number;
  trSpeachDuration: number;
  trArticleActorID: number;
  trArticleActor: IArticleActor;
  trPause: number; // how many seconds pause before

  // grouping
  parentKeyGuid: string;
  hasChildren: boolean;
  childrenType: string; // 0 01 012 0123; if is empty - added by hand

  childrenClosed: boolean; // show or not children if hasChildren = true
  hidden: boolean; // show or not if it is children parentKeyGuid != ''
  selected: boolean;

  showActor: boolean;
  showTrActor: boolean;
  silent: boolean; // dont use for video and audio
  playListItem?: IPlayListItem; // for sPlayer
}
