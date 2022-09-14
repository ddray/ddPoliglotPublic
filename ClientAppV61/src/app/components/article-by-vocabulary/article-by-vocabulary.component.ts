import {
  Component,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
  AfterViewInit,
  EventEmitter
} from '@angular/core';
import {
  FormsModule,
  FormGroup,
  Validators,
  FormBuilder,
  ReactiveFormsModule,
  FormControl
} from '@angular/forms';
import { TranslateService } from '../../shared/services/translate.service';
import { Guid, Utils } from '../../shared/models/System';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import {
  switchMap,
  startWith,
  map,
  catchError,
  takeUntil,
  tap,
  concatMap
} from 'rxjs/operators';
import { merge, Observable, of as observableOf, Subject } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import { AuthorizeService, IUser } from '../../../api-authorization/authorize.service';
import { IWord } from '../../shared/interfaces/IWord';
import { WordService } from '../../shared/services/word.service';
import { IWordPhrase } from '../../shared/interfaces/IWordPhrase';
import { IListArg, ISearchListArg } from 'src/app/shared/interfaces/IListArg';
import { IWordSelected } from 'src/app/shared/interfaces/IWordSelected';
import { SelectWordPhraseDialogComponent } from 'src/app/shared/components/select-word-phrase-dialog/select-word-phrase-dialog.component';
// tslint:disable-next-line:max-line-length
import { UpdateWordTranslationDialogComponent } from 'src/app/shared/components/update-word-translation-dialog/update-word-translation-dialog.component';
import { IWordTranslation } from 'src/app/shared/interfaces/IWordTranslation';
import { IWordPhraseTranslation } from 'src/app/shared/interfaces/IWordPhraseTranslation';
// tslint:disable-next-line:max-line-length
import { UpdateWordphraseTranslationDialogComponent } from 'src/app/shared/components/update-wordphrase-translation-dialog/update-wordphrase-translation-dialog.component';
import { IMixParams, IMixItem } from 'src/app/shared/interfaces/IMixItem';
import {
  PhrasesMixType,
  ArticleActorRole,
  ArticlePhraseType
} from 'src/app/shared/enums/Enums';
import { IArticle } from 'src/app/shared/interfaces/IArticle';
import { GlobalService } from 'src/app/shared/services/global.service';
import { IArticlePhrase } from 'src/app/shared/interfaces/IArticlePhrase';
import { IArticleActor } from 'src/app/shared/interfaces/IArticleActor';
import { ArticleUtils } from 'src/app/shared/models/ArticleUtils';
import { ArticleService } from 'src/app/shared/services/article.service';
import { ToastrService } from 'ngx-toastr';
import { IRatingControlValue } from 'src/app/shared/components/rating/rating.component';
import { IWordUser } from 'src/app/shared/interfaces/IWordUser';
import { AppSettingsService } from '../../shared/services/app-settings.service';
import { ArticleByParamService } from '../../shared/services/articleByParam.service';
import {
  IArticleByParam,
  IArticleByParamData
} from '../../shared/interfaces/IArticleByParam';
import { SelectTemplateTextDialogComponent } from '../../shared/components/select-template-text-dialog/select-template-text-dialog.component';
import { IMixParamTextTemp } from '../../shared/interfaces/IMixParamTextTemp';
import { MixParamService } from '../../shared/services/mix-param.service';
import { IPhraseHiddenProperty } from 'src/app/shared/interfaces/IPhraseHiddenProperty';
import { UpdateTemplateJsonDialogComponent } from 'src/app/shared/components/update-template-json-dialog/update-template-json-dialog.component';
import { SelectRepeatWordsPhrasesDialogComponent } from 'src/app/shared/components/select-repeat-words-phrases-dialog/select-repeat-words-phrases-dialog.component';
import { error } from 'protractor';

@Component({
  selector: 'app-article-by-vocabulary',
  templateUrl: './article-by-vocabulary.component.html',
  styleUrls: ['./article-by-vocabulary.component.scss']
})
export class ArticleByVocabularyComponent implements OnInit, OnDestroy {
  public isLoading = true;
  public articleByParam: IArticleByParam;
  public articleByParamData: IArticleByParamData;
  public formGr: FormGroup;

  private firstDictorPhrasesPreset = '';
  private beforeFinishDictorPhrasesPreset = '';
  private finishDictorPhrasesPreset = '';
  private refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();
  private articleCnt = 0;
  private currentUser: IUser;
  private articleSave$: Subject<IArticle>;
  private dialogTextTmp = '';

  constructor(
    private wordService: WordService,
    private toasterService: ToastrService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private globalService: GlobalService,
    private route: ActivatedRoute,
    private router: Router,
    private articleService: ArticleService,
    public authorizeService: AuthorizeService,
    private appSettingsService: AppSettingsService,
    private articleByParamService: ArticleByParamService,
    private mixParamService: MixParamService
  ) { }

  ngOnInit() {
    this.articleByParamData = {
      selectedWords: new Array<IWordSelected>(),
      mixParamsList: new Array<IMixParams>(),
      baseName: '',
      firstDictorPhrases: '',
      beforeFinishDictorPhrases: '',
      finishDictorPhrases: '',
      wordPhrasesToRepeat: new Array<IWordPhrase>(),
      wordsToRepeat: new Array<IWord>(),
      maxWordsToRepeatForGeneration: 35
    };

    this.route.paramMap
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe((params: ParamMap) => {
        this.authorizeService
          .getCurrentUser()
          .pipe(takeUntil(this.ngUnsubscribe$))
          .subscribe((user) => {
            this.currentUser = user;
            this.getData(params.get('id'));
          });
      });

    this.formGr = this.fb.group({
      baseName: ['', [Validators.required]],
      firstDictorPhrases: [this.firstDictorPhrasesPreset, [Validators.required]],
      beforeFinishDictorPhrases: [
        this.beforeFinishDictorPhrasesPreset,
        [Validators.required]
      ],
      finishDictorPhrases: [this.finishDictorPhrasesPreset, [Validators.required]],
      dialogText: [this.dialogTextTmp],
      maxWordsToRepeatForGeneration: ['35']
    });
  }

  getData(id) {
    if (id > 0) {
      this.setLoader(true);
      this.articleByParamService
        .getById(id)
        .toPromise()
        .then((result) => {
          console.log('articleByParamService.getById: ', result);
          // this.SetChanged(false);
          this.articleByParam = result;
          this.articleByParamData = JSON.parse(this.articleByParam.dataJson);

          this.formGr
            .get('baseName')
            .setValue(this.articleByParamData.baseName);
          this.formGr
            .get('firstDictorPhrases')
            .setValue(this.articleByParamData.firstDictorPhrases);
          this.formGr
            .get('beforeFinishDictorPhrases')
            .setValue(this.articleByParamData.beforeFinishDictorPhrases);
          this.formGr
            .get('finishDictorPhrases')
            .setValue(this.articleByParamData.finishDictorPhrases);
          this.formGr
            .get('dialogText')
            .setValue(this.articleByParamData.dialogText);
          this.formGr
            .get('maxWordsToRepeatForGeneration')
            .setValue(this.articleByParamData.maxWordsToRepeatForGeneration);
        })
        .catch((err) => {
          console.log('err: ', err);
        })
        .finally(() => {
          this.setLoader(false);
        });
    } else {
      const currAppSettings = this.appSettingsService.getCurrent();
      this.articleByParam = {
        articleByParamID: 0,
        userID: this.currentUser.id,
        learnLanguageID: currAppSettings.LearnLanguage.languageID,
        nativeLanguageID: currAppSettings.NativeLanguage.languageID,
        name: '',
        dataJson: ''
      } as IArticleByParam;

      this.setLoader(false);
    }
  }

  getTemplateAsJson() {
    this.articleByParamData.baseName = this.formGr.get('baseName').value;
    this.articleByParamData.firstDictorPhrases =
      this.formGr.get('firstDictorPhrases').value;
    this.articleByParamData.beforeFinishDictorPhrases = this.formGr.get(
      'beforeFinishDictorPhrases'
    ).value;
    this.articleByParamData.finishDictorPhrases =
      this.formGr.get('finishDictorPhrases').value;
    this.articleByParamData.dialogText = this.formGr.get('dialogText').value;
    this.articleByParamData.maxWordsToRepeatForGeneration = this.formGr.get(
      'maxWordsToRepeatForGeneration'
    ).value;
    const jsonStr = JSON.stringify(this.articleByParamData);
    return jsonStr;
  }

  save() {
    if (!this.articleByParam.name) {
      alert('Parameters name required for save');
      return;
    }

    this.isLoading = true;

    this.articleByParam.dataJson = this.getTemplateAsJson();

    this.articleByParamService
      .save(this.articleByParam)
      .toPromise()
      .then((result) => {
        this.articleByParam.articleByParamID = result.articleByParamID;
        this.isLoading = false;
      });
  }

  getDictorTextFromTemplate(tempKey, formControlName) {
    const dialogRef = this.dialog.open(SelectTemplateTextDialogComponent, {
      disableClose: true,
      width: '800px',
      data: { tempKey }
    });

    dialogRef
      .afterClosed()
      .toPromise()
      .then((dialogResult: IMixParamTextTemp[]) => {
        if (dialogResult) {
          const val = this.formGr.get(formControlName).value;
          let res = '';
          dialogResult.forEach((x) => {
            res = (res ? res + '\n' : '') + x.text;
          });

          res = (val ? val + '\n' : '') + res;
          this.formGr.get(formControlName).setValue(res);
        }
      });
  }

  alertErr(err: string) {
    alert(err);
    console.log(err);
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

    if (this.articleByParamData.selectedWords.length === 0) {
      alert('selected words are required');
      return;
    }

    // let err = ArticleUtils.validTextOrJson(this.formGr.get('firstDictorPhrases').value);
    let err = ArticleUtils.validTextOrJson(this.formGr.get('firstDictorPhrases').value);
    if (err) {
      this.alertErr('firstDictorPhrases err: ' + this.formGr.get('firstDictorPhrases').value);
      return;
    }

    err = ArticleUtils.validTextOrJson(this.formGr.get('beforeFinishDictorPhrases').value);
    if (err) {
      this.alertErr('beforeFinishDictorPhrases err: ' + this.formGr.get('beforeFinishDictorPhrases').value);
      return;
    }

    err = ArticleUtils.validTextOrJson(this.formGr.get('finishDictorPhrases').value);
    if (err) {
      this.alertErr('finishDictorPhrases err: ' + this.formGr.get('finishDictorPhrases').value);
      return;
    }

    let mixParamsReq = false;
    this.articleByParamData.mixParamsList.forEach((mixParams) => {
      if (!mixParams.firstDictorPhrases) {
        mixParamsReq = true;
      }

      err = ArticleUtils.validTextOrJson(mixParams.beforeByOrderMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.beforeByOrderMixDictorPhrases err: ' + mixParams.beforeByOrderMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.insideByOrderMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.insideByOrderMixDictorPhrases err: ' + mixParams.insideByOrderMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.beforeBaseWordsRevMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.beforeBaseWordsRevMixDictorPhrases err: ' + mixParams.beforeBaseWordsRevMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.insideBaseWordsRevMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.insideBaseWordsRevMixDictorPhrases err: ' + mixParams.insideBaseWordsRevMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.beforeBaseWordsDirMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.beforeBaseWordsDirMixDictorPhrases err: ' + mixParams.beforeBaseWordsDirMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.insideBaseWordsDirMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.insideBaseWordsDirMixDictorPhrases err: ' + mixParams.insideBaseWordsDirMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.beforeAllRevMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.beforeAllRevMixDictorPhrases err: ' + mixParams.beforeAllRevMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.insideAllRevMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.insideAllRevMixDictorPhrases err: ' + mixParams.insideAllRevMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.beforeAllDirMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.beforeAllDirMixDictorPhrases err: ' + mixParams.beforeAllDirMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.insideAllDirMixDictorPhrases);
      if (err) {
        this.alertErr('mixParams.insideAllDirMixDictorPhrases err: ' + mixParams.insideAllDirMixDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.firstDictorPhrases);
      if (err) {
        this.alertErr('mixParams.firstDictorPhrases err: ' + mixParams.firstDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.finishDictorPhrases);
      if (err) {
        this.alertErr('mixParams.finishDictorPhrases err: ' + mixParams.finishDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.beforeFinishDictorPhrases);
      if (err) {
        this.alertErr('mixParams.finishDictorPhrases err: ' + mixParams.beforeFinishDictorPhrases);
        return;
      }

      err = ArticleUtils.validTextOrJson(mixParams.firstBeforeDialogDictorPhrases);
      if (err) {
        this.alertErr('mixParams.firstBeforeDialogDictorPhrases err: ' + mixParams.firstBeforeDialogDictorPhrases);
        return;
      }

    });

    if (mixParamsReq) {
      alert('In some article not all requeue parameters were filled out');
      return;
    }


    let allTranslatedAndExists = true;
    this.articleByParamData.selectedWords.forEach((wordSelected) => {
      if (
        !wordSelected.word.text ||
        !wordSelected.word.wordTranslation ||
        !wordSelected.word.wordTranslation.text
      ) {
        allTranslatedAndExists = false;
      }

      if (!wordSelected.phraseWordsSelected) {
        allTranslatedAndExists = false;
      } else {
        wordSelected.phraseWordsSelected.forEach((wordPhrase) => {
          if (
            !wordPhrase.text ||
            !wordPhrase.wordPhraseTranslation ||
            !wordPhrase.wordPhraseTranslation.text
          ) {
            allTranslatedAndExists = false;
          }
        });
      }
    });

    if (!allTranslatedAndExists) {
      alert('some words have not any phrases or there are no some translations');
      return;
    }

    const mixParamsSave$ = new Subject<IMixParams>();
    mixParamsSave$
      .pipe(
        takeUntil(this.ngUnsubscribe$),
        concatMap((newMixParams: IMixParams) => {
          return this.mixParamService.saveMixParam(newMixParams);
        })
      )
      .subscribe(
        (result) => {
          console.log('save IMixParams:', result);
          this.toasterService.info(`MixParams ${result} Saved`, 'Success');
        },
        (error) => {
          console.log('Save error:', error);
          this.toasterService.error('MixParams create:', 'Error');
        },
        () => {
          this.setLoader(false);
          console.log('MixParams Save complited');
          this.toasterService.success('MixParams create comleted', 'Success');
          this.save(); // save with current articles main phrases keyGuid to link article and mixParams
        }
      );

    this.articleSave$ = new Subject<IArticle>();
    this.articleSave$
      .pipe(
        takeUntil(this.ngUnsubscribe$),
        concatMap((newArticle: IArticle) => {
          return this.articleService.Save(newArticle);
        })
      )
      .subscribe(
        (result) => {
          console.log('ArticleSaved:', result);
          this.toasterService.info(`Articles ${result} created`, 'Success');
        },
        (error) => {
          console.log('Save error:', error);
          this.toasterService.error('Article create:', 'Error');
        },
        () => {
          this.setLoader(false);
          console.log('Save complited');
          this.toasterService.success('Articles create comleted', 'Success');
        }
      );

    const [mixItems, wordsToRepeatMixItems, wordPhrasesToRepeatMixItems] =
      this.gatherItems();
    let phraseText = '';
    let trPhraseText = '';

    // fill out presentation text which will be placed in beginning and in conclusion
    const arrLessonText = new Array<string>();

    // fill out text and translation for new phrase

    mixItems.forEach((mixItem) => {
      if(this.articleByParamData.selectedWords.length < 50){

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

      arrLessonText.push(mixItem.source);
    }
    });

    let articleCnt = 1;
    this.articleByParamData.mixParamsList.forEach((mixParams) => {
      mixParams.mixItems = mixItems;

      // Create article
      const newArticle = {
        articleID: 0,
        userID: this.currentUser.id,
        name: `${this.articleByParamData.baseName}_${articleCnt}`,
        language: this.appSettingsService.getCurrent().LearnLanguage.code,
        languageTranslation:
          this.appSettingsService.getCurrent().NativeLanguage.code,
        textHashCode: '',
        textSpeechFileName: '',
        videoFileName: '',
        articlePhrases: new Array<IArticlePhrase>(),
        articleActors: new Array<IArticleActor>()
      } as IArticle;

      // actors
      const allActors = this.globalService.getActorsForArticle(newArticle);
      newArticle.articleActors = allActors;

      const textActors = newArticle.articleActors.filter(
        (x) => x.role === ArticleActorRole.textSpeaker
      );
      const textDictorActors = newArticle.articleActors.filter(
        (x) => x.role === ArticleActorRole.textDictorSpeaker
      );
      const translatedActors = newArticle.articleActors.filter(
        (x) => x.role === ArticleActorRole.textTranslatedSpeaker
      );
      const translatedDictorActors = newArticle.articleActors.filter(
        (x) => x.role === ArticleActorRole.textTranslatedDictorSpeaker
      );

      const textActorDefault = textActors.find((x) => x.defaultInRole);
      const translatedActorDefault = translatedActors.find((x) => x.defaultInRole);
      const textDictorActorDefault = textDictorActors.find((x) => x.defaultInRole);
      const translatedDictorActorDefault = translatedDictorActors.find(
        (x) => x.defaultInRole
      );

      let orderNum = 0;

      // add first dictor phrases (general)
      const arrFirstDictorPhrasesGen = ArticleUtils
        .GetOnePhraseOrRnd(this.formGr.get('firstDictorPhrases').value) 
        .split(/\r\n|\n|\r/);

      arrFirstDictorPhrasesGen.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(
            newArticle,
            textActors,
            textDictorActors,
            translatedActors,
            translatedDictorActors,
            ArticlePhraseType.dictor
          );
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      // add first dictor phrases before repetition (article spicific) *---- 1
      const arrFirstDictorPhrase = ArticleUtils
        .GetOnePhraseOrRnd(mixParams.firstDictorPhrases).split(/\r\n|\n|\r/);
          // mixParams.firstDictorPhrases.split(/\r\n|\n|\r/);
      arrFirstDictorPhrase.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(
            newArticle,
            textActors,
            textDictorActors,
            translatedActors,
            translatedDictorActors,
            ArticlePhraseType.dictor
          );
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      // add repetitions phrases
      const repetitionsPhrases = new Array<IArticlePhrase>();
      this.articleByParamData.wordPhrasesToRepeat.forEach((phrase: IWordPhrase) => {
        let phrText = phrase.text;
        const newPhrase = ArticleUtils.getEmptyArticlePhrase(
          newArticle,
          textActors,
          textDictorActors,
          translatedActors,
          translatedDictorActors,
          ArticlePhraseType.speaker
        );

        if (phrText.indexOf('::') > 0) {
          // actor name was specifited
          const arr = phrText.split('::');
          const actorNameStart = arr[0];
          phrText = arr[1].trim();

          const actor = textActors.find((x) =>
            x.name.startsWith(actorNameStart)
          );
          if (actor) {
            newPhrase.articleActor = actor;
            newPhrase.articleActorID = actor.articleActorID;
          }
        }

        newPhrase.text = phrText;
        newPhrase.trText = '';
        repetitionsPhrases.push(newPhrase);
      });

      repetitionsPhrases.forEach((artPhrase: IArticlePhrase) => {
        artPhrase.orderNum = orderNum++;
        newArticle.articlePhrases.push({ ...artPhrase });
      });

      // add dictor phrases before new dialog
      if (mixParams.firstBeforeDialogDictorPhrases && true) {
        // const arrFirstBeforeDialogDictorPhrases =
        //   mixParams.firstBeforeDialogDictorPhrases.split(/\r\n|\n|\r/); // *---- 2

        const arrFirstBeforeDialogDictorPhrases =
          ArticleUtils.GetOnePhraseOrRnd(mixParams.firstBeforeDialogDictorPhrases).split(/\r\n|\n|\r/);
        arrFirstBeforeDialogDictorPhrases.forEach((phrase) => {
          if (phrase && phrase.length > 2) {
            const firstBeforeDialogDictorPhrase =
              ArticleUtils.getEmptyArticlePhrase(
                newArticle,
                textActors,
                textDictorActors,
                translatedActors,
                translatedDictorActors,
                ArticlePhraseType.dictor
              );
            firstBeforeDialogDictorPhrase.trText = phrase;
            firstBeforeDialogDictorPhrase.orderNum = orderNum++;
            newArticle.articlePhrases.push(firstBeforeDialogDictorPhrase);
          }
        });
      }

      // Prepare first presentation phrases from Dialog
      const arrDialogText = this.formGr.get('dialogText').value
        ? this.formGr.get('dialogText').value.split(/\r\n|\n|\r/)
        : [];
      const dialogPhrases = new Array<IArticlePhrase>();
      arrDialogText.forEach((phrase: string) => {
        if (phrase && phrase.length > 2) {
          let phText = phrase;
          const newPhrase = ArticleUtils.getEmptyArticlePhrase(
            newArticle,
            textActors,
            textDictorActors,
            translatedActors,
            translatedDictorActors,
            ArticlePhraseType.speaker
          );

          if (phrase.indexOf('::') > 0) {
            // actor name was specifited
            const arr = phrase.split('::');
            const actorNameStart = arr[0];
            phText = arr[1].trim();

            const actor = textActors.find((x) =>
              x.name.startsWith(actorNameStart)
            );
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

      // Prepare first presentation phrases from lesson phrases
      const lessonPhrases = new Array<IArticlePhrase>();
      if(this.articleByParamData.selectedWords.length < 50){
        arrLessonText.forEach((phrase: string) => {
          if (phrase && phrase.length > 2) {
            let phrText = phrase;
            const newPhrase = ArticleUtils.getEmptyArticlePhrase(
              newArticle,
              textActors,
              textDictorActors,
              translatedActors,
              translatedDictorActors,
              ArticlePhraseType.speaker
            );
  
            if (phrase.indexOf('::') > 0) {
              // actor name was specifited
              const arr = phrase.split('::');
              const actorNameStart = arr[0];
              phrText = arr[1].trim();
  
              const actor = textActors.find((x) =>
                x.name.startsWith(actorNameStart)
              );
              if (actor) {
                newPhrase.articleActor = actor;
                newPhrase.articleActorID = actor.articleActorID;
              }
            }
  
            newPhrase.text = phrText;
            newPhrase.trText = '';
            lessonPhrases.push(newPhrase);
          }
        });
      }  

      if (dialogPhrases.length) {
        // Add first presentation phrases from Dialog
        dialogPhrases.forEach((artPhrase: IArticlePhrase) => {
          artPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push({ ...artPhrase });
        });
      } else {
        // Add first presentation phrases from lesson phrases
        lessonPhrases.forEach((artPhrase: IArticlePhrase) => {
          artPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push({ ...artPhrase });
        });
      }

      // prepare parent phrase for main mix
      const articlePhrase = ArticleUtils.getEmptyArticlePhrase(
        newArticle,
        textActors,
        textDictorActors,
        translatedActors,
        translatedDictorActors
      );
      articlePhrase.text = phraseText;
      articlePhrase.trText = trPhraseText;
      articlePhrase.orderNum = orderNum++;
      articlePhrase.silent = true;

      // add parent phrase for amin list
      newArticle.articlePhrases.push(articlePhrase);
      newArticle.articlePhrases.forEach((x) => {
        if (x.type === ArticlePhraseType.speaker) {
          x.showActor = !(x.articleActor.keyGuid === textActorDefault.keyGuid);
          x.showTrActor = !(
            x.trArticleActor.keyGuid === translatedActorDefault.keyGuid
          );
        } else {
          x.showActor = !(
            x.articleActor.keyGuid === textDictorActorDefault.keyGuid
          );
          x.showTrActor = !(
            x.trArticleActor.keyGuid === translatedDictorActorDefault.keyGuid
          );
        }
      });

      const res = ArticleUtils.mixAndAddToArticle(
        mixParams,
        newArticle,
        articlePhrase,
        textActors,
        textDictorActors,
        translatedActors,
        translatedDictorActors,
        wordsToRepeatMixItems,
        wordPhrasesToRepeatMixItems
      );

      articlePhrase.hasChildren = true;

      // add dictor after end of mixes

      // add before Finish Dictor Phrases (general)
      // const arrbeforeFinishDictorPhrasesGen = this.formGr
      //   .get('beforeFinishDictorPhrases')
      //   .value.split(/\r\n|\n|\r/);
        const arrbeforeFinishDictorPhrasesGen = ArticleUtils
          .GetOnePhraseOrRnd(this.formGr.get('beforeFinishDictorPhrases').value) 
          .split(/\r\n|\n|\r/);


      arrbeforeFinishDictorPhrasesGen.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(
            newArticle,
            textActors,
            textDictorActors,
            translatedActors,
            translatedDictorActors,
            ArticlePhraseType.dictor
          );
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      if (mixParams.beforeFinishDictorPhrases) {
        // add before Finish Dictor Phrases (article spicific)
        // const arrbeforeByOrderMixDictorPhrases =
        //   mixParams.beforeFinishDictorPhrases.split(/\r\n|\n|\r/);

        const arrbeforeByOrderMixDictorPhrases = ArticleUtils
          .GetOnePhraseOrRnd(mixParams.beforeFinishDictorPhrases) 
          .split(/\r\n|\n|\r/);


        arrbeforeByOrderMixDictorPhrases.forEach((phrase) => {
          if (phrase && phrase.length > 2) {
            const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(
              newArticle,
              textActors,
              textDictorActors,
              translatedActors,
              translatedDictorActors,
              ArticlePhraseType.dictor
            );
            firstDictorPhrase.trText = phrase;
            firstDictorPhrase.orderNum = orderNum++;
            newArticle.articlePhrases.push(firstDictorPhrase);
          }
        });
      }

      if (dialogPhrases.length) {
        // Add last presentation phrases from Dialog
        dialogPhrases.forEach((artPhrase: IArticlePhrase) => {
          artPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push({ ...artPhrase });
        });
      } else {
        // Add last presentation phrases from lesson phrases
        lessonPhrases.forEach((artPhrase: IArticlePhrase) => {
          artPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push({ ...artPhrase });
        });
      }

      // add Finish Dictor Phrases (general)
      // const arrFinishDictorPhrasesGen = this.formGr
      //   .get('finishDictorPhrases')
      //   .value.split(/\r\n|\n|\r/);

        const arrFinishDictorPhrasesGen = ArticleUtils
          .GetOnePhraseOrRnd(this.formGr.get('finishDictorPhrases').value) 
          .split(/\r\n|\n|\r/);

      arrFinishDictorPhrasesGen.forEach((phrase) => {
        if (phrase && phrase.length > 2) {
          const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(
            newArticle,
            textActors,
            textDictorActors,
            translatedActors,
            translatedDictorActors,
            ArticlePhraseType.dictor
          );
          firstDictorPhrase.trText = phrase;
          firstDictorPhrase.orderNum = orderNum++;
          newArticle.articlePhrases.push(firstDictorPhrase);
        }
      });

      if (mixParams.finishDictorPhrases) {
        // add Finish Dictor Phrases (article spicific)
        // const arrbeforeByOrderMixDictorPhrases =
        //   mixParams.finishDictorPhrases.split(/\r\n|\n|\r/);

          const arrbeforeByOrderMixDictorPhrases = ArticleUtils
          .GetOnePhraseOrRnd(mixParams.finishDictorPhrases) 
          .split(/\r\n|\n|\r/);

        arrbeforeByOrderMixDictorPhrases.forEach((phrase) => {
          if (phrase && phrase.length > 2) {
            const firstDictorPhrase = ArticleUtils.getEmptyArticlePhrase(
              newArticle,
              textActors,
              textDictorActors,
              translatedActors,
              translatedDictorActors,
              ArticlePhraseType.dictor
            );
            firstDictorPhrase.trText = phrase;
            firstDictorPhrase.orderNum = orderNum++;
            newArticle.articlePhrases.push(firstDictorPhrase);
          }
        });
      }

      // recalculate order
      ArticleUtils.recalculateArticlePhrasesOrder(newArticle);

      if(this.articleByParamData.selectedWords.length >= 50) {
        // test validation article
        newArticle.articlePhrases.forEach((articlePhrase) => {
          articlePhrase.pause = 1;
          articlePhrase.trPause = 1;
        });
      }
  
      articleCnt = articleCnt + 1;

      console.log(`new article ${newArticle.name}`, newArticle);

      // merge voices list
      newArticle.articleActors = [];
      textActors.forEach((x) => newArticle.articleActors.push(x));
      translatedActors.forEach((x) => newArticle.articleActors.push(x));
      textDictorActors.forEach((x) => newArticle.articleActors.push(x));
      translatedDictorActors.forEach((x) => newArticle.articleActors.push(x));

      this.setLoader(true);

      this.articleSave$.next(newArticle);

      mixParams.articlePhraseKeyGuid = articlePhrase.keyGuid;
      mixParams.textHashCode = Utils.getHashCode(articlePhrase.text);
      mixParams.trTextHashCode = Utils.getHashCode(articlePhrase.trText);

      mixParamsSave$.next(mixParams);
    });

    this.articleSave$.complete();
  }

  gatherItems(): IMixItem[][] {
    const mixItems = new Array<IMixItem>();
    let cnt = 0;
    this.articleByParamData.selectedWords.forEach((wordSelected) => {
      mixItems.push({
        mixItemID: 0,
        mixParamID: 0,
        keyGuid: Guid.newGuid().ToString(),
        source: ArticleUtils.SetPhraseHiddenProp(wordSelected.word.text, {
          Pron: wordSelected.word.pronunciation
        } as IPhraseHiddenProperty),
        inDict: '',
        inContext: wordSelected.word.wordTranslation
          ? wordSelected.word.wordTranslation.text
          : '',
        endPhrase: false,
        pretext: false,
        childrenType: '',
        orderNum: cnt++,
        baseWord: true
      } as IMixItem);

      if (wordSelected.phraseWordsSelected) {
        wordSelected.phraseWordsSelected.forEach((wordPhrase) => {
          mixItems.push({
            mixItemID: 0,
            mixParamID: 0,
            keyGuid: Guid.newGuid().ToString(),
            source: wordPhrase.text,
            inDict: '',
            inContext: wordPhrase.wordPhraseTranslation
              ? wordPhrase.wordPhraseTranslation.text
              : '',
            endPhrase: false,
            pretext: false,
            childrenType: '',
            orderNum: cnt++,
            baseWord: false
          } as IMixItem);
        });
      }
    });

    const wordsToRepeatMixItems = new Array<IMixItem>();
    cnt = 0;
    this.articleByParamData.wordsToRepeat.forEach((word) => {
      wordsToRepeatMixItems.push({
        mixItemID: 0,
        mixParamID: 0,
        keyGuid: Guid.newGuid().ToString(),
        source: ArticleUtils.SetPhraseHiddenProp(word.text, {
          Pron: word.pronunciation
        } as IPhraseHiddenProperty),
        inDict: '',
        inContext: word.wordTranslation ? word.wordTranslation.text : '',
        endPhrase: false,
        pretext: false,
        childrenType: '',
        orderNum: cnt++,
        baseWord: true
      } as IMixItem);
    });

    const wordPhrasesToRepeatMixItems = new Array<IMixItem>();
    cnt = 0;
    this.articleByParamData.wordPhrasesToRepeat.forEach((wordPhrase) => {
      wordPhrasesToRepeatMixItems.push({
        mixItemID: 0,
        mixParamID: 0,
        keyGuid: Guid.newGuid().ToString(),
        source: wordPhrase.text,
        inDict: '',
        inContext: wordPhrase.wordPhraseTranslation
          ? wordPhrase.wordPhraseTranslation.text
          : '',
        endPhrase: false,
        pretext: false,
        childrenType: '',
        orderNum: cnt++,
        baseWord: false
      } as IMixItem);
    });

    return [mixItems, wordsToRepeatMixItems, wordPhrasesToRepeatMixItems];
  }

  setLoader(value) {
    this.isLoading = value;
  }

  addMixParam(mixType: number) {
    this.articleByParamData.mixParamsList.push(this.getMixParamsV0());
  }

  deleteMixParamFromList(mixParam) {
    if (window.confirm('Are sure you want to delete this item ?')) {
      this.articleByParamData.mixParamsList =
        this.articleByParamData.mixParamsList.filter(
          (x) => x.articlePhraseKeyGuid !== mixParam.articlePhraseKeyGuid
        );
    }
  }

  moveToTop(mixParam) {
    this.articleByParamData.mixParamsList =
      this.articleByParamData.mixParamsList.filter(
        (x) => x.articlePhraseKeyGuid !== mixParam.articlePhraseKeyGuid
      );

    this.articleByParamData.mixParamsList.unshift(mixParam);
  }

  AddPhrase(wordSelected: IWordSelected) {
    this.openSelectWordPhraseDialog(wordSelected);
  }

  deletePhraseFromList(wordSelected: IWordSelected, wordPhrase: IWordPhrase) {
    if (window.confirm('Are sure you want to delete this item ?')) {
      wordSelected.phraseWordsSelected = wordSelected.phraseWordsSelected.filter(
        (x) => x.wordPhraseID !== wordPhrase.wordPhraseID
      );
      wordPhrase.selected = false;
    }
  }

  openSelectWordPhraseDialog(wordSelected: IWordSelected) {
    console.log('wordSelected:', wordSelected);

    const dialogRef = this.dialog.open(SelectWordPhraseDialogComponent, {
      disableClose: true,
      width: '800px',
      data: {
        selectionVisibile: true,
        wordSelected
      }
    });

    dialogRef
      .afterClosed()
      .toPromise()
      .then((dialogResult: Array<IWordPhrase>) => {
        console.log('dialogResult:', dialogResult);

        if (dialogResult === null) {
          return;
        }

        if (!wordSelected.phraseWordsSelected) {
          wordSelected.phraseWordsSelected = Array<IWordPhrase>();
        }

        dialogResult.forEach((x) => {
          if (
            !wordSelected.phraseWordsSelected.some(
              (e) => e.wordPhraseID === x.wordPhraseID
            )
          ) {
            wordSelected.phraseWordsSelected.push(x);
          }
        });

        console.log(
          'wordSelected.phraseWordsSelected:',
          wordSelected.phraseWordsSelected
        );
      })
      .finally(() => {
        this.setLoader(false);
      });
  }

  addRowsToList(words: Array<IWord>) {
    words.forEach(word => {
      if (
        this.articleByParamData.selectedWords.some(
          (e) => e.word.wordID === word.wordID
        )
      ) {
        alert('This element already selected: ' + word.text);
        return;
      }
      
      let sortedPhrases = word.wordPhraseSelected.sort((a, b) => {
        return (a.phraseOrderInCurrentWord ?? 0) - (b.phraseOrderInCurrentWord ?? 0)
      });

      let sortedPhrases1 = sortedPhrases.reverse();
      const wordSelected = {
        word,
        phraseWords: [],
        phraseWordsSelected: sortedPhrases1.slice(0,3),
      } as IWordSelected;
  
      this.articleByParamData.selectedWords.push(wordSelected);
      word.selected = true;
      });
  }

  addRowToList(word: IWord) {
    if (
      this.articleByParamData.selectedWords.some(
        (e) => e.word.wordID === word.wordID
      )
    ) {
      alert('This element already selected');
      return;
    }

    const wordSelected = {
      word,
      phraseWords: []
    } as IWordSelected;

    this.articleByParamData.selectedWords.push(wordSelected);
    word.selected = true;
  }

  editWordPhraseTranslation(wordPhrase: IWordPhrase) {
    console.log('editWordPhraseTranslation:', wordPhrase);

    const dialogRef = this.dialog.open(UpdateWordphraseTranslationDialogComponent, {
      disableClose: true,
      width: '600px',
      data: {
        itemData: {
          ...wordPhrase,
          wordPhraseTranslation: wordPhrase.wordPhraseTranslation
            ? {
              ...wordPhrase.wordPhraseTranslation,
              wordPhrase: { ...wordPhrase, wordPhraseTranslation: null }
            }
            : ({
              wordPhraseTranslationID: 0,
              wordPhraseID: wordPhrase.wordPhraseID,
              languageID:
                this.appSettingsService.getCurrent().NativeLanguage
                  .languageID,
              text: '',
              wordPhrase: { ...wordPhrase, wordPhraseTranslation: null }
            } as IWordPhraseTranslation)
        }
      }
    });

    dialogRef
      .afterClosed()
      .toPromise()
      .then((dialogResult: IWordPhrase) => {
        console.log('dialogResult:', dialogResult);

        if (dialogResult == null) {
          return;
        }

        this.wordService
          .updateWordPhraseTranslation(dialogResult.wordPhraseTranslation)
          .toPromise()
          .then((result: IWordPhraseTranslation) => {
            wordPhrase.wordPhraseTranslation = result;
          });
      })
      .finally(() => {
        this.setLoader(false);
      });
  }

  editWordTranslation(word: IWord) {
    console.log('editWordTranslation:', word);

    const dialogRef = this.dialog.open(UpdateWordTranslationDialogComponent, {
      disableClose: true,
      width: '600px',
      data: {
        itemData: {
          ...word,
          wordTranslation: word.wordTranslation
            ? { ...word.wordTranslation }
            : ({
              wordTranslationID: 0,
              wordID: word.wordID,
              languageID:
                this.appSettingsService.getCurrent().NativeLanguage
                  .languageID,
              text: ''
            } as IWordTranslation)
        }
      }
    });

    dialogRef
      .afterClosed()
      .toPromise()
      .then((dialogResult: IWord) => {
        console.log('dialogResult:', dialogResult);

        if (dialogResult == null) {
          return;
        }

        this.wordService
          .updateWordTranslation(dialogResult.wordTranslation)
          .toPromise()
          .then((result: any) => {
            word.wordTranslation = dialogResult.wordTranslation;
          });
      })
      .finally(() => {
        this.setLoader(false);
      });
  }

  translateAllSelectedPhrases() {
    this.setLoader(true);

    const translate$ = new Subject<IWordPhraseTranslation>();
    translate$
      .pipe(
        takeUntil(this.ngUnsubscribe$),
        concatMap((wordPhraseTranslation: IWordPhraseTranslation) => {
          return this.wordService.updateWordPhraseTranslation(
            wordPhraseTranslation
          );
        })
      )
      .subscribe(
        (result: IWordPhraseTranslation) => {
          console.log('wordPhraseTranslation:', result);
          this.articleByParamData.selectedWords.forEach((word) => {
            if (word.phraseWordsSelected) {
              word.phraseWordsSelected.forEach((wordPhrase) => {
                if (wordPhrase.wordPhraseID === result.wordPhraseID) {
                  // wordPhrase = {...wordPhrase, wordPhraseTranslation: result};
                  wordPhrase.wordPhraseTranslation = result;
                  console.log('replace transl', wordPhrase);
                }
              });
            }
          });
        },
        (error) => {
          console.log('wordPhraseTranslation error:', error);
        },
        () => {
          this.setLoader(false);
          console.log('wordPhraseTranslation complited');
        }
      );

    this.articleByParamData.selectedWords.forEach((word) => {
      if (word.phraseWordsSelected) {
        word.phraseWordsSelected.forEach((wordPhrase) => {
          if (
            !wordPhrase.wordPhraseTranslation ||
            !wordPhrase.wordPhraseTranslation.text
          ) {
            const ph = {
              wordPhraseTranslationID: 0,
              wordPhraseID: wordPhrase.wordPhraseID,
              languageID:
                this.appSettingsService.getCurrent().NativeLanguage
                  .languageID,
              text: '',
              wordPhrase: { ...wordPhrase, wordPhraseTranslation: null }
            } as IWordPhraseTranslation;

            console.log('need to translate', ph);
            translate$.next(ph);
          }
        });
      }
    });

    translate$.complete();
  }

  cutText(text: string, len: number = 40) {
    let result: string;

    if (!text || text.length <= len) {
      return text;
    }

    result = text.substring(0, len) + ' ...';
    return result;
  }

  deleteRowFromList(wordSelected: IWordSelected) {
    if (window.confirm('Are sure you want to delete this item ?')) {
      this.articleByParamData.selectedWords =
        this.articleByParamData.selectedWords.filter(
          (e) => e.word.wordID !== wordSelected.word.wordID
        );
      wordSelected.word.selected = false;
    }
  }

  editRow(word: IWord) {
    console.log(word);
  }

  delete(id) {
    if (window.confirm('Are sure you want to delete this item ?')) {
      this.wordService
        .Delete(id)
        .pipe(takeUntil(this.ngUnsubscribe$))
        .subscribe((result) => {
          this.toasterService.success('Word delete', 'deleted successful');
          this.refreshPage.emit(1);
        });
    }
  }

  // add3StepMixParam() {
  //   this.addStep1();
  //   this.addStep2();
  //   this.addStep3();
  // }

  // addStep1() {
  //   this.articleByParamData.mixParamsList.push(this.getMixParamsV1());
  // }

  // addStep2() {
  //   this.articleByParamData.mixParamsList.push(this.getMixParamsV2());
  // }

  // addStep3() {
  //   this.articleByParamData.mixParamsList.push(this.getMixParamsV3());
  // }

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
      trRepeat: 0
    };
  }

  // getMixParamsV1() {
  //   const item = this.getEmptyMixParams();
  //   return {
  //     ...item,
  //     name: 'template 1',
  //     active: true,
  //     repeatOrder: 3,
  //     addSlow2InRepeatOrder: true,
  //     addSlowInRepeatOrder: true,
  //     repeatBaseWord: 0,
  //     repeat: 0,
  //     trActive: false,
  //     trRepeatOrder: 0,
  //     trAddSlow2InRepeatOrder: false,
  //     trAddSlowInRepeatOrder: false,
  //     trRepeatBaseWord: 0,
  //     trRepeat: 0,
  //   };
  // }

  // getMixParamsV2() {
  //   const item = this.getEmptyMixParams();
  //   return {
  //     ...item,
  //     name: 'template 2',
  //     active: true,
  //     repeatOrder: 0,
  //     addSlow2InRepeatOrder: false,
  //     addSlowInRepeatOrder: false,
  //     repeatBaseWord: 10,
  //     repeat: 0,
  //     trActive: true,
  //     trRepeatOrder: 0,
  //     trAddSlow2InRepeatOrder: false,
  //     trAddSlowInRepeatOrder: false,
  //     trRepeatBaseWord: 20,
  //     trRepeat: 0,
  //   };
  // }

  // getMixParamsV3() {
  //   const item = this.getEmptyMixParams();
  //   return {
  //     ...item,
  //     name: 'template 3',
  //     active: true,
  //     repeatOrder: 0,
  //     addSlow2InRepeatOrder: false,
  //     addSlowInRepeatOrder: false,
  //     repeatBaseWord: 0,
  //     repeat: 0,
  //     trActive: true,
  //     trRepeatOrder: 0,
  //     trAddSlow2InRepeatOrder: false,
  //     trAddSlowInRepeatOrder: false,
  //     trRepeatBaseWord: 0,
  //     trRepeat: 12,
  //   };
  // }

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
      trRepeatOrder: 0
    } as IMixParams;
  }

  changeTemplateModal() {
    const jsonStr = this.getTemplateAsJson();
    const dialogRef = this.dialog.open(UpdateTemplateJsonDialogComponent, {
      disableClose: true,
      width: '800px',
      data: jsonStr
    });

    dialogRef
      .afterClosed()
      .toPromise()
      .then((dialogResult: ISearchListArg) => {
        if (dialogResult) {
          try {
            this.articleByParamData = JSON.parse(dialogResult.str1);
          } catch (error) { }
        }
      });
  }

  addRepeatWord() {
    const dialogRef = this.dialog.open(SelectRepeatWordsPhrasesDialogComponent, {
      disableClose: true,
      width: '800px',
      data: {
        selectedWords: [
          ...(this.articleByParamData.wordsToRepeat
            ? this.articleByParamData.wordsToRepeat
            : new Array<IWord>())
        ],
        selectedPhrases: [
          ...(this.articleByParamData.wordPhrasesToRepeat
            ? this.articleByParamData.wordPhrasesToRepeat
            : new Array<IWordPhrase>())
        ],
        baseName: this.articleByParam.name
      }
    });

    dialogRef
      .afterClosed()
      .toPromise()
      .then((dialogResult) => {
        console.log('addRepeatWord dialogResult', dialogResult);
        if (dialogResult) {
          this.articleByParamData.wordsToRepeat = [
            ...dialogResult.selectedWords
          ];
          this.articleByParamData.wordPhrasesToRepeat = [
            ...dialogResult.selectedPhrases
          ];
        }
      });
  }

  generateRepeatWord() {
    if (!this.formGr.get('baseName').value) {
      alert('Please enter "BASE NAME"');
      return;
    }

    const baseName = this.formGr.get('baseName').value;
    this.wordService
      .getListForRepetition({
        parentID: this.articleByParam.articleByParamID,
        str1: this.formGr.get('baseName').value,
        int1: this.formGr.get('maxWordsToRepeatForGeneration').value
      })
      .toPromise()
      .then((result: IWord[]) => {
        this.articleByParamData.wordsToRepeat = [];
        this.articleByParamData.wordPhrasesToRepeat = [];
        result.forEach((x) => {
          this.articleByParamData.wordsToRepeat.push(x);
          if (x.wordPhraseSelected?.length > 0) {
            console.log('add ', x.wordPhraseSelected[0]);
            this.articleByParamData.wordPhrasesToRepeat.push(
              x.wordPhraseSelected[0]
            );
          }
        });
      });
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
