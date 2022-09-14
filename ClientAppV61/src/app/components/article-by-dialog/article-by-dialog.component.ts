import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit, EventEmitter } from '@angular/core';
import { FormsModule, FormGroup, Validators, FormBuilder, ReactiveFormsModule, FormControl } from '@angular/forms';
import { Guid, Utils } from '../../shared/models/System';
import { switchMap, startWith, map, catchError, takeUntil, tap, concatMap } from 'rxjs/operators';
import { merge, Observable, of as observableOf, Subject } from 'rxjs';
import { MatDialog} from '@angular/material/dialog';
import { AuthorizeService, IUser } from '../../../api-authorization/authorize.service';
import { IWordSelected } from 'src/app/shared/interfaces/IWordSelected';
import { IMixParams, IMixItem } from 'src/app/shared/interfaces/IMixItem';
import { PhrasesMixType, ArticleActorRole, ArticlePhraseType } from 'src/app/shared/enums/Enums';
import { IArticle } from 'src/app/shared/interfaces/IArticle';
import { GlobalService } from 'src/app/shared/services/global.service';
import { IArticlePhrase } from 'src/app/shared/interfaces/IArticlePhrase';
import { IArticleActor } from 'src/app/shared/interfaces/IArticleActor';
import { ArticleUtils } from 'src/app/shared/models/ArticleUtils';
import { ArticleService } from 'src/app/shared/services/article.service';
import { ToastrService } from 'ngx-toastr';
import { AppSettingsService } from '../../shared/services/app-settings.service';
import { MixParamService } from '../../shared/services/mix-param.service';
import { SelectTemplateTextDialogComponent } from '../../shared/components/select-template-text-dialog/select-template-text-dialog.component';
import { IMixParamTextTemp } from '../../shared/interfaces/IMixParamTextTemp';
import { IArticleByParam, IArticleByParamData } from '../../shared/interfaces/IArticleByParam';
import { ArticleByParamService } from '../../shared/services/articleByParam.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { UpdateTemplateJsonDialogComponent } from 'src/app/shared/components/update-template-json-dialog/update-template-json-dialog.component';
import { ISearchListArg } from 'src/app/shared/interfaces/IListArg';

@Component({
  selector: 'app-article-by-dialog',
  templateUrl: './article-by-dialog.component.html',
  styleUrls: ['./article-by-dialog.component.scss']
})
export class ArticleByDialogComponent implements OnInit, OnDestroy {
  public isLoadingResults = false;
  // public mixParamsList = new Array<IMixParams>();
  public mixParamsV1: IMixParams;

  public formGr: FormGroup;

  public articleByParam: IArticleByParam;
  public articleByParamData: IArticleByParamData;

  private ngUnsubscribe$ = new Subject();
  private articleCnt = 0;
  private currentUser: IUser;

  private firstDictorPhrasesPreset = '';
  private beforeFinishDictorPhrasesPreset = '';
  private finishDictorPhrasesPreset = '';
  private dialogTextTmp = '';
  private phrasesTextTmp = '';
  private phrasesTranslationTextTmp = '';

  constructor(
    private toasterService: ToastrService,
    private fb: FormBuilder,
    private globalService: GlobalService,
    private articleService: ArticleService,
    public authorizeService: AuthorizeService,
    private appSettingsService: AppSettingsService,
    private mixParamService: MixParamService,
    private dialog: MatDialog,
    private articleByParamService: ArticleByParamService,
    private route: ActivatedRoute,
  ) {
  }

  ngOnInit() {

    this.articleByParamData = {
      mixParamsList: new Array<IMixParams>(),
      baseName: '',
      firstDictorPhrases: '',
      beforeFinishDictorPhrases: '',
      finishDictorPhrases: '',
      wordsToRepeat: [],
      wordPhrasesToRepeat: [],
      maxWordsToRepeatForGeneration: 35
    };

    this.route.paramMap
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe((params: ParamMap) => {
        this.authorizeService.getCurrentUser()
          .pipe(takeUntil(this.ngUnsubscribe$))
          .subscribe((user) => {
            this.currentUser = user;
            this.getData(params.get('id'));
          });
      });

    this.mixParamsV1 = this.getMixParamsV1();


    this.formGr = this.fb.group({
      baseName: ['', [Validators.required]],
      dialogText: [this.dialogTextTmp, [Validators.required]],
      phrasesText: [this.phrasesTextTmp, [Validators.required]],
      phrasesTranslationText: [this.phrasesTranslationTextTmp, [Validators.required]],
      firstDictorPhrases: [this.firstDictorPhrasesPreset, [Validators.required]],
      beforeFinishDictorPhrases: [this.beforeFinishDictorPhrasesPreset, [Validators.required]],
      finishDictorPhrases: [this.finishDictorPhrasesPreset, [Validators.required]],
    });
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
          const val = this.formGr.get(formControlName).value;
          let res = '';
          dialogResult.forEach(x => {
            res = (res ? (res + '\n') : '') + x.text;
          });

          res = (val ? val + '\n' : '') + res;
          this.formGr.get(formControlName).setValue(res);
        }
      });
  }

  getData(id) {
    if (id > 0) {
      this.setLoader(true);
      this.articleByParamService.getById(id)
        .toPromise()
        .then(result => {
          console.log('articleByParamService.getById: ', result);
          // this.SetChanged(false);
          this.articleByParam = result;
          this.articleByParamData = JSON.parse(this.articleByParam.dataJson);

          this.formGr.get('baseName').setValue(this.articleByParamData.baseName);
          this.formGr.get('dialogText').setValue(this.articleByParamData.dialogText);
          this.formGr.get('phrasesText').setValue(this.articleByParamData.phrasesText);
          this.formGr.get('phrasesTranslationText').setValue(this.articleByParamData.phrasesTranslationText);
          this.formGr.get('firstDictorPhrases').setValue(this.articleByParamData.firstDictorPhrases);
          this.formGr.get('beforeFinishDictorPhrases').setValue(this.articleByParamData.beforeFinishDictorPhrases);
          this.formGr.get('finishDictorPhrases').setValue(this.articleByParamData.finishDictorPhrases);
        }).catch(err => {
          console.log('err: ', err);
        }).finally(() => {
          this.setLoader(false);
        });
    } else {
      // new article
      const currAppSettings = this.appSettingsService.getCurrent();
      this.articleByParam = {
        articleByParamID: 0,
        userID: this.currentUser.id,
        learnLanguageID: currAppSettings.LearnLanguage.languageID,
        nativeLanguageID: currAppSettings.NativeLanguage.languageID,
        name: '',
        type: 1, // by dialog
        dataJson: ''
      } as IArticleByParam;

      // this.SetChanged(true);
    }
  }

  save() {
    if (!this.articleByParam.name) {
      alert('Parameters name required for save');
      return;
    }

    this.setLoader(true);

    this.articleByParam.dataJson = this.getTemplateAsJson();

    this.articleByParamService.save(this.articleByParam)
      .toPromise()
      .then(result => {
        this.articleByParam.articleByParamID = result.articleByParamID;
        this.setLoader(false);
      });
  }

  onSubmit() {
    this.save();

    if (this.formGr.invalid) {
      alert('something i wrong with form');
      return;
    }

    if (this.articleByParamData.mixParamsList.length === 0) {
      alert('one article parameter is required at list');
      return;
    }

    const arrPhrases = (this.formGr.get('phrasesText').value as string).split(/\r?\n/);
    const arrPhrasesTr = (this.formGr.get('phrasesTranslationText').value as string).split(/\r?\n/);

    if (arrPhrases.length !== arrPhrasesTr.length) {
      alert('Lines in phrases and its translation is not equal');
      return;
    }

    let mixParamsReq = false;
    this.articleByParamData.mixParamsList.forEach((mixParams) => {
      if (!mixParams.firstDictorPhrases) {
        mixParamsReq = true;
      }
    });

    if (mixParamsReq) {
      alert('In some article not all requeue parameters were filled out');
      return;
    }

    const mixParamsSave$ = new Subject<IMixParams>();
    mixParamsSave$
      .pipe(
        takeUntil(this.ngUnsubscribe$),
        concatMap((newMixParams: IMixParams) => {
          return this.mixParamService.saveMixParam(newMixParams);
        })
      ).subscribe(
        (result) => {
          console.log('save IMixParams:', result);
          this.toasterService
            .info(`MixParams ${result} Saved`, 'Success');
        },
        error => {
          console.log('Save error:', error);
          this.toasterService.error('MixParams create:', 'Error');

        },
        () => {
          this.setLoader(false);
          console.log('MixParams Save complited');
          this.toasterService.info('MixParams create comleted', 'Success');
        }
      );

    const articleSave$ = new Subject<IArticle>();
    articleSave$
    .pipe(
      takeUntil(this.ngUnsubscribe$),
      concatMap((newArticle: IArticle) => {
        return this.articleService.Save(newArticle);
      })
    ).subscribe(
      (result) => {
        console.log('ArticleSaved:', result);
        this.toasterService
        .info(`Articles ${result} created`, 'Success', {disableTimeOut: true});
      },
      error => {
        console.log('Save error:', error);
        this.toasterService.error('Article create:', 'Error');

      },
      () => {
        this.setLoader(false);
        console.log('Save complited');
        this.toasterService.success('Articles create comleted', 'Success');
      }
    );

    const mixItems = this.gatherItems(arrPhrases, arrPhrasesTr);
    console.log('mixItems', mixItems);

    let phraseText = '';
    let trPhraseText = '';

    // fiil out text and translation for new phrase
    mixItems.forEach((mixItem) => {
      if (phraseText) {
        phraseText = phraseText + '\r';
      }

      if (!ArticleUtils.isSentence(mixItem.source)) {
        mixItem.source += '.';
      }

      phraseText = phraseText + mixItem.source;

      if (trPhraseText) {
        trPhraseText = trPhraseText + '\r';
      }

      if (!ArticleUtils.isSentence(mixItem.inContext)) {
        mixItem.inContext += '.';
      }

      trPhraseText = trPhraseText + mixItem.inContext;
    });

    let articleCnt = 1;
    this.articleByParamData.mixParamsList.forEach((mixParams: IMixParams) => {
      mixParams.mixItems = mixItems;


      // Create article
      const newArticle = {
        articleID: 0,
        userID: this.currentUser.id,
        name: `${this.formGr.get('baseName').value}_${articleCnt}`,
        language: this.appSettingsService.getCurrent().LearnLanguage.code,
        languageTranslation: this.appSettingsService.getCurrent().NativeLanguage.code,
        textHashCode: '',
        textSpeechFileName: '',
        videoFileName: '',
        articlePhrases: new Array<IArticlePhrase>(),
        articleActors: new Array<IArticleActor>()
      } as IArticle;


      // actors
      const allActors = this.globalService.getActorsForArticle(newArticle);
      newArticle.articleActors = allActors;

      const textActors = newArticle.articleActors.filter((x) => x.role === ArticleActorRole.textSpeaker);
      const textDictorActors = newArticle.articleActors.filter((x) => x.role === ArticleActorRole.textDictorSpeaker);
      const translatedActors = newArticle.articleActors.filter((x) => x.role === ArticleActorRole.textTranslatedSpeaker);
      const translatedDictorActors = newArticle.articleActors.filter((x) => x.role === ArticleActorRole.textTranslatedDictorSpeaker);

      const textActorDefault = textActors.find(x => x.defaultInRole);
      const translatedActorDefault = translatedActors.find(x => x.defaultInRole);
      const textDictorActorDefault = textDictorActors.find(x => x.defaultInRole);
      const translatedDictorActorDefault = translatedDictorActors.find(x => x.defaultInRole);

      let orderNum = 0;

      // add first dictor phrases (general)
      const arrFirstDictorPhrasesGen = this.formGr.get('firstDictorPhrases').value.split(/\r?\n/);
      arrFirstDictorPhrasesGen.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
            translatedActors, translatedDictorActors, ArticlePhraseType.dictor);
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      // add first dictor phrases (article spicific)
      const arrFirstDictorPhrase = mixParams.firstDictorPhrases.split(/\r?\n/);
      arrFirstDictorPhrase.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
            translatedActors, translatedDictorActors, ArticlePhraseType.dictor);
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      // add dialog text
      const arrDialogText = this.formGr.get('dialogText').value.split(/\r?\n/);
      const dialogPhrases = new Array<IArticlePhrase>();
      arrDialogText.forEach((phrase: string) => {
        if (phrase && phrase.length > 2) {
          let phText = phrase;
          const newPhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
            translatedActors, translatedDictorActors, ArticlePhraseType.speaker);

          if (phrase.indexOf('::') > 0) {
            // actor name was specifited
            const arr = phrase.split('::');
            const actorNameStart = arr[0];
            phText = arr[1].trim();

            const actor = textActors.find(x => x.name.startsWith(actorNameStart));
            if (actor) {
              newPhrase.articleActor = actor;
              newPhrase.articleActorID = actor.articleActorID;
            }
          }

          newPhrase.text = phText;
          newPhrase.trText = '';
          dialogPhrases.push(newPhrase);
        }
      });

      dialogPhrases.forEach((artPhrase: IArticlePhrase) => {
        artPhrase.orderNum = orderNum++;
        newArticle.articlePhrases.push({ ...artPhrase });
      });
      //// create new phrases

      const articlePhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
        translatedActors, translatedDictorActors);
      articlePhrase.text = phraseText;
      articlePhrase.trText = trPhraseText;
      articlePhrase.orderNum = orderNum++;
      articlePhrase.silent = true;

      newArticle.articlePhrases.push(articlePhrase);

      newArticle.articlePhrases.forEach((x) => {
        if (x.type === ArticlePhraseType.speaker) {
          x.showActor = !(x.articleActor.keyGuid === textActorDefault.keyGuid);
          x.showTrActor = !(x.trArticleActor.keyGuid === translatedActorDefault.keyGuid);
        } else {
          x.showActor = !(x.articleActor.keyGuid === textDictorActorDefault.keyGuid);
          x.showTrActor = !(x.trArticleActor.keyGuid === translatedDictorActorDefault.keyGuid);
        }
      });

      const res = ArticleUtils.mixAndAddToArticle(mixParams, newArticle, articlePhrase,
          textActors, textDictorActors, translatedActors, translatedDictorActors);

      articlePhrase.hasChildren = true;

      // add dictor after end of mixes

      // add before Finish Dictor Phrases (general)
      const arrbeforeFinishDictorPhrasesGen = this.formGr.get('beforeFinishDictorPhrases').value.split(/\r?\n/);
      arrbeforeFinishDictorPhrasesGen.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
            translatedActors, translatedDictorActors, ArticlePhraseType.dictor);
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      if (mixParams.beforeFinishDictorPhrases) {
        // add before Finish Dictor Phrases (article spicific)
        const arrbeforeByOrderMixDictorPhrases = mixParams.beforeFinishDictorPhrases.split(/\r?\n/);
        arrbeforeByOrderMixDictorPhrases.forEach((phrase) => {
          if (phrase && phrase.length > 2) {
            const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
              translatedActors, translatedDictorActors, ArticlePhraseType.dictor);
            firstDictorPhrase.trText = phrase;
            firstDictorPhrase.orderNum = orderNum++;
            newArticle.articlePhrases.push(firstDictorPhrase);
          }
        });
      }

      // add dialog phrases to end of article
      dialogPhrases.forEach((artPhrase: IArticlePhrase) => {
        artPhrase.orderNum = orderNum++;
        newArticle.articlePhrases.push({ ...artPhrase });
      });

      // add Finish Dictor Phrases (general)
      const arrFinishDictorPhrasesGen = this.formGr.get('finishDictorPhrases').value.split(/\r?\n/);
      arrFinishDictorPhrasesGen.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
            translatedActors, translatedDictorActors, ArticlePhraseType.dictor);
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      if (mixParams.finishDictorPhrases) {
        // add Finish Dictor Phrases (article spicific)
        const arrbeforeByOrderMixDictorPhrases = mixParams.finishDictorPhrases.split(/\r?\n/);
        arrbeforeByOrderMixDictorPhrases.forEach((phrase) => {
          if (phrase && phrase.length > 2) {
            const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(newArticle, textActors, textDictorActors,
              translatedActors, translatedDictorActors, ArticlePhraseType.dictor);
            firstDictorPhrase.trText = phrase;
            firstDictorPhrase.orderNum = orderNum++;
            newArticle.articlePhrases.push(firstDictorPhrase);
          }
        });
      }

      // recalculate order
      ArticleUtils.recalculateArticlePhrasesOrder(newArticle);

      articleCnt = articleCnt + 1;

      console.log(`new article ${newArticle.name}`, newArticle);

      // merge voices list
      newArticle.articleActors = [];
      textActors.forEach(x => newArticle.articleActors.push(x));
      translatedActors.forEach(x => newArticle.articleActors.push(x));
      textDictorActors.forEach(x => newArticle.articleActors.push(x));
      translatedDictorActors.forEach(x => newArticle.articleActors.push(x));

      this.setLoader(true);

      articleSave$.next(newArticle);

      mixParams.articlePhraseKeyGuid = articlePhrase.keyGuid;
      mixParams.textHashCode = Utils.getHashCode(articlePhrase.text);
      mixParams.trTextHashCode = Utils.getHashCode(articlePhrase.trText);

      mixParamsSave$.next(mixParams);
    });

    articleSave$.complete();
  }

  gatherItems(arrPhrases: string[], arrPhrasesTr: string[]): IMixItem[] {
    const mixItems = new Array<IMixItem>();
    let cnt = 0;

    arrPhrases.forEach((phrase, index) => {
      mixItems.push(
        {
          mixItemID: 0,
          mixParamID: 0,
          keyGuid: Guid.newGuid().ToString(),
          source: phrase,
          inDict: '',
          inContext: arrPhrasesTr[index],
          endPhrase: false,
          pretext: false,
          childrenType: '',
          orderNum: cnt++,
          baseWord: phrase.trim().indexOf(' ') === -1,
        } as IMixItem
      );
    });

    return mixItems;
  }

  setLoader(value) {
    this.isLoadingResults = value;
  }

  addMixParam(mixType: number) {
    this.articleByParamData.mixParamsList.push(this.getMixParamsV0());
  }

  deleteMixParamFromList(mixParam) {
    if (window.confirm('Are sure you want to delete this item ?')) {
      this.articleByParamData.mixParamsList =
        this.articleByParamData.mixParamsList.filter(x => x.articlePhraseKeyGuid !== mixParam.articlePhraseKeyGuid);
    }
  }

  add3StepMixParam() {
    this.addStep1();
    this.addStep2();
    this.addStep3();
  }

  addStep1() {
    this.articleByParamData.mixParamsList.push(this.getMixParamsV1());
  }

  addStep2() {
    this.articleByParamData.mixParamsList.push(this.getMixParamsV2());
  }

  addStep3() {
    this.articleByParamData.mixParamsList.push(this.getMixParamsV3());
  }

  getMixParamsV0() {
    const item = this.getEmptyMixParams();

    return {
      ...item,
      active: true,
      repeatOrder: 0,
      addSlow2InRepeatOrder: false,
      addSlowInRepeatOrder: false,
      repeatBaseWord: 0,
      repeat: 0,
      trActive: true,
      trRepeatOrder: 0,
      trAddSlow2InRepeatOrder: false,
      trAddSlowInRepeatOrder: false,
      trRepeatBaseWord: 0,
      trRepeat: 0,
    };
  }

  getMixParamsV1() {
    const item = this.getEmptyMixParams();
    return {
      ...item,
      name: 'template 1',
      active: true,
      repeatOrder: 3,
      addSlow2InRepeatOrder: true,
      addSlowInRepeatOrder: true,
      repeatBaseWord: 0,
      repeat: 0,
      trActive: false,
      trRepeatOrder: 0,
      trAddSlow2InRepeatOrder: false,
      trAddSlowInRepeatOrder: false,
      trRepeatBaseWord: 0,
      trRepeat: 0,
    };
  }

  getMixParamsV2() {
    const item = this.getEmptyMixParams();
    return {
      ...item,
      name: 'template 2',
      active: true,
      repeatOrder: 0,
      addSlow2InRepeatOrder: false,
      addSlowInRepeatOrder: false,
      repeatBaseWord: 10,
      repeat: 0,
      trActive: true,
      trRepeatOrder: 0,
      trAddSlow2InRepeatOrder: false,
      trAddSlowInRepeatOrder: false,
      trRepeatBaseWord: 20,
      trRepeat: 0,
    };
  }

  getMixParamsV3() {
    const item = this.getEmptyMixParams();
    return {
      ...item,
      name: 'template 3',
      active: true,
      repeatOrder: 0,
      addSlow2InRepeatOrder: false,
      addSlowInRepeatOrder: false,
      repeatBaseWord: 0,
      repeat: 0,
      trActive: true,
      trRepeatOrder: 0,
      trAddSlow2InRepeatOrder: false,
      trAddSlowInRepeatOrder: false,
      trRepeatBaseWord: 0,
      trRepeat: 12,
    };
  }

  getEmptyMixParams() {
    this.articleCnt = this.articleCnt + 1;
    return {
      articlePhraseKeyGuid: Guid.newGuid().ToString(),
      name: 'article params' + this.articleCnt.toString(),
      textHashCode: 0,
      trTextHashCode: 0,
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
      phrasesMixType: PhrasesMixType.random,
      repeat: 0,
      trRepeat: 0,
      repeatOrder: 0,
      trRepeatOrder: 0,
    } as IMixParams;
  }

  cutText(text: string, len: number = 40) {
    let result: string;

    if (!text || text.length <= len) {
      return text;
    }

    result = text.substring(0, len) + ' ...';
    return result;
  }

  getTemplateAsJson() {
    this.articleByParamData.baseName = this.formGr.get('baseName').value;
    this.articleByParamData.dialogText = this.formGr.get('dialogText').value;
    this.articleByParamData.phrasesText = this.formGr.get('phrasesText').value;
    this.articleByParamData.phrasesTranslationText = this.formGr.get('phrasesTranslationText').value;
    this.articleByParamData.firstDictorPhrases = this.formGr.get('firstDictorPhrases').value;
    this.articleByParamData.beforeFinishDictorPhrases = this.formGr.get('beforeFinishDictorPhrases').value;
    this.articleByParamData.finishDictorPhrases = this.formGr.get('finishDictorPhrases').value;
    const jsonStr = JSON.stringify(this.articleByParamData);
    return jsonStr;
  }

  changeTemplateModal() {
    const jsonStr = this.getTemplateAsJson();
    const dialogRef = this.dialog.open(
      UpdateTemplateJsonDialogComponent,
      {
        disableClose: true,
        width: '800px',
        data: jsonStr,
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((dialogResult: ISearchListArg) => {
        if (dialogResult) {
          try {
            this.articleByParamData = JSON.parse(dialogResult.str1);
          } catch (error) {
          }
        }
      });
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
