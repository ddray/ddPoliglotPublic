import { IPlayListItem, PlayListItemTypes, IPlailable
  , IPlailablePare, PlayList, PlayListActivityTypes } from '../interfaces/IPlailable';
import { IArticlePhrase } from '../interfaces/IArticlePhrase';
import { Utils, Guid } from '../models/System';
import { IArticleByParamData } from '../interfaces/IArticleByParam';
import { IMixParams } from '../interfaces/IMixItem';
import { IWord } from '../interfaces/IWord';
import { environment } from 'src/environments/environment';

export class PlayListUtils {
  private static phrasesAudioUrl: string = environment.phrasesAudioUrl;

  static makePlayLists(data: IWord[], articleByParamData: IArticleByParamData): PlayList[] {
    // prepare audio data in source data.
    // all current data is in
    // data: words and phrases
    // articleByParamData: templates to generate articles

    // add audio objects to each word and phrase
    this.addPlayListItemsToWordsAndPhrasies(data);

    // add audio objects to each dictor phrase
    this.addPlayListItemsToDictorPhrasies(articleByParamData);

    // prepare list of words and phrases to add for each lesson
    const playListDialogOrWordPhrases = this.getDialogOrWordPhrases(data);

    // generate playlists (articles - trenings)
    const playLists = new Array<PlayList>();

    // uncomment to test video styles
    playLists.push(this.testPlayList());

    articleByParamData.mixParamsList.forEach((mixParam: IMixParams, index) => {
      // add first dictor phrases (article spicific)
      const playListFirstDictorPhrases = new Array<IPlayListItem>();

      if (mixParam.firstDictorPhrasesWithAudio.length > 0) {
        var phrases = mixParam.firstDictorPhrasesWithAudio[0];
        if (mixParam.firstDictorPhrasesWithAudio.length > 1) {
          var ind =
              Utils.getRandomInt(0, mixParam.firstDictorPhrasesWithAudio.length - 1);
          phrases = mixParam.firstDictorPhrasesWithAudio[ind];
        }

        phrases.forEach((phrase) => {
          if (phrase.playListItem != null) {
            if (phrase?.playListItem) {
              playListFirstDictorPhrases.push({...phrase.playListItem, activityType: PlayListActivityTypes.textFirst});
            }
          }
        });
      }

      // generate playlist based on this mixParam (article)
      const playListByOrderRnd = this.mixPhrasesByOrderRndMethod(data, mixParam);

      // repeat rnd base words only
      const playListRndBaseWords = this.mixPhrasesRndMethod(data, mixParam.active, mixParam.repeatBaseWord, false, false,
        mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudio, mixParam.insideBaseWordsDirMixDictorPhrasesWithAudio);

      // repeat rnd base words only tr first
      const playListRndBaseWordsTrFirst = this.mixPhrasesRndMethod(data, mixParam.trActive, mixParam.trRepeatBaseWord, false, true,
        mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudio, mixParam.insideBaseWordsRevMixDictorPhrasesWithAudio);

      // repeat rnd all
      const playListRndAll = this.mixPhrasesRndMethod(data, mixParam.active, mixParam.repeat, true, false,
        mixParam.beforeAllDirMixDictorPhrasesWithAudio, mixParam.insideAllDirMixDictorPhrasesWithAudio);

      // repeat rnd base words only tr first
      const playListRndAllTrFirst = this.mixPhrasesRndMethod(data, mixParam.trActive, mixParam.trRepeat, true, true,
        mixParam.beforeAllRevMixDictorPhrasesWithAudio, mixParam.insideAllRevMixDictorPhrasesWithAudio);

      const beginning = playListFirstDictorPhrases.concat(playListDialogOrWordPhrases);
      const resultList = mixParam.trFirst
        ? beginning.concat(playListByOrderRnd, playListRndBaseWordsTrFirst,
          playListRndBaseWords, playListRndAllTrFirst, playListRndAll)
        : beginning.concat(playListByOrderRnd, playListRndBaseWords,
          playListRndBaseWordsTrFirst, playListRndAll, playListRndAllTrFirst);

      playLists.push(new PlayList(Guid.newGuid().ToString(), `playlist: ${index}`, true, false, resultList));
    });

    return playLists;
  }

  private static getDialogOrWordPhrases(data: IWord[]): IPlayListItem[] {
    const list = new Array<IPlayListItem>();
    data.forEach((word: IWord) => {
      list.push({...word.playListItem, pause: 1, activityType: PlayListActivityTypes.textFirst,
        } as IPlayListItem);

      word.wordPhraseSelected.forEach((wordPhrase) => {
        list.push({...wordPhrase.playListItem, pause: 1, activityType: PlayListActivityTypes.textFirst,
          } as IPlayListItem);
        });
    });

    return list;
  }

  private static mixPhrasesRndMethod(
    data: IWord[],
    active: boolean,
    repeat: number,
    includePhrases: boolean,
    trFirst: boolean,
    before: IArticlePhrase[][],
    inside: IArticlePhrase[][]
    ): Array<IPlayListItem> {

    let resultList: Array<IPlayListItem> = [];
    const list: Array<IPlailablePare> = [];
    let rndList: Array<number> = [];

    if (active) {
      // prepare plain list of phrases
      const phPars = new Array<IPlailablePare>();
      data.forEach((word: IWord) => {
        phPars.push({source: word, translation: word.wordTranslation} as IPlailablePare);
        if (includePhrases) {
          word.wordPhraseSelected.forEach((wordPhrase) => {
            phPars.push({source: wordPhrase, translation: wordPhrase.wordPhraseTranslation} as IPlailablePare);
          });
        }
      });

      for (let r = 0; r < repeat; r++) {
        for (let i = 0; i < phPars.length; i++) {
            list.push(phPars[i]);
            rndList.push((r * phPars.length) + i);
        }
      }

      const cnt = (phPars.length * repeat) - 1;
      for (let r = 0; r < (phPars.length * repeat); r++) {
        const rndNum = Utils.getRandomInt(0, cnt - r);
        const ind = rndList[rndNum];
        if (trFirst) {
          resultList.push({...list[ind].translation.playListItem, activityType: PlayListActivityTypes.translationFirst});
          resultList.push({...list[ind].source.playListItem, activityType: PlayListActivityTypes.translationFirst});
        } else {
          resultList.push({...list[ind].source.playListItem, activityType: PlayListActivityTypes.textFirst});
          resultList.push({...list[ind].translation.playListItem, activityType: PlayListActivityTypes.textFirst});
        }

        rndList = rndList.filter((x) => x !== ind);
      }

      resultList = this.addDictorPhrases(resultList, before, inside);
    }

    return resultList;
  }

  private static mixPhrasesByOrderRndMethod(data: IWord[], mixParam: IMixParams): IPlayListItem[] {
    let resultList = new Array<IPlayListItem>();
    if (mixParam.active) {
      // add words and phrases to list
      data.forEach((word: IWord) => {
        // add word
        this.fillByOrder(word, word.wordTranslation, mixParam, resultList);

        // add word's phrases
        word.wordPhraseSelected.forEach((wordPhrase) => {
          this.fillByOrder(wordPhrase, wordPhrase.wordPhraseTranslation, mixParam, resultList);
        });
      });

      if (resultList?.length > 0) {
        resultList = this.addDictorPhrases(resultList,
          mixParam.beforeByOrderMixDictorPhrasesWithAudio, mixParam.insideByOrderMixDictorPhrasesWithAudio);
      }
    }

    return resultList;
  }

  private static addDictorPhrases(list: IPlayListItem[], before: IArticlePhrase[][], inside: IArticlePhrase[][]): IPlayListItem[] {
    const resultList = new Array<IPlayListItem>();

    if (list.length === 0) {
      return new Array<IPlayListItem>();
    }

    if (before.length > 0) {
      var phrases = before[0];
      if (before.length > 1) {
        var ind = Utils.getRandomInt(0, before.length - 1);
        phrases = before[ind];
      }

      phrases.forEach((phrase) => {
        if (phrase.playListItem != null) {
          if (phrase?.playListItem) {
            resultList.push({...phrase.playListItem, activityType: PlayListActivityTypes.textFirst});
          }
        }
      });
    }

    // inside
    const needInsert = Math.floor(inside.length > 0
      ? list.length / (inside.length + 1)
      : list.length * 2) - 1;

    let needInsertCnt = needInsert;
    let currIndexToInsert = 0;
    list.forEach(item => {
      if (needInsertCnt < 0
        && (
          (item.activityType === PlayListActivityTypes.textFirst
            && (item.type === PlayListItemTypes.word || item.type === PlayListItemTypes.wordPhrase)
          )
          ||
          (item.activityType === PlayListActivityTypes.translationFirst
            && (item.type === PlayListItemTypes.wordTranslation || item.type === PlayListItemTypes.wordPhraseTranslation)
          )
        )
      ) {
        // only before this type of items
        needInsertCnt = needInsert;
        inside[currIndexToInsert]
        .forEach((phrase) => {
          if (phrase?.playListItem) {
            resultList.push({...phrase.playListItem, activityType: PlayListActivityTypes.textFirst});
          }
        });
        currIndexToInsert++;
        if (currIndexToInsert >= inside.length) {
          currIndexToInsert = 0;
        }
      } else {
        needInsertCnt--;
      }

      resultList.push({...item});
    });

    return resultList;
  }

  private static addPlayListItemsToDictorPhrasies(articleByParamData: IArticleByParamData) {
    // go throw all params and make playListItems for each dictor phrase
    articleByParamData.mixParamsList.forEach((mixParam: IMixParams) => {
      this.addPlayListItemsToDictorPhrase(mixParam.firstDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.beforeByOrderMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.insideByOrderMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.insideBaseWordsDirMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.insideBaseWordsRevMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.beforeAllDirMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.insideAllDirMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.beforeAllRevMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.insideAllRevMixDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.beforeFinishDictorPhrasesWithAudio);
      this.addPlayListItemsToDictorPhrase(mixParam.finishDictorPhrasesWithAudio);
    });
  }

  private static addPlayListItemsToDictorPhrase(dictorPhrasesBlocks: IArticlePhrase[][]) {
    if (dictorPhrasesBlocks) {
      dictorPhrasesBlocks.forEach((phraseBlock) => {
        phraseBlock.forEach((phrase) => {
          if (phrase?.text && phrase.text.length > 2) {
            const newItem = {
              itemID: 0,
              itemParentID: 0,
              itemRef: null,
              itemParentRef: null,
              text: phrase.text,
              pronunciation: '',
              textSpeechFileName: phrase.textSpeechFileName,
              speachDuration: phrase.speachDuration,
              pause: 1,
              type: PlayListItemTypes.dictorPhrase,
            } as IPlayListItem;

            newItem.audio = new Audio();
            newItem.audio.src = `${this.phrasesAudioUrl}/${newItem.textSpeechFileName}`;
            newItem.audio.load();

            phrase.playListItem = newItem;
          }
        });
      });
    }
  }

  private static addChildDictorPhrasesToPlayList(
    dictorPhrases: IArticlePhrase[],
    list: IPlayListItem[],
  ) {
    dictorPhrases.forEach((phrase) => {
      if (phrase?.playListItem) {
        list.push(phrase.playListItem);
      }
    });
  }

  private static fillByOrder(item: IPlailable, itemTranslation: IPlailable, mixParams: IMixParams, list: Array<IPlayListItem>) {
    for (let index = 0; index < mixParams.repeatOrder; index++) {
      list.push({...item.playListItem, order: list.length, activityType: PlayListActivityTypes.textFirst});
      list.push({...itemTranslation.playListItem, order: list.length, activityType: PlayListActivityTypes.textFirst});

      if (mixParams.addSlow2InRepeatOrder) {
        list.push({...item.playListItemSpeed2, order: list.length, activityType: PlayListActivityTypes.textFirst});
      }

      if (mixParams.addSlowInRepeatOrder) {
        list.push({...item.playListItemSpeed1, order: list.length, activityType: PlayListActivityTypes.textFirst});
      }
    }
  }

  private static addPlayListItemsToWordsAndPhrasies(data: IWord[]) {
    const list = new Array<IPlayListItem>();
    data.forEach((word: IWord) => {
      word.playListItem = {
        itemID: word.wordID,
        itemParentID: 0,
        itemRef: word,
        itemParentRef: null,
        text: word.text,
        pronunciation: word.pronunciation,
        textSpeechFileName: word.textSpeechFileName,
        speachDuration: word.speachDuration,
        pause: word.speachDuration,
        type: PlayListItemTypes.word,
      } as IPlayListItem;
      list.push(word.playListItem);

      word.playListItemSpeed1 = {
        itemID: word.wordID,
        itemParentID: 0,
        itemRef: word,
        itemParentRef: null,
        text: word.text,
        pronunciation: word.pronunciation,
        textSpeechFileName: word.textSpeechFileNameSpeed1,
        speachDuration: word.speachDurationSpeed1,
        pause: word.speachDurationSpeed1,
        type: PlayListItemTypes.wordSpeed1,
      } as IPlayListItem;
      list.push(word.playListItemSpeed1);

      word.playListItemSpeed2 = {
        itemID: word.wordID,
        itemParentID: 0,
        itemRef: word,
        itemParentRef: null,
        text: word.text,
        pronunciation: word.pronunciation,
        textSpeechFileName: word.textSpeechFileNameSpeed2,
        speachDuration: word.speachDurationSpeed2,
        pause: word.speachDurationSpeed2,
        type: PlayListItemTypes.wordSpeed2,
      } as IPlayListItem;
      list.push(word.playListItemSpeed2);

      word.wordTranslation.playListItem = {
        itemID: word.wordTranslation.wordTranslationID,
        itemParentID: word.wordID,
        itemRef: word.wordTranslation,
        itemParentRef: word,
        text: word.wordTranslation.text,
        pronunciation: '',
        textSpeechFileName: word.wordTranslation.textSpeechFileName,
        speachDuration: word.wordTranslation.speachDuration,
        pause: word.wordTranslation.speachDuration,
        type: PlayListItemTypes.wordTranslation,
      } as IPlayListItem;
      list.push(word.wordTranslation.playListItem);

      word.wordPhraseSelected.forEach((wordPhrase) => {
        wordPhrase.playListItem = {
          itemID: wordPhrase.wordPhraseID,
          itemParentID: word.wordID,
          itemRef: wordPhrase,
          itemParentRef: word,
          text: wordPhrase.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileName,
          speachDuration: wordPhrase.speachDuration,
          pause: wordPhrase.speachDuration,
          type: PlayListItemTypes.wordPhrase,
        } as IPlayListItem;
        list.push(wordPhrase.playListItem);

        // add wordPhrase text Speed1
        wordPhrase.playListItemSpeed1 = {
          itemID: wordPhrase.wordPhraseID,
          itemParentID: word.wordID,
          itemRef: wordPhrase,
          itemParentRef: word,
          text: wordPhrase.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileNameSpeed1,
          speachDuration: wordPhrase.speachDurationSpeed1,
          pause: wordPhrase.speachDurationSpeed1,
          type: PlayListItemTypes.wordPhraseSpeed1,
        } as IPlayListItem;
        list.push(wordPhrase.playListItemSpeed1);

        wordPhrase.playListItemSpeed2 = {
          itemID: wordPhrase.wordPhraseID,
          itemParentID: word.wordID,
          itemRef: wordPhrase,
          itemParentRef: word,
          text: wordPhrase.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.textSpeechFileNameSpeed2,
          speachDuration: wordPhrase.speachDurationSpeed2,
          pause: wordPhrase.speachDurationSpeed2,
          type: PlayListItemTypes.wordPhraseSpeed2,
        } as IPlayListItem;
        list.push(wordPhrase.playListItemSpeed2);

        wordPhrase.wordPhraseTranslation.playListItem = {
          itemID: wordPhrase.wordPhraseTranslation.wordPhraseTranslationID,
          itemParentID: wordPhrase.wordPhraseID,
          itemRef: wordPhrase.wordPhraseTranslation,
          itemParentRef: wordPhrase,
          text: wordPhrase.wordPhraseTranslation.text,
          pronunciation: '',
          textSpeechFileName: wordPhrase.wordPhraseTranslation.textSpeechFileName,
          speachDuration: wordPhrase.wordPhraseTranslation.speachDuration,
          pause: wordPhrase.wordPhraseTranslation.speachDuration,
          type: PlayListItemTypes.wordPhraseTranslation,
        } as IPlayListItem;

        list.push(wordPhrase.wordPhraseTranslation.playListItem);
      });
    });

    // add audio objects
    list.forEach((item) => {
      item.audio = new Audio();
      item.audio.src = `${this.phrasesAudioUrl}/${item.textSpeechFileName}`;
      item.audio.load();
    });
  }

  private static testPlayList(): PlayList {

    const list = new Array<IPlayListItem>();

    // TextYesPronYesTranslNo_TextFirst = 0,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: {wordTranslation: {text: 'translation 0'}},
      itemParentRef: null,
      text: 'word 0',
      pronunciation: 'pron 0',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.word,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // TextYesPronYesTranslYes_TextFirst = 2,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: {text: 'word 2', pronunciation: 'pron 2'},
      text: 'translation 2',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordTranslation,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // TextNoPronNoTranslYes_TranslFirst = 3,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: {text: 'word 3', pronunciation: 'pron 3'},
      text: 'translation 3',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordTranslation,
      activityType: PlayListActivityTypes.translationFirst
    } as IPlayListItem);

    // TextYesPronYesTranslYes_TranslFirst = 1,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: {wordTranslation: {text: 'translation 1'}},
      itemParentRef: null,
      text: 'word 1',
      pronunciation: 'pron 1',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.word,
      activityType: PlayListActivityTypes.translationFirst
    } as IPlayListItem);

    // WORD PHRASE

    // TextYesTranslNo_TextFirst = 4,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: {wordPhraseTranslation: {text: 'phrase translation4'}},
      itemParentRef: null,
      text: 'phrase text 4',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhrase,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // TextYesTranslYes_TextFirst = 6,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: {text: 'phrase text 6'},
      text: 'Phrase Translation text 6',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhraseTranslation,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // TextYesTranslYes_TextFirst = 6,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: {text: 'phrase text 6 HHHHHH HHHHH HHHHHH HHHH HH HHHH HHHHHHHHHH HHHH HHHH HH HHHHHHHH HHHHHH HHHHHHHH HHH'},
      text: 'Phrase Translation text 6  HHHHHH HHHHH HHH HHHHHHH HH HHHH HHHHHHH HHHH HHH HHHH HH HHHHHH HH HHHH',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhraseTranslation,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // TextYesTranslYes_TextFirst = 6,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: {text: 'phrase text 6 (49) HHHHHH HHHHH HHHH HHHHHH HHHH'},
      text: 'Phrase Translation text 6 (49) HHHHHH HHHHH HHHHH',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhraseTranslation,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // TextYesTranslYes_TranslFirst = 5,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: {wordPhraseTranslation: {text: 'phrase translation 5 short (49) HHHHHH HHHHH HHHH'}},
      itemParentRef: null,
      text: 'phrase text 5 short (49) HHHHHH HHHHH HHH HHHHHH',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhrase,
      activityType: PlayListActivityTypes.translationFirst
    } as IPlayListItem);

    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: {wordPhraseTranslation: {text: 'phrase translation 5 middle (99) HHHHHH HHHHH HHH HHHHHHH HH HHHH HHHHHHH HHHH HHHHHHH HHHHHHH HHHH'}},
      itemParentRef: null,
      text: 'phrase text 5 middle (99) HHHHHH HHHHH HHH HHHHHHH HH HHHH HHHHHHH HHHH HHHHHHH HHHHHHH HHHH',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhrase,
      activityType: PlayListActivityTypes.translationFirst
    } as IPlayListItem);

    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: {wordPhraseTranslation: {text: 'phrase translation 5 long (162) HHHHHH HHHHH HHH HHHHHHH HH HHHH HHHHHHH HHHH HHH HHHH HH HHHHHH HH HHHH dfdfdfg fgbgdffgHH HHHHHH HH HHHH dfdfdfg fgbgdffg'}},
      itemParentRef: null,
      text: 'phrase text 5 long (162) HHHHHH HHHHH HHH HHHHHHH HH HHHH HHHHHHH HHHH HHH HHHH HH HHHHHH HH HHHH dfdfdfg fgbgdffg HH HHHHHH HH HHHH dfdfdfg fgbgdffg',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhrase,
      activityType: PlayListActivityTypes.translationFirst
    } as IPlayListItem);

    // TextNoTranslYes_TranslFirst = 7,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: {wordPhraseTranslation: {text: 'phrase translation 7'}},
      itemParentRef: null,
      text: 'phrase translation 7',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.wordPhraseTranslation,
      activityType: PlayListActivityTypes.translationFirst
    } as IPlayListItem);

    // Dictor_TextFirst = 8,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: null,
      text: 'dictor phrase 8 short (49) HHHHHH HHHHH HHHHHHHHH',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.dictorPhrase,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // Dictor_TextFirst = 8,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: null,
      text: 'dictor phrase text 8 middle (99) HHHHHH HHHHH HHHHH ssdd HHHHHH HHHHH HHHHH ssdd HHHHHH HHHHH HHHHH',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.dictorPhrase,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    // Dictor_TextFirst = 8,
    list.push({
      itemID: 0,
      itemParentID: 0,
      itemRef: null,
      itemParentRef: null,
      text: 'dictor Phrase text 8 long (162) HHHHHH HHHHH HHH HHHHHHH HH HHHH HHHHHHH HHHH HHH HHHH HH HHHHHH HH HHHH dfdfdfg fgbgdffg fgfhfhhgHH HHHH dfdfdfg fgbgdffg fgfhfhhg',
      pronunciation: '',
      textSpeechFileName: '',
      speachDuration: 2,
      pause: 2,
      type: PlayListItemTypes.dictorPhrase,
      activityType: PlayListActivityTypes.textFirst
    } as IPlayListItem);

    list.reverse();

    const playList = new PlayList(Guid.newGuid().ToString(), 'test', true, true, list);
    return playList;
  }
}
