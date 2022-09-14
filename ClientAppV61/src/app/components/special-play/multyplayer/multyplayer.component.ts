import { Component, OnInit } from '@angular/core';
import { PlayList, AudioSPlayerState, PlayListActivityTypes, IPlayListItem,
  CurrentScreenTypes, PlayListItemTypes, ScreenTextTypes, ScreenTextWithClass,
  CurrentScreen, ScreenClass, TextLen
} from 'src/app/shared/interfaces/IPlailable';
import { IWord } from 'src/app/shared/interfaces/IWord';
import { IWordPhrase } from 'src/app/shared/interfaces/IWordPhrase';
import { MatSliderChange } from '@angular/material/slider';

@Component({
  selector: 'app-multyplayer',
  templateUrl: './multyplayer.component.html',
  styleUrls: ['./multyplayer.component.scss']
})
export class MultyplayerComponent implements OnInit {
  public audioSPlayer = new AudioSPlayer();
  public currentScreen: CurrentScreen;
  public buttonPlayDisabled = false;
  public buttonStopDisabled = false;
  public buttonSkipNextDisabled = false;
  public buttonSkipPrevDisabled = false;
  public PlayListItemType = PlayListItemTypes;
  public PlayListActivityType = PlayListActivityTypes;
  public CurrentScreenType = CurrentScreenTypes;
  public playLists = new Array<PlayList>();
  public displayedColumns3: string[] = ['text', 'type', 'activity', 'filename', 'pause', 'act'];
  public audioSPlayerState = AudioSPlayerState;
  public isLoadingResults = false;

  constructor(
    ) { }

  ngOnInit(): void {
    this.audioSPlayer.onDrawText = (
      сurrentScreenType: CurrentScreenTypes,
      activityType: PlayListActivityTypes,
      text: string,
      pron: string,
      transl: string,
      ) => {
      return new Promise((resolve, reject) => {
        this.currentScreen = {
          сurrentScreenType,
          activityType,
          text: {
            value: text,
            screenClass: this.calcScreenClassByType(сurrentScreenType, ScreenTextTypes.Text, text)
          } as ScreenTextWithClass,
          pron: {
            value: pron,
            screenClass: this.calcScreenClassByType(сurrentScreenType, ScreenTextTypes.Pron, pron)
          } as ScreenTextWithClass,
          transl: {
            value: transl,
            screenClass: this.calcScreenClassByType(сurrentScreenType, ScreenTextTypes.Transl, transl)
          } as ScreenTextWithClass,
        } as CurrentScreen;

        resolve();
      });
    };

    this.audioSPlayer.onPlayListFinished = async () => {
      const currIndex =  this.playLists.findIndex(x => x.isCurrent);
      if (currIndex < (this.playLists.length - 1)) {
        await this.selectPlayList(currIndex + 1);
        await this.play();
      } else {
        await this.stop();
      }
    };
  }

  async setPlayLists(newPlayLists: PlayList[]) {
    await this.audioSPlayer.audioStop();
    this.playLists = newPlayLists;
    if (newPlayLists && newPlayLists.length > 0) {
      await this.selectPlayList(0);
    }
  }

  async play() {
    await this.audioSPlayer.audioPlay();
  }

  async stop() {
    await this.audioSPlayer.audioStop();
  }

  async skipToFirst(audioCurrentState: AudioSPlayerState) {
    await this.audioSPlayer.goToFirst(audioCurrentState);
  }

  async skipToLast(audioCurrentState: AudioSPlayerState) {
    await this.audioSPlayer.goToLast(audioCurrentState);
  }

  async skipToPrevious(audioCurrentState: AudioSPlayerState) {
    await this.audioSPlayer.setCurrentAudioShift(-1, audioCurrentState);
  }

  async skipToNext(audioCurrentState: AudioSPlayerState) {
    await this.audioSPlayer.setCurrentAudioShift(1, audioCurrentState);
  }

  async setCurrent(playListItem: IPlayListItem) {
    await this.audioSPlayer.setCurrent(playListItem);
  }

  async selectPlayList(listNumber: number) {
    await this.audioSPlayer.audioStop();
    this.playLists.forEach((pl , index) => {
      pl.isCurrent = (index === listNumber);
    });

    this.audioSPlayer.setPlayList(this.playLists[listNumber]);
  }

  calcScreenClassByType(currentScreenTypes: CurrentScreenTypes, screenTextTypes: ScreenTextTypes, text: string): ScreenClass {
    const result = {
      level0: ['video-text', `${CurrentScreenTypes[currentScreenTypes]}_${ScreenTextTypes[screenTextTypes]}_L0`],
      level1: ['video-text', `${CurrentScreenTypes[currentScreenTypes]}_${ScreenTextTypes[screenTextTypes]}_L1`],
      level2: ['video-text', this.calcTextClass(currentScreenTypes, screenTextTypes, text)]
    } as ScreenClass;

    return result;
  }

  calcTextClass(currentScreenTypes: CurrentScreenTypes, screenTextTypes: ScreenTextTypes, text: string): string {
    let result = '';
    if (currentScreenTypes === CurrentScreenTypes.TextNoPronNoTranslYes_TranslFirst
      || currentScreenTypes === CurrentScreenTypes.TextYesPronYesTranslNo_TextFirst
      || currentScreenTypes === CurrentScreenTypes.TextYesPronYesTranslYes_TextFirst
      || currentScreenTypes === CurrentScreenTypes.TextYesPronYesTranslYes_TranslFirst
      ) {
        result = `video-word-${ScreenTextTypes[screenTextTypes]}`;
    } else if (currentScreenTypes === CurrentScreenTypes.TextNoTranslYes_TranslFirst
      || currentScreenTypes === CurrentScreenTypes.TextYesTranslNo_TextFirst
      || currentScreenTypes === CurrentScreenTypes.TextYesTranslYes_TextFirst
      || currentScreenTypes === CurrentScreenTypes.TextYesTranslYes_TranslFirst
     ) {
      result = `video-phrase-${ScreenTextTypes[screenTextTypes]}-${TextLen[this.getTextSize(text.length, screenTextTypes)]}`;
    } else if (currentScreenTypes === CurrentScreenTypes.Dictor_TextFirst) {
      result = `video-dictor-${TextLen[this.getTextSize(text.length, screenTextTypes)]}`;
    }

    return result;
  }

  getTextSize(length: number, screenTextTypes: ScreenTextTypes): TextLen {
    if (screenTextTypes === ScreenTextTypes.Text) {
      if (length < 50) {
        return TextLen.Short;
      } else if (length < 100) {
        return TextLen.Middle;
      } else {
        return TextLen.Long;
      }
    } else {
      // transl
      if (length < 50) {
        return TextLen.Short;
      } else if (length < 100) {
        return TextLen.Middle;
      } else {
        return TextLen.Long;
      }
    }
  }
}


class AudioSPlayer {
  private playList: PlayList;
  private audioCurrentIndex = 0;
  private audioCurrentState: AudioSPlayerState = AudioSPlayerState.stoped;
  private audioCurrentTimeOut: any;
  private audioCurrentTimeOnCanceled: () => void;
  private audioCurrent: any;
  public audioPlayListCurrentState: AudioSPlayerState = AudioSPlayerState.stoped;
  public onDrawText: (type: number, activityType: PlayListActivityTypes, text: string, pron: string, transl: string) => Promise<void>;
  public onPlayListFinished: () => Promise<void>;
  public currentPositionInSeconds = 0;

  constructor() { }

   getState(): AudioSPlayerState {
    return this.audioCurrentState;
  }

  getPlayList(): PlayList {
    return this.playList;
  }

  async setPlayList(playList: PlayList) {
    await this.audioStop();
    this.playList = playList;
    this.playList.recalculate();
    this.playList.setCurrent(0);
    this.audioCurrentIndex = 0;
    this.currentPositionInSeconds = 0;
    await this.drawText(playList.getItems()[0]);
  }

  async audioStop() {
    this.audioCurrentState = AudioSPlayerState.stoped;
    if (this.audioCurrent) {
      // console.log('audioStop request audio pause:', this.audioCurrentIndex);
      (this.audioCurrent as HTMLAudioElement).pause();
    }

    if (this.audioCurrentTimeOut) {
      // console.log('audioStop request pause pause:', this.audioCurrentIndex);
      clearTimeout(this.audioCurrentTimeOut);
      if (this.audioCurrentTimeOnCanceled) {
        this.audioCurrentTimeOnCanceled();
      }

      this.audioCurrentTimeOut = null;
    }

    return new Promise((resolve, reject) => {
      if (this.audioPlayListCurrentState === AudioSPlayerState.stoped) {
        resolve(null);
      } else {
        setTimeout(() => {
          if (this.audioPlayListCurrentState === AudioSPlayerState.stoped) {
            resolve(null);
          } else {
            setTimeout(() => {
              if (this.audioPlayListCurrentState === AudioSPlayerState.stoped) {
                resolve(null);
              } else {
                setTimeout(() => {
                  if (this.audioPlayListCurrentState === AudioSPlayerState.stoped) {
                    resolve(null);
                  }
                } , 100);
              }
            } , 100);
          }
        } , 100);
      }
    });
  }

  async audioPlay() {
    this.audioPlayListCurrentState = AudioSPlayerState.plaing;
    // console.log('audioPlay start at:', this.audioCurrentIndex);

    // copy items for play to list
    const list = this.playList.getRest(this.audioCurrentIndex);

    this.audioCurrentState = AudioSPlayerState.plaing;
    let cnt = 0;
    for await (const playListItem of list) {
      if (this.audioCurrentState !== AudioSPlayerState.plaing) {
        break;
      } else {
        await this.drawText(playListItem);
      }

      if (this.audioCurrentState !== AudioSPlayerState.plaing) {
        break;
      } else {
        await this.playFile(playListItem);
      }

      if (this.audioCurrentState !== AudioSPlayerState.plaing) {
        break;
      } else {
        await this.playPause(playListItem);
      }

      if (this.audioCurrentState !== AudioSPlayerState.plaing) {
        break;
      } else {
        await this.audioCurrentIndexStep(1);
      }

      cnt++;
    }

    this.audioPlayListCurrentState = AudioSPlayerState.stoped;

    if (list.length === cnt && this.audioCurrentState === AudioSPlayerState.plaing) {
      // this playlist is finished, go to next playlist
      await this.onPlayListFinished();
    }
  }

  async audioCurrentIndexStep(step: number) {
    return new Promise((resolve, reject) => {
      this.audioCurrentIndex += step;
      if (this.audioCurrentIndex >= this.playList.length()) {
        this.audioCurrentIndex = this.playList.length() - 1;
      }

      if (this.audioCurrentIndex < 0) {
        this.audioCurrentIndex = 0;
      }

      this.currentPositionInSeconds = this.playList.getByIndex(this.audioCurrentIndex).fromSecond;
      this.playList.setCurrent(this.audioCurrentIndex);
      resolve(null);
    });
  }

  async setCurrentAudioShift(shift: number, audioCurrentState: AudioSPlayerState) {
    if (audioCurrentState === AudioSPlayerState.plaing) {
      await this.audioStop();
      await this.audioCurrentIndexStep(shift);
      await this.audioPlay();
    } else {
      await this.audioCurrentIndexStep(shift);
      await this.drawText(this.playList.getByIndex(this.audioCurrentIndex));
    }
  }

  async goToLast(audioCurrentState: AudioSPlayerState) {
    await this.setCurrentAudioShift(this.playList.length() - this.audioCurrentIndex, this.audioCurrentState);
  }

  async goToFirst(audioCurrentState: AudioSPlayerState) {
    await this.setCurrentAudioShift(this.audioCurrentIndex * (-1), this.audioCurrentState);
  }

  async setCurrent(playListItem: IPlayListItem) {
    await this.setCurrentAudioShift(playListItem.order - this.audioCurrentIndex, this.audioCurrentState);
  }

  playFile(playListItem: IPlayListItem) {
    return new Promise((resolve, reject) => {
      if (this.audioCurrentState === AudioSPlayerState.plaing) {
        playListItem.audio.onended = null;
        playListItem.audio.onended = () => {

          // console.log('playFile finish at:', this.audioCurrentIndex, playListItem.textSpeechFileName);
          resolve(null);
        };

        playListItem.audio.onpause = null;
        playListItem.audio.onpause = () => {
          // console.log('playFile finish at (pause):', this.audioCurrentIndex, playListItem.textSpeechFileName);
          resolve(null);
        };

        this.audioCurrent = playListItem.audio;
        // console.log('playFile start at:', this.audioCurrentIndex, playListItem.textSpeechFileName);
        playListItem.audio.currentTime = 0;
        playListItem.audio.play();
      } else {
        resolve(null);
      }
    });
  }

  async playPause(playListItem: IPlayListItem) {
    return new Promise((resolve, reject) => {
      // console.log('playPause start at:', this.audioCurrentIndex, playListItem.textSpeechFileName);
      this.audioCurrentTimeOnCanceled = null;
      this.audioCurrentTimeOnCanceled = () => {
        this.audioCurrentTimeOnCanceled = null;
        resolve(null);
      };

      this.audioCurrentTimeOut = setTimeout(() => {
        // console.log('playPause finish at:', this.audioCurrentIndex, playListItem.textSpeechFileName);
        this.audioCurrentTimeOnCanceled = null;
        resolve(null);
      } , playListItem.pause * 1000);
    });
  }

  async drawText(playListItem: IPlayListItem) {
    let videoCurrentText = '';
    let videoCurrentPronText = '';
    let videoCurrentTranslationText = '';
    let caseType: CurrentScreenTypes = 0;

    if (playListItem.type === PlayListItemTypes.word
      || playListItem.type === PlayListItemTypes.wordSpeed1
      || playListItem.type === PlayListItemTypes.wordSpeed2
      ) {
      if (playListItem.activityType === PlayListActivityTypes.textFirst) {
        // TextYesPronYesTranslNo_TextFirst = 0,
        caseType = CurrentScreenTypes.TextYesPronYesTranslNo_TextFirst;
        videoCurrentText = playListItem.text;
        videoCurrentPronText = playListItem.pronunciation;
        videoCurrentTranslationText = '';
      } else {
        // TextYesPronYesTranslYes_TranslFirst = 1,
        caseType = CurrentScreenTypes.TextYesPronYesTranslYes_TranslFirst;
        videoCurrentText = playListItem.text;
        videoCurrentPronText = playListItem.pronunciation;
        videoCurrentTranslationText = (playListItem.itemRef as IWord).wordTranslation.text;
      }
    } else if (playListItem.type === PlayListItemTypes.wordTranslation) {
      if (playListItem.activityType === PlayListActivityTypes.textFirst) {
        // TextYesPronYesTranslYes_TextFirst = 2,
        caseType = CurrentScreenTypes.TextYesPronYesTranslYes_TextFirst;
        videoCurrentText = (playListItem.itemParentRef as IWord).text;
        videoCurrentPronText = (playListItem.itemParentRef as IWord).pronunciation;
        videoCurrentTranslationText = playListItem.text;
      } else {
        // TextNoPronNoTranslYes_TranslFirst = 3,
        caseType = CurrentScreenTypes.TextNoPronNoTranslYes_TranslFirst;
        videoCurrentText = '';
        videoCurrentTranslationText = playListItem.text;
      }
    } else if (playListItem.type === PlayListItemTypes.wordPhrase
      || playListItem.type === PlayListItemTypes.wordPhraseSpeed1
      || playListItem.type === PlayListItemTypes.wordPhraseSpeed2
      ) {
      if (playListItem.activityType === PlayListActivityTypes.textFirst) {
        // TextYesTranslNo_TextFirst = 4,
        caseType = CurrentScreenTypes.TextYesTranslNo_TextFirst;
        videoCurrentText = playListItem.text;
        videoCurrentTranslationText = '';
      } else {
        // TextYesTranslYes_TranslFirst = 5,
        caseType = CurrentScreenTypes.TextYesTranslYes_TranslFirst;
        videoCurrentText = playListItem.text;
        videoCurrentTranslationText = (playListItem.itemRef as IWordPhrase).wordPhraseTranslation.text;
      }
    } else if (playListItem.type === PlayListItemTypes.wordPhraseTranslation) {
      if (playListItem.activityType === PlayListActivityTypes.textFirst) {
        // TextYesTranslYes_TextFirst = 6,
        caseType = CurrentScreenTypes.TextYesTranslYes_TextFirst;
        videoCurrentText = (playListItem.itemParentRef as IWordPhrase).text;
        videoCurrentTranslationText = playListItem.text;
      } else {
        // TextNoTranslYes_TranslFirst = 7,
        caseType = CurrentScreenTypes.TextNoTranslYes_TranslFirst;
        videoCurrentText = '';
        videoCurrentTranslationText = playListItem.text;
      }
    } else if (playListItem.type === PlayListItemTypes.dictorPhrase) {
      // Dictor_TextFirst = 8,
      caseType = CurrentScreenTypes.Dictor_TextFirst;
      videoCurrentText = playListItem.text;
      videoCurrentTranslationText = '!!';
    }

    await this.onDrawText(
      caseType,
      playListItem.activityType,
      videoCurrentText,
      videoCurrentPronText,
      videoCurrentTranslationText
    );
  }

  getDurationInSeconds() {
    if (this.playList) {
      return this.playList.getDurationInSeconds();
    } else {
      return 0;
    }
  }

  async sliderChanged(event: MatSliderChange) {
    const item = this.playList.getBySeconds(event.value);
    // console.log('sliderChanged', event.value);
    // console.log('sliderChanged item', item);
    await this.setCurrent(item);
  }

  sliderInput(event: any) {
    console.log('sliderInput:', event);
  }
}
