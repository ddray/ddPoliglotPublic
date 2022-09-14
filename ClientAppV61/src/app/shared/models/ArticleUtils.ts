import { IMixItem, IMixParams } from '../interfaces/IMixItem';
import { IPrepareMixItem } from '../interfaces/IPrepareMixItem';
import { IArticlePhrase } from '../interfaces/IArticlePhrase';
import { IArticleActor } from '../interfaces/IArticleActor';
import { IArticle } from '../interfaces/IArticle';
import { ArticlePhraseType, ArticlePhraseActivityType } from '../enums/Enums';
import { Guid, Utils } from './System';
import { IPhraseHiddenProperty, ITextWithPhraseHiddenProperty } from '../interfaces/IPhraseHiddenProperty';

export class ArticleUtils {
  static mixAndAddToArticle(
    mixParams: IMixParams,
    article: IArticle,
    articlePhrase: IArticlePhrase,
    textActors: IArticleActor[],
    textDictorActors: IArticleActor[],
    translatedActors: IArticleActor[],
    translatedDictorActors: IArticleActor[],
    wordsToRepeatMixItems: IMixItem[] = [],
    wordPhrasesToRepeatMixItems: IMixItem[] = []
  ): IMixItem[] {

    const index = article.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    const resultArr = mixParams.mixItems.filter((x) => x.source.length > 0);

    // repeat each item by order
    const newItemsByOrder: Array<IPrepareMixItem>
      = this.mixPhrasesByOrderRndMethod(resultArr, mixParams.active, mixParams.repeatOrder,
        mixParams.addSlow2InRepeatOrder, mixParams.addSlowInRepeatOrder);

    const trNewItemsByOrder: Array<IPrepareMixItem>
      = this.mixPhrasesByOrderRndMethod(resultArr, mixParams.trActive, mixParams.trRepeatOrder, false, false);

    // repeat rnd base words only
    const newItemsBaseWords: Array<IPrepareMixItem>
      = this.mixPhrasesRndMethod(resultArr.filter(x => x.baseWord), mixParams.active, mixParams.repeatBaseWord, wordsToRepeatMixItems, []);

    const trNewItemsBaseWords: Array<IPrepareMixItem>
      = this.mixPhrasesRndMethod(resultArr.filter(x => x.baseWord), mixParams.trActive, mixParams.trRepeatBaseWord,
      wordsToRepeatMixItems, []);

    // repeat rnd all
    const newItems: Array<IPrepareMixItem>
      = this.mixPhrasesRndMethod(resultArr, mixParams.active, mixParams.repeat, wordsToRepeatMixItems, wordPhrasesToRepeatMixItems);

    const trNewItems: Array<IPrepareMixItem>
      = this.mixPhrasesRndMethod(resultArr, mixParams.trActive, mixParams.trRepeat, wordsToRepeatMixItems, wordPhrasesToRepeatMixItems);

    let insertIndex = index;
    if (mixParams.trFirst) {

      // By Order

      // this.addMixedToArticle(trNewItemsByOrder, insertIndex, 1,
      //  article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors);
      // insertIndex += trNewItemsByOrder.length;

      insertIndex += this.addMixedToArticle(newItemsByOrder, insertIndex, 0,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeByOrderMixDictorPhrases, mixParams.insideByOrderMixDictorPhrases, 0);

      // Base words mix
      insertIndex += this.addMixedToArticle(trNewItemsBaseWords, insertIndex, 0,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeBaseWordsRevMixDictorPhrases, mixParams.insideBaseWordsRevMixDictorPhrases, 1);

      insertIndex += this.addMixedToArticle(newItemsBaseWords, insertIndex, 1,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeBaseWordsDirMixDictorPhrases, mixParams.insideBaseWordsDirMixDictorPhrases, 1);

      // all mix
      insertIndex += this.addMixedToArticle(trNewItems, insertIndex, 1,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeAllRevMixDictorPhrases, mixParams.insideAllRevMixDictorPhrases, 2);

      insertIndex += this.addMixedToArticle(newItems, insertIndex, 0,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeAllDirMixDictorPhrases, mixParams.insideAllDirMixDictorPhrases, 2);
    } else {
      // Order By Mix
      insertIndex += this.addMixedToArticle(newItemsByOrder, insertIndex, 0,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeByOrderMixDictorPhrases, mixParams.insideByOrderMixDictorPhrases, 0);

      // Base words mix
      insertIndex += this.addMixedToArticle(newItemsBaseWords, insertIndex, 0,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeBaseWordsDirMixDictorPhrases, mixParams.insideBaseWordsDirMixDictorPhrases, 1);

      insertIndex += this.addMixedToArticle(trNewItemsBaseWords, insertIndex, 1,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeBaseWordsRevMixDictorPhrases, mixParams.insideBaseWordsRevMixDictorPhrases, 1);

      // all mix
      insertIndex += this.addMixedToArticle(newItems, insertIndex, 0,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeAllDirMixDictorPhrases, mixParams.insideAllDirMixDictorPhrases, 2);

      insertIndex += this.addMixedToArticle(trNewItems, insertIndex, 1,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors,
        mixParams.beforeAllRevMixDictorPhrases, mixParams.insideAllRevMixDictorPhrases, 2);
    }

    return resultArr;
  }

  static recalculateArticlePhrasesOrder(article: IArticle) {
    article.articlePhrases.forEach((x, i) => { x.orderNum = i; });
  }


  static addChildDictorPhrasesToArticle(
    dictorPhrasesText,
    index,
    activityType,
    article: IArticle,
    articlePhrase: IArticlePhrase,
    textActors: IArticleActor[],
    textDictorActors: IArticleActor[],
    translatedActors: IArticleActor[],
    translatedDictorActors: IArticleActor[],
  ): number {
    let cnt = 1;
    // const arrDictorPhrases = dictorPhrasesText.split(/\r?\n/);
    const arrDictorPhrases = dictorPhrasesText.split(/\r\n|\n|\r/);
    
    arrDictorPhrases.forEach((phrase) => {
      if (phrase && phrase.length > 2) {
        const newItem = this.getEmptyArticlePhrase(article, textActors, textDictorActors,
          translatedActors, translatedDictorActors, ArticlePhraseType.dictor);
        newItem.trText = phrase;
        newItem.orderNum = index + cnt;
        newItem.parentKeyGuid = articlePhrase.keyGuid;
        newItem.hasChildren = false;
        article.articlePhrases.splice(index + cnt++, 0, newItem);
      }
    });

    return (cnt - 1);
  }

  static addMixedToArticle(
    newItems,
    index,
    activityType,
    article: IArticle,
    articlePhrase: IArticlePhrase,
    textActors: IArticleActor[],
    textDictorActors: IArticleActor[],
    translatedActors: IArticleActor[],
    translatedDictorActors: IArticleActor[],
    beforeMixDictorPhrases: string,
    insideMixDictorPhrases: string,
    typeOfSourceForMix: number = 0
  ): number {

    if (newItems.length === 0) {
      return 0;
    }

    const textDictorActorDefault = textDictorActors.find(x => x.defaultInRole);
    const translatedDictorActorDefault = translatedDictorActors.find(x => x.defaultInRole);
    let cnt = 1;

    
    // insert "before dictor phrases"
    if (newItems.length > 0
      && beforeMixDictorPhrases
      && beforeMixDictorPhrases.length > 0) {
      // add dictor phrases before Mix
      let beforeMixDictorPhrasesRnd = this.GetOnePhraseOrRnd(beforeMixDictorPhrases);
      const cnt1 = this.addChildDictorPhrasesToArticle(beforeMixDictorPhrasesRnd, index + (cnt - 1), 0,
        article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors);
      cnt += cnt1;
    }


    let arrInsideDictorPhraseTextBlocks: string[] = [];

    // calculate, after how items insert dictor phrase by CntPhrases/CntDictorPhrases
    /*
    // prepare inside phrase textes
    if (insideMixDictorPhrases && insideMixDictorPhrases.length > 2) {
      arrInsideDictorPhraseTextBlocks = this.GetListPhraseOrRnd(insideMixDictorPhrases);
    }

    // const needInsert = Math.floor(arrInsideDictorPhraseTextBlocks.length > 0
    //   ? newItems.length / (arrInsideDictorPhraseTextBlocks.length + 1)
    //   : newItems.length * 2) - 1;
    */

    const needStepInsert = typeOfSourceForMix === 0 
      ? 12 // first Order mix
      : typeOfSourceForMix === 1
        ? 20 // mix Base words
        : 14; // mix all
    const countToInsert = (newItems.length / needStepInsert) - 1;

    if (insideMixDictorPhrases && insideMixDictorPhrases.length > 2 && countToInsert > 0) {
      arrInsideDictorPhraseTextBlocks = this.GetListPhraseOrRnd2(insideMixDictorPhrases, countToInsert);
    }

    let needInsertCnt = needStepInsert;
    let currIndexToInsert = 0;

    newItems.forEach(item => {
      if (needInsertCnt < 0 
        && (
          item.childrenType === 'order rpt' 
          || item.childrenType === 'rpt' 
          || item.childrenType === 'rnd'
          || item.childrenType === 'rnd repeat wrd'
          || item.childrenType === 'rnd repeat phrase'
          )
        ) {
        // only before this type of phrases
        needInsertCnt = needStepInsert;
        const cnt1 = this.addChildDictorPhrasesToArticle(arrInsideDictorPhraseTextBlocks[currIndexToInsert], index + (cnt - 1), 0,
          article, articlePhrase, textActors, textDictorActors, translatedActors, translatedDictorActors);
        cnt += cnt1;
        currIndexToInsert++;
        if (currIndexToInsert >= arrInsideDictorPhraseTextBlocks.length) {
          currIndexToInsert = 0;
        }
      } else {
        needInsertCnt--;
      }

      const newItem = this.getEmptyArticlePhrase(article, textActors, textDictorActors, translatedActors, translatedDictorActors);

      newItem.type = item.type;
      newItem.text = item.source;
      newItem.trText = item.translated;
      newItem.activityType = activityType;
      newItem.pause = this.calcPauseByWordsQty(item.source.split(' ').length);
      newItem.trPause =
        newItem.type === 0
          ? activityType === 0
            ? newItem.childrenType.length > 0 && newItem.childrenType[0] === 'r'
              ? newItem.pause + 1 // children repeat
              : newItem.pause
            : newItem.pause + 1
          : 2; // after dictor
      newItem.articleActor = newItem.type === 0 ? articlePhrase.articleActor : textDictorActorDefault;
      newItem.articleActorID = newItem.type === 0 ? articlePhrase.articleActorID : textDictorActorDefault.articleActorID;
      newItem.trArticleActor = newItem.type === 0 ? articlePhrase.trArticleActor : translatedDictorActorDefault;
      newItem.trArticleActorID = newItem.type === 0 ? articlePhrase.trArticleActorID : translatedDictorActorDefault.articleActorID;
      newItem.parentKeyGuid = articlePhrase.keyGuid;
      newItem.childrenType = item.childrenType;
      newItem.hasChildren = false;
      article.articlePhrases.splice(index + cnt, 0, newItem);
      cnt++;
    });

    return cnt - 1;
  }

  public static GetListPhraseOrRnd(source: string): string[] {
    let result = [];
    if (source && source.trim()[0] === '{') {
      let sourceObj = this.parseJsonString(source.trim());
      sourceObj.variants.forEach(element => {
        const el = this.GetOneRndFromStrArray(element as Array<string>);
        result.push(this.trimAndReplace(el));
      });
    } else {
      result = source ? source.split('///') : [];
    }

    return result;
  }

  public static GetListPhraseOrRnd2(source: string, qty): string[] {
    let result = [];
    if (source && source.trim()[0] === '{') {
      let sourceObj = this.parseJsonString(source.trim());
      let inxVariant = 0;
      let cntAdded = 0;
      while (cntAdded < qty) {
        const el = this.GetOneRndFromStrArray(sourceObj.variants[inxVariant] as Array<string>);
        result.push(this.trimAndReplace(el));
        cntAdded++;
        if (++inxVariant >= sourceObj.variants.length) {
          inxVariant = 0;
        }
      }
    } else {
      const ar = source ? source.split('///') : [];
      if (ar.length > 0) {
        let inx = 0;
        let cntAdded = 0;
        while (cntAdded < qty) {
          result.push(ar[inx]);
          cntAdded++;
          if (++inx >= ar.length) {
            inx = 0;
          }
        }
      } else {
        result = ar;
      } 
    }

    return result;
  }

  static validTextOrJson(source: string): string | any {
    try {
      const obj = this.GetListPhraseOrRnd(source);
      return '';
    } catch (error) {
      return 'error';
    }
  }

  public static GetOnePhraseOrRnd(source: string): string {
    let result = '';
    if (source && source.trim()[0] === '{') {
      let sourceObj = this.parseJsonString(source.trim());
      const variantIndex = Utils.getRandomInt(0, sourceObj.variants.length - 1)
      result = this.GetOneRndFromStrArray(sourceObj.variants[variantIndex] as Array<string>);
    } else {
      result = source;
    }

    return this.trimAndReplace(result);
  }

  private static trimAndReplace(source: string) {
    let result = source.trim().replace(/\s+/g, " ").replace(/-nl-/g, "\r");
    return result; 
  }

  private static parseJsonString(source: string) {
    let result = source.trim().replace(/\\n/g, "\\n")  
    .replace(/\\'/g, "\\'")
    .replace(/\\"/g, '\\"')
    .replace(/\\&/g, "\\&")
    .replace(/\\r/g, "\\r")
    .replace(/\\t/g, "\\t")
    .replace(/\\b/g, "\\b")
    .replace(/\\f/g, "\\f");
    result = result.replace(/[\u0000-\u0019]+/g,"").replace(/\s+/g, " ");; 
    return JSON.parse(result);
  }

  private static GetOneRndFromStrArray(sourceArr: string[]): string {
    let result = '';
    if (sourceArr && sourceArr.length > 0) {
      const ind = Utils.getRandomInt(0, sourceArr.length -1);
      result = sourceArr[ind];
    }

    return result;
  }
 
  static isSentence(str: string): boolean {
    if (!str) {
      return false;
    }

    if (!str.endsWith('.')
      && !str.endsWith('?')
      && !str.endsWith('!')
    ) {
      return false;
    } else {
      return true;
    }
  }

  static mixPhrasesByOrderRndMethod(
    resultArr: Array<IMixItem>,
    active: boolean,
    repeat: number,
    extraSlow: boolean,
    slow: boolean
  ) {

    const newItems: Array<IPrepareMixItem> = [];
    if (active) {
      resultArr.forEach(el => {
        if (el.source && el.source.length > 0) {
          const item: IPrepareMixItem =
            { source: el.source, translated: el.inContext, childrenType: 'order rpt', type: 0 };
          for (let a = 0; a < repeat; a++) {
            // every time add 3 phrases
            // 1. original
            newItems.push(item);

            if (extraSlow) {
              // 2. without translation lowly 40% with breack 400 ms
              newItems.push({ ...item, translated: '', childrenType: 'order rpt 2', source: this.markAudiAsSlow(el.source, 2) });
            }

            if (slow) {
              // 3. without translation lowly 80% with breack 200 ms
              newItems.push({ ...item, translated: '', childrenType: 'order rpt 3', source: this.markAudiAsSlow(el.source, 1) });
            }
          }
        }
      });
    }

    return newItems;
  }

  static getActorLabel(articleActor: IArticleActor) {
    if (!articleActor) {
      return '';
    }

    return articleActor.name + ' / ' + articleActor.voiceName
      + ' / ' + articleActor.voicePitch + ' / ' + articleActor.voiceSpeed;
  }

  static getEmptyArticlePhrase(
    article: IArticle,
    textActors: IArticleActor[],
    textDictorActors: IArticleActor[],
    translatedActors: IArticleActor[],
    translatedDictorActors: IArticleActor[],
    type: ArticlePhraseType = ArticlePhraseType.speaker,
    articlePhrase?: IArticlePhrase): IArticlePhrase {

    const defaultTextActor: IArticleActor =
      (type === ArticlePhraseType.speaker)
        ? textActors.find(x => x.defaultInRole)
        : textDictorActors.find(x => x.defaultInRole);

    const defaultTranslationActor: IArticleActor =
      (type === ArticlePhraseType.speaker)
        ? translatedActors.find(x => x.defaultInRole)
        : translatedDictorActors.find(x => x.defaultInRole);

    defaultTextActor.articleID = article.articleID;
    defaultTranslationActor.articleID = article.articleID;

    const emptyArticlePhrase: IArticlePhrase = {
      articlePhraseID: 0,
      articleID: article.articleID,
      activityType: (type === ArticlePhraseType.speaker) ? ArticlePhraseActivityType.textFirst : ArticlePhraseActivityType.trTextFirst,
      type,
      text: '',
      language: article.language,
      textSpeechFileName: '',
      hashCode: 0,
      speachDuration: 0,
      articleActorID: defaultTextActor.articleActorID,
      articleActor: defaultTextActor,
      pause: 1,
      trText: '',
      trLanguage: article.languageTranslation,
      trTextSpeechFileName: '',
      trHashCode: 0,
      trSpeachDuration: 0,
      trArticleActorID: defaultTranslationActor.articleActorID,
      trArticleActor: defaultTranslationActor,
      trPause: 1,
      orderNum: 0,
      keyGuid: Guid.newGuid().ToString(),
      selected: false,
      showActor: false,
      showTrActor: false,
      parentKeyGuid: articlePhrase ? articlePhrase.parentKeyGuid : '',
      hasChildren: false,
      childrenType: '',
      childrenClosed: false,
      hidden: false,
      silent: false
    };

    return emptyArticlePhrase;
  }

  static markAudiAsSlow(text: string, slowInd: number): string {
    const arr = text.split(' ');
    let result = slowInd === 1 ? '~' : slowInd === 2 ? '~~' : '';
    const pause = slowInd === 1 ? '|' : slowInd === 2 ? '||' : '';
    arr.forEach((str) => {
      result += (pause + str + ' ');
    });

    return result.trim();
  }

  static calcPauseByWordsQty(qty: number) {
    return qty === 0 ? 1
      : qty < 3 ? qty
        : qty < 5 ? qty - 1
          : qty < 8 ? qty - 2
            : qty - 3;

  }

  static SetPhraseHiddenProp(text: string, phraseHiddenProperty: IPhraseHiddenProperty): string {
    const textWithProp = ArticleUtils.GetTextWithPhraseHiddenProp(text);
    if (textWithProp.PhraseHiddenProperty) {
      textWithProp.PhraseHiddenProperty.Pron = phraseHiddenProperty.Pron;
    } else {
      textWithProp.PhraseHiddenProperty = phraseHiddenProperty;
    }

    const jsonPart = this.isPhraseHiddenProp(textWithProp.PhraseHiddenProperty)
      ? ' ' + JSON.stringify(textWithProp.PhraseHiddenProperty)
      : '';
    const result = `${textWithProp.Text}${jsonPart}`;

    return result;
  }

  static GetWithoutPhraseHiddenProp(text: string): string {
    return;
  }

  static GetPhraseHiddenProp(text: string): IPhraseHiddenProperty {
    return;
  }

  static isPhraseHiddenProp(item: IPhraseHiddenProperty): boolean {
    if (item?.Pron) {
      return true;
    }

    return false;
  }

  static GetTextWithPhraseHiddenProp(text: string): ITextWithPhraseHiddenProperty {
    const start = text.indexOf('{');
    const end = text.indexOf('}');
    if ( start < 0 || end < 0) {
      return { Text: text} as ITextWithPhraseHiddenProperty;
    }

    const json = text.substring(start, end);
    const resText = text.substring(0, end + 1);

    const phraseHiddenProperty = JSON.parse(json) as IPhraseHiddenProperty;
    return { Text: resText, PhraseHiddenProperty: phraseHiddenProperty} as ITextWithPhraseHiddenProperty;
  }

  static mixPhrasesRndMethod(
    resultArr: Array<IMixItem>,
    active: boolean,
    repeat: number,
    wordsToRepeatMixItems: IMixItem[] = [],
    wordPhrasesToRepeatMixItems: IMixItem[] = []
  ) {
    const newItems: Array<IPrepareMixItem> = [];
    const list: Array<IPrepareMixItem> = [];
    let rndList: Array<number> = [];

    if (repeat <= 0) {
      return [];
    }

    // add main words or/and phrases
    let indCnt = 0;
    for (let r = 0; r < repeat; r++) {
      resultArr.forEach((item) => {
        if (item.source && item.source.length > 0) {
          list.push({ source: item.source, translated: item.inContext, childrenType: 'rnd', type: 0 });
          rndList.push(indCnt++);
        }
      });
    }

    // add words to repeat
    for (let r = 0; r < 2; r++) {
      wordsToRepeatMixItems.forEach((item) => {
        if (item.source && item.source.length > 0) {
          list.push({ source: item.source, translated: item.inContext, childrenType: 'rnd repeat wrd', type: 0 });
          rndList.push(indCnt++);
        }
      });
    }

    // add phrases to repeat
    for (let r = 0; r < 2; r++) {
      wordPhrasesToRepeatMixItems.forEach((item) => {
        if (item.source && item.source.length > 0) {
          list.push({ source: item.source, translated: item.inContext, childrenType: 'rnd repeat phrase', type: 0 });
          rndList.push(indCnt++);
        }
      });
    }

    const cnt = list.length - 1;
    list.forEach(item => {
      const rndNum = Utils.getRandomInt(0, rndList.length - 1);
      const ind = rndList[rndNum];
      newItems.push(list[ind]);
      rndList = rndList.filter((x) => x !== ind);
    });

    return newItems;
  }
}

