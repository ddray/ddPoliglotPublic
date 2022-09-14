import { Component, OnInit, OnDestroy, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { FormsModule, FormGroup, Validators, FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { TranslateService } from '../../shared/services/translate.service';
import { Guid, Utils } from '../../shared/models/System';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { ArticleService } from '../../shared/services/article.service';
import { IArticle } from '../../shared/interfaces/IArticle';
import { switchMap, startWith, map, catchError, takeUntil, concatMap } from 'rxjs/operators';
import { merge, Observable, of as observableOf, Subject } from 'rxjs';
import { IArticlePhrase } from '../../shared/interfaces/IArticlePhrase';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../../environments/environment';
import { NavMenuService } from '../../shared/services/navmenu.service';
import { navCommandsOutput, navCommandsInput } from '../../shared/interfaces/INavCommands';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import { SplitPhraseDialogComponent } from '../../shared/components/split-phrase-dialog/split-phrase-dialog.component';
import { IMixItem, IMixParams } from '../../shared/interfaces/IMixItem';
import { IArticleActor } from '../../shared/interfaces/IArticleActor';
import { ArticleActorRole, ArticlePhraseType, ArticlePhraseActivityType, PhrasesMixType, PhrasesSplitType } from '../../shared/enums/Enums';
import { ArticleActorDialogComponent } from '../../shared/components/article-actor-dialog/article-actor-dialog.component';
import { SelectActorsDialogComponent } from '../../shared/components/select-actors-dialog/select-actors-dialog.component';
import { AuthorizeService, IUser } from '../../../api-authorization/authorize.service';
import { VideoPlayerDialogComponent } from '../../shared/components/video-player-dialog/video-player-dialog.component';
import { IPrepareMixItem } from '../../shared/interfaces/IPrepareMixItem';
import { GlobalService } from 'src/app/shared/services/global.service';
import { ILanguage } from 'src/app/shared/interfaces/ILanguage';
import { ArticleUtils } from 'src/app/shared/models/ArticleUtils';
import { AppSettingsService } from '../../shared/services/app-settings.service';
import { IAppSettings } from '../../shared/interfaces/IAppSettings';
import { IWorkJobsStateEntity } from '../../shared/interfaces/IWorkJobsStateEntity';

@Component({
  selector: 'app-article',
  templateUrl: './article.component.html',
  styleUrls: ['./article.component.scss']
})
export class ArticleComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('componentRootElement', { static: false }) componentRootElement: ElementRef;
  @ViewChild('textActorsTable', { static: false }) textActorsTable: MatTable<any>;
  @ViewChild('translatedActorsTable', { static: false }) translatedActorsTable: MatTable<any>;
  @ViewChild('textDictorActorsTable', { static: false }) textDictorActorsTable: MatTable<any>;
  @ViewChild('translatedDictorActorsTable', { static: false }) translatedDictorActorsTable: MatTable<any>;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private articleService: ArticleService,
    private translateService: TranslateService,
    private toasterService: ToastrService,
    private navMenuService: NavMenuService,
    private dialog: MatDialog,
    private authorizeService: AuthorizeService,
    private globalService: GlobalService,
    private appSettingsService: AppSettingsService,
  ) { }

  private ngUnsubscribe$ = new Subject();
  private currentUser: IUser;

  public dataItem: IArticle;
  public isLoadingResults = true;
  public isRateLimitReached = false;
  public isChanged = false;
  public showToolbox = false;
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;

  public textActors: IArticleActor[] = [];
  public translatedActors: IArticleActor[] = [];
  public textDictorActors: IArticleActor[] = [];
  public translatedDictorActors: IArticleActor[] = [];

  public textActorDefaultName: string;
  public translatedActorDefaultName: string;
  public textDictorActorDefaultName: string;
  public translatedDictorActorDefaultName: string;

  public actorColumns: string[] = ['name', 'defaultInRole', 'voiceName', 'act'];

  private restoreScrollPosition = false;
  private scrollPosition = 0;
  public loaderHeight = '100px';
  private changeLoaderSize = false;
  public languages: Array<ILanguage>;

  private makeAudioArticleIntervalRef;
  private makeVideoArticleIntervalRef;

  async ngOnInit() {
    this.route.paramMap
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe((params: ParamMap) => {
        this.authorizeService.getCurrentUser()
          .pipe(takeUntil(this.ngUnsubscribe$))
          .subscribe((u) => {
            this.currentUser = u;
            this.getData(params.get('id'));
          });
      });

    this.languages = await this.globalService.getLanguages();

    this.navMenuService.navOutput
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe(data => {
        if (!data) {
          return;
        }

        if (data.command === navCommandsOutput.Article_Save_Click) {
          this.Save();
        } else if (data.command === navCommandsOutput.Article_ShowPhraseToolbox_Click) {
          this.showToolbox = data.payload;
        } else if (data.command === navCommandsOutput.Article_MakeAudioFile_Click) {
          this.makeAudio();
        } else if (data.command === navCommandsOutput.Article_MakeVideoFile_Click) {
          this.makeVideo();
        } else if (data.command === navCommandsOutput.Article_OpenVideoDialog_Click) {
          this.showVideoPlayerDialog();
        } else if (data.command === navCommandsOutput.Article_RecalculatePauses_Click) {
          this.RecalculatePauses();
        } else if (data.command === navCommandsOutput.Article_CollapseAll_Click) {
          this.collapseAll();
        }

      });
  }

  getData(id) {
    this.setLoader(true);
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_All_Disabled, payload: true });
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_Save_Loader, payload: false });
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeAudioFile_Loader, payload: false });

    if (id > 0) {
      let selectedKeys: Array<string> = [];
      let childrenClosedKeys: Array<string> = [];

      if (this.dataItem && this.dataItem.articlePhrases.length > 0) {
        selectedKeys = this.dataItem.articlePhrases.filter(x => x.selected).map(y => y.keyGuid);
        childrenClosedKeys = this.dataItem.articlePhrases.filter(x => x.childrenClosed).map(y => y.keyGuid);
      }

      this.articleService.getById(id)
        .toPromise()
        .then(result => {
          result.articlePhrases = result.articlePhrases.sort((x, y) => x.orderNum - y.orderNum);

          // restore selected
          result.articlePhrases.forEach(x => { x.selected = selectedKeys.some(y => y === x.keyGuid); });

          // restore children show/hide
          result.articlePhrases.forEach(x => { x.childrenClosed = childrenClosedKeys.some(y => y === x.keyGuid); });
          result.articlePhrases.forEach(x => { x.hidden = childrenClosedKeys.some(y => y === x.parentKeyGuid); });

          const el = document.getElementById('rootSidenav');
          this.scrollPosition = el.scrollTop;
          this.restoreScrollPosition = true;

          this.dataItem = result;
          this.splitVoicesByRoles();
          this.setActorsLabels();

          this.SetChanged(false);

          // refresh audio name in main menu
          this.navMenuService.navInput
            .next({ command: navCommandsInput.Article_AudioFile_Value, payload: this.dataItem.textSpeechFileName });
          this.navMenuService.navInput
            .next({ command: navCommandsInput.Article_VideoFile_Value, payload: this.dataItem.videoFileName });
        }).catch(err => {
          console.log('err: ', err);
        }).finally(() => {
          this.setLoader(false);
          this.navMenuService.navInput.next({ command: navCommandsInput.Article_All_Disabled, payload: false });
        });
    } else {
      // new article
      const currAppSettings = this.appSettingsService.getCurrent();

      const newArticle: IArticle = {
        articleID: 0,
        userID: this.currentUser.id,
        articlePhrases: [],
        articleActors: [],
        language: currAppSettings.LearnLanguage.code,
        languageTranslation: currAppSettings.NativeLanguage.code,
        name: '',
        textHashCode: '',
        textSpeechFileName: '',
        videoFileName: ''
      };

      this.dataItem = newArticle;
      this.dataItem.articlePhrases = [];
      this.fillActorsByLanguage();
      this.setLoader(false);
      this.navMenuService.navInput.next({ command: navCommandsInput.Article_All_Disabled, payload: false });
      // refresh audio name in main menu
      this.navMenuService.navInput.next({ command: navCommandsInput.Article_AudioFile_Value, payload: this.dataItem.textSpeechFileName });
      this.navMenuService.navInput.next({ command: navCommandsInput.Article_VideoFile_Value, payload: this.dataItem.videoFileName });
      this.SetChanged(true);
    }
  }

  setActorsLabels() {
    const textActorDefault = this.textActors.find(x => x.defaultInRole);
    const translatedActorDefault = this.translatedActors.find(x => x.defaultInRole);
    const textDictorActorDefault = this.textDictorActors.find(x => x.defaultInRole);
    const translatedDictorActorDefault = this.translatedDictorActors.find(x => x.defaultInRole);

    this.textActorDefaultName = ArticleUtils.getActorLabel(textActorDefault);
    this.translatedActorDefaultName = ArticleUtils.getActorLabel(translatedActorDefault);
    this.textDictorActorDefaultName = ArticleUtils.getActorLabel(textDictorActorDefault);
    this.translatedDictorActorDefaultName = ArticleUtils.getActorLabel(translatedDictorActorDefault);

    this.dataItem.articlePhrases.forEach((x) => {
      if (x.type === ArticlePhraseType.speaker) {
        x.showActor = !(x.articleActor.keyGuid === textActorDefault.keyGuid);
        x.showTrActor = !(x.trArticleActor.keyGuid === translatedActorDefault.keyGuid);
      } else {
        x.showActor = !(x.articleActor.keyGuid === textDictorActorDefault.keyGuid);
        x.showTrActor = !(x.trArticleActor.keyGuid === translatedDictorActorDefault.keyGuid);
      }
    });
  }

  setShowActorInPhrase() {
  }

  ngAfterViewChecked() {
    if (this.restoreScrollPosition) {
      this.restoreScrollPosition = false;
      const el = document.getElementById('rootSidenav');
      el.scrollTop = this.scrollPosition;
    }

    if (this.changeLoaderSize) {
      this.changeLoaderSize = false;
      if (this.componentRootElement) {
        setTimeout(() => {
          this.loaderHeight = Math.round((this.componentRootElement.nativeElement as HTMLElement).getBoundingClientRect().height) + 'px';
        }, 0);
      }
    }
  }

  ngAfterViewInit() {
  }

  learnLanguageChanged(event) {
    this.fillActorsByLanguage();
  }

  nativeLanguageChanged(event) {
    this.fillActorsByLanguage();
  }

  changeRowActors(articlePhrase: IArticlePhrase) {
    this.setLoader(true);
    const index = this.dataItem.articlePhrases.findIndex(x => x.keyGuid === articlePhrase.keyGuid);

    if (index >= 0) {
      const dialogRef = this.dialog.open(
        SelectActorsDialogComponent,
        {
          disableClose: true,
          width: '600px',
          data:
          {
            actor: articlePhrase.articleActor,
            trActor: articlePhrase.trArticleActor,
            actors: articlePhrase.type === ArticlePhraseType.speaker
              ? this.textActors
              : this.textDictorActors,
            trActors: articlePhrase.type === ArticlePhraseType.speaker
              ? this.translatedActors
              : this.translatedDictorActors
          }
        }
      );

      dialogRef.afterClosed().toPromise()
        .then((result) => {
          if (result === null) { return; }
          let flag = false;
          if (articlePhrase.articleActor.keyGuid !== result.key) {
            const actors = articlePhrase.type === ArticlePhraseType.speaker
              ? this.textActors
              : this.textDictorActors;
            const actor = actors.find(x => x.keyGuid === result.key);
            articlePhrase.articleActor = actor;
            articlePhrase.articleActorID = actor.articleActorID;
            articlePhrase.pause = 1;
            articlePhrase.speachDuration = 0;
            articlePhrase.textSpeechFileName = '';
            flag = true;
          }

          if (articlePhrase.trArticleActor.keyGuid !== result.trKey) {
            const actors = articlePhrase.type === ArticlePhraseType.speaker
              ? this.translatedActors
              : this.translatedDictorActors;
            const actor = actors.find(x => x.keyGuid === result.trKey);
            articlePhrase.trArticleActor = actor;
            articlePhrase.trArticleActorID = actor.articleActorID;
            articlePhrase.trPause = 1;
            articlePhrase.trSpeachDuration = 0;
            articlePhrase.trTextSpeechFileName = '';
            flag = true;
          }

          if (flag) {
            this.setActorsLabels();
            this.SetChanged(true);
          }
        }).finally(() => {
          this.setLoader(false);
        });

    } else {
      this.setLoader(false);
    }
  }

  splitByPhrasesRow(articlePhrase: IArticlePhrase) {
    this.setLoader(true);
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index >= 0) {
      const arPhrases = this.splitByDotAndComa(articlePhrase.text);
      this.translateService.translateArray({ source: arPhrases, sourceLanguage: this.dataItem.language,
        targetLanguage: this.dataItem.languageTranslation })
        .toPromise()
        .then((resultTranslation) => {
          console.log('translateArray:', resultTranslation);
          let cnt = 1;
          arPhrases.forEach(str => {
            const newItem = this.getEmptyArticlePhrase();
            newItem.text = str;
            newItem.trText = resultTranslation[cnt - 1];
            newItem.activityType = articlePhrase.activityType;
            newItem.pause = 1;
            newItem.trPause = 1;
            this.dataItem.articlePhrases.splice(index + cnt++, 0, newItem);
          });

          ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
          this.SetChanged(true);
        }).finally(() => {
          this.setLoader(false);
        });
    } else {
      this.setLoader(false);
    }
  }

  splitForRandomGenerationRow(articlePhrase: IArticlePhrase) {
    this.setLoader(true);
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index >= 0) {
      const dialogRef = this.dialog.open(
        SplitPhraseDialogComponent,
        {
          disableClose: true,
          width: '1000px',
          data:
          {
            articlePhrase,
            phrasesMixType: PhrasesMixType.random,
            phrasesSplitType: PhrasesSplitType.NL,
          }
        }
      );

      dialogRef.afterClosed().toPromise()
        .then((splitDialogResult: IMixParams) => {
          if (splitDialogResult === null) { return; }

          const mixItems = ArticleUtils.mixAndAddToArticle(splitDialogResult, this.dataItem, articlePhrase,
            this.textActors, this.textDictorActors, this.translatedActors, this.translatedDictorActors);

          if (mixItems.length > 0) {
            articlePhrase.hasChildren = true;
          }

          ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
          this.SetChanged(true);
        }).finally(() => {
          this.setLoader(false);
        });

    } else {
      this.setLoader(false);
    }
  }

  splitBySubPhrasesRow(articlePhrase: IArticlePhrase) {
    this.setLoader(true);
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);

    if (index >= 0) {
      const dialogRef = this.dialog.open(
        SplitPhraseDialogComponent,
        {
          disableClose: true,
          width: '800px',
          data:
          {
            articlePhrase,
            phrasesMixType: PhrasesMixType.wave,
            phrasesSplitType: PhrasesSplitType.space,
          }
        }
      );

      dialogRef.afterClosed().toPromise()
        .then((splitDialogResult: IMixParams) => {
          if (splitDialogResult === null) { return; }

          const resultArr = splitDialogResult.mixItems.filter((x) => x.source.length > 0);

          const newItems: Array<IPrepareMixItem>
            = this.mixPhrasesWaveMethod(resultArr
              , splitDialogResult.active
              , splitDialogResult.m0
              , splitDialogResult.m01
              , splitDialogResult.m012
              , splitDialogResult.m0123
              , splitDialogResult.m0_repeat
              , splitDialogResult.m01_repeat
              , splitDialogResult.m012_repeat
              , splitDialogResult.m0123_repeat
              , splitDialogResult.offer1
              , splitDialogResult.offer2
              , splitDialogResult.offer3
              , splitDialogResult.offer4
              , splitDialogResult.offer5
            );

          const trNewItems: Array<IPrepareMixItem>
            = this.mixPhrasesWaveMethod(resultArr
              , splitDialogResult.trActive
              , splitDialogResult.trM0
              , splitDialogResult.trM01
              , splitDialogResult.trM012
              , splitDialogResult.trM0123
              , splitDialogResult.trM0_repeat
              , splitDialogResult.trM01_repeat
              , splitDialogResult.trM012_repeat
              , splitDialogResult.trM0123_repeat
              , splitDialogResult.trOffer1
              , splitDialogResult.trOffer2
              , splitDialogResult.trOffer3
              , splitDialogResult.trOffer4
              , splitDialogResult.trOffer5
            );

          if (splitDialogResult.trFirst) {
            ArticleUtils.addMixedToArticle(trNewItems, index, 1,
              this.dataItem, articlePhrase, this.textActors, this.textDictorActors, this.translatedActors, this.translatedDictorActors, '', '');
            ArticleUtils.addMixedToArticle(newItems, index + trNewItems.length, 0,
              this.dataItem, articlePhrase, this.textActors, this.textDictorActors, this.translatedActors, this.translatedDictorActors, '', '');
          } else {
            ArticleUtils.addMixedToArticle(newItems, index, 0,
              this.dataItem, articlePhrase, this.textActors, this.textDictorActors, this.translatedActors, this.translatedDictorActors, '', '');
            ArticleUtils.addMixedToArticle(trNewItems, index + newItems.length, 1,
              this.dataItem, articlePhrase, this.textActors, this.textDictorActors, this.translatedActors, this.translatedDictorActors, '', '');
          }

          if (resultArr.length > 0) {
            articlePhrase.hasChildren = true;
          }

          ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
          this.SetChanged(true);
        }).finally(() => {
          this.setLoader(false);
        });

    } else {
      this.setLoader(false);
    }
  }

  mixPhrasesWaveMethod(
    resultArr: Array<IMixItem>,
    active: boolean,
    m0: boolean,
    m01: boolean,
    m012: boolean,
    m0123: boolean,
    m0_repeat: boolean,
    m01_repeat: boolean,
    m012_repeat: boolean,
    m0123_repeat: boolean,
    dictorText1: string,
    dictorText2: string,
    dictorText3: string,
    dictorText4: string,
    dictorText5: string,
  ) {

    const newItems: Array<IPrepareMixItem> = [];
    if (active) {
      for (let i = 0; i < resultArr.length; i++) {
        if (resultArr[i].source && resultArr[i].source.length > 0) {
          // 0
          if (i < resultArr.length && m0) {
            newItems.push({ source: resultArr[i].source, translated: this.getDict(resultArr[i]), childrenType: '0', type: 0 });
            if (m0_repeat) {
              newItems.push({ source: resultArr[i].source, translated: '', childrenType: 'r0', type: 0 });
            }
          }
          // 1
          // 0 1
          if (i + 1 < resultArr.length
            && !resultArr[i].endPhrase
            && m01
            && !resultArr[i + 1].pretext // if pretext it cannot be last (and or with ...)
          ) {
            newItems.push({ source: resultArr[i].source + ' ' + resultArr[i + 1].source,
            translated: this.getContext(resultArr[i]) + ' ' + this.getContext(resultArr[i + 1]), childrenType: '01', type: 0 });
            if (m01_repeat) {
              newItems.push({ source: resultArr[i].source + ' ' + resultArr[i + 1].source, translated: '', childrenType: 'r01', type: 0 });
            }
          }
          // 2
          // 0 1 2
          if (i + 2 < resultArr.length
            && !resultArr[i].endPhrase
            && !resultArr[i + 1].endPhrase
            && m012
            && !resultArr[i + 2].pretext
          ) {
            newItems.push({ source: resultArr[i].source + ' ' + resultArr[i + 1].source + ' ' + resultArr[i + 2].source,
            translated: this.getContext(resultArr[i]) + ' ' + this.getContext(resultArr[i + 1]) + ' ' + this.getContext(resultArr[i + 2]),
            childrenType: '012', type: 0 });
            if (m012_repeat) {
              newItems.push({ source: resultArr[i].source + ' ' + resultArr[i + 1].source + ' ' + resultArr[i + 2].source,
              translated: '', childrenType: 'r012', type: 0 });
            }
          }
          // 0 1 2 3
          if (i + 3 < resultArr.length
            && !resultArr[i].endPhrase
            && !resultArr[i + 1].endPhrase
            && !resultArr[i + 2].endPhrase
            && m0123
            && !resultArr[i + 3].pretext
          ) {
            newItems.push({
              source: resultArr[i].source + ' ' + resultArr[i + 1].source + ' ' + resultArr[i + 2].source + ' ' + resultArr[i + 3].source,
              translated: this.getContext(resultArr[i]) + ' ' + this.getContext(resultArr[i + 1]) + ' '
              + this.getContext(resultArr[i + 2]) + ' ' + this.getContext(resultArr[i + 3]),
            childrenType: '0123', type: 0 });
            if (m0123_repeat) {
              newItems.push({ source: resultArr[i].source + ' ' + resultArr[i + 1].source + ' ' + resultArr[i + 2].source + ' ' +
              resultArr[i + 3].source, translated: '', childrenType: 'r0123', type: 0 });
            }
          }
        }
      }

      return newItems;
    }

    // add dictor speaches
    const dSpeaches: Array<string> = [dictorText1, dictorText2, dictorText3, dictorText4, dictorText5]
      .filter(x => x);

    if (dSpeaches.length > 0) {
      // add first in begining
      newItems.splice(0, 0, { source: '', translated: dSpeaches[0], childrenType: 'dictor0', type: 1 });
    }

    if (dSpeaches.length > 1) {
      // 2 -> 3 - 6
      const ind = Utils.getRandomInt(3, 6);
      if (newItems.length > (ind + 3)) {
        newItems.splice(ind, 0, { source: '', translated: dSpeaches[1], childrenType: 'dictor0', type: 1 });
      }
    }

    if (dSpeaches.length > 2) {
      // 3 -> 10 - 15
      const ind = Utils.getRandomInt(10, 15);
      if (newItems.length > (ind + 3)) {
        newItems.splice(ind, 0, { source: '', translated: dSpeaches[2], childrenType: 'dictor0', type: 1 });
      }
    }

    if (dSpeaches.length > 3) {
      // 4 -> 20 - 30
      const ind = Utils.getRandomInt(20, 30);
      if (newItems.length > (ind + 3)) {
        newItems.splice(ind, 0, { source: '', translated: dSpeaches[3], childrenType: 'dictor0', type: 1 });
      }
    }

    if (dSpeaches.length > 4) {
      // 5 -> 45 - 60
      const ind = Utils.getRandomInt(45, 60);
      if (newItems.length > (ind + 3)) {
        newItems.splice(ind, 0, { source: '', translated: dSpeaches[4], childrenType: 'dictor0', type: 1 });
      }
    }

    return newItems;
  }

  getContext(splitPhraseItem: IMixItem) {
    return splitPhraseItem.inContext.length > 0 ? splitPhraseItem.inContext : splitPhraseItem.inDict;
  }

  getDict(splitPhraseItem: IMixItem) {
    return splitPhraseItem.inDict.length > 0 ? splitPhraseItem.inDict : splitPhraseItem.inContext;
  }

  splitByDotAndComa(text: string): Array<string> {
    const ar = text.replace(/([.?!])\s*(?=[A-Z])/g, '$1|').replace(/,/g, '|').split('|');
    return ar;
  }

  textToSpeechRow(articlePhrase: IArticlePhrase) {
    this.setLoader(true);
    this.articleService.textToSpeachArticlePhrase(articlePhrase.articlePhraseID)
      .toPromise()
      .then(result => {
        this.getData(this.dataItem.articleID);
      })
      .catch(er => {
        this.setLoader(false);
      });
  }

  insertRow() {
    this.dataItem.articlePhrases.splice(0, 0, this.getEmptyArticlePhrase());
    ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
    this.SetChanged(true);
  }

  insertDictorRow() {
    this.dataItem.articlePhrases.splice(0, 0, this.getEmptyArticlePhrase(1));
    ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
    this.SetChanged(true);
  }

  insertRowBefore(articlePhrase: IArticlePhrase) {
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index >= 0) {
      this.dataItem.articlePhrases.splice(index, 0, this.getEmptyArticlePhrase(0, articlePhrase));
      ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
      this.SetChanged(true);
    }
  }

  insertRowAfter(articlePhrase: IArticlePhrase) {
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index >= 0) {
      const emptyArticlePhrase = this.getEmptyArticlePhrase(0, articlePhrase);
      if (!articlePhrase.hasChildren) {
        this.dataItem.articlePhrases.splice(index + 1, 0, emptyArticlePhrase);
      } else {
        // find last children and insert ufter that
        const children = this.dataItem.articlePhrases
          .filter(x => x.parentKeyGuid === articlePhrase.keyGuid);
        if (children.length === 0) {
          // no children
          this.dataItem.articlePhrases.splice(index + 1, 0, emptyArticlePhrase);
        } else {
          const max = Math.max.apply(Math, children.map((o) => o.orderNum));
          const indexCh = this.dataItem.articlePhrases.findIndex((x) => x.parentKeyGuid === articlePhrase.keyGuid && x.orderNum === max);
          this.dataItem.articlePhrases.splice(indexCh + 1, 0, emptyArticlePhrase);
        }
      }
      ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
      this.SetChanged(true);
    }
  }

  addDictorSpeechRow(articlePhrase: IArticlePhrase) {
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index >= 0) {
      const emptyArticlePhrase = this.getEmptyArticlePhrase(1, articlePhrase);
      emptyArticlePhrase.type = 1;

      if (!articlePhrase.hasChildren) {
        this.dataItem.articlePhrases.splice(index + 1, 0, emptyArticlePhrase);
      } else {
        // find last children and insert ufter that
        const children = this.dataItem.articlePhrases
          .filter(x => x.parentKeyGuid === articlePhrase.keyGuid);
        if (children.length === 0) {
          // no children
          this.dataItem.articlePhrases.splice(index + 1, 0, emptyArticlePhrase);
        } else {
          const max = Math.max.apply(Math, children.map((o) => o.orderNum ));
          const indexCh = this.dataItem.articlePhrases.findIndex((x) => x.parentKeyGuid === articlePhrase.keyGuid && x.orderNum === max);
          this.dataItem.articlePhrases.splice(indexCh + 1, 0, emptyArticlePhrase);
        }
      }

      ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
      this.SetChanged(true);
    }
  }

  deleteRow(articlePhrase: IArticlePhrase) {
    if (articlePhrase.hasChildren) {
      if (this.dataItem.articlePhrases.some(x => x.parentKeyGuid === articlePhrase.keyGuid)) {
        window.alert('You cannot delete this item because it has children?');
        return;
      }
    }

    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index >= 0) {
      if (articlePhrase.parentKeyGuid) {
        // clear paren if it is last child
        if (this.dataItem.articlePhrases.filter(x => x.parentKeyGuid === articlePhrase.parentKeyGuid).length < 2) {
          this.dataItem.articlePhrases.find(x => x.keyGuid === articlePhrase.parentKeyGuid).hasChildren = false;
        }
      }

      this.dataItem.articlePhrases.splice(index, 1);
      ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
      this.SetChanged(true);
    }
  }

  dublicateRow(articlePhrase: IArticlePhrase) {
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index >= 0) {
      const newItem = this.getEmptyArticlePhrase();
      newItem.activityType = articlePhrase.activityType;
      newItem.type = articlePhrase.type;
      newItem.text = articlePhrase.text;
      newItem.language = articlePhrase.language;
      newItem.textSpeechFileName = articlePhrase.textSpeechFileName;
      newItem.hashCode = articlePhrase.hashCode;
      newItem.speachDuration = articlePhrase.speachDuration;
      newItem.articleActorID = articlePhrase.articleActorID;
      newItem.articleActor = articlePhrase.articleActor;
      newItem.pause = articlePhrase.pause;

      newItem.trText = articlePhrase.trText;
      newItem.trLanguage = articlePhrase.trLanguage;
      newItem.trTextSpeechFileName = articlePhrase.trTextSpeechFileName;
      newItem.trHashCode = articlePhrase.trHashCode;
      newItem.trSpeachDuration = articlePhrase.trSpeachDuration;
      newItem.trArticleActor = articlePhrase.trArticleActor;
      newItem.trArticleActorID = articlePhrase.trArticleActorID;
      newItem.trPause = articlePhrase.trPause;

      newItem.parentKeyGuid = articlePhrase.parentKeyGuid;
      newItem.childrenType = '';
      newItem.hasChildren = false;

      this.dataItem.articlePhrases.splice(index + 1, 0, newItem);
      ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
      this.SetChanged(true);
    }

    return index;
  }

  moveUpRow(articlePhrase: IArticlePhrase) {
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index > 0) {
      if (
        (this.dataItem.articlePhrases[index].parentKeyGuid
          && (!this.dataItem.articlePhrases[index - 1].hasChildren
            && this.dataItem.articlePhrases[index - 1].parentKeyGuid)
        )
        ||
        (!this.dataItem.articlePhrases[index].parentKeyGuid
          && (!this.dataItem.articlePhrases[index - 1].hasChildren
            && !this.dataItem.articlePhrases[index - 1].parentKeyGuid)
        )
      ) {
        this.dataItem.articlePhrases.splice(index, 1);
        this.dataItem.articlePhrases.splice(index - 1, 0, articlePhrase);
        ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
        this.SetChanged(true);
      }
    }
  }

  moveDownRow(articlePhrase: IArticlePhrase) {
    const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === articlePhrase.keyGuid);
    if (index < (this.dataItem.articlePhrases.length - 1)) {
      if (
        (this.dataItem.articlePhrases[index].parentKeyGuid
          && (!this.dataItem.articlePhrases[index + 1].hasChildren
            && this.dataItem.articlePhrases[index + 1].parentKeyGuid)
        )
        ||
        (!this.dataItem.articlePhrases[index].parentKeyGuid
          && (!this.dataItem.articlePhrases[index + 1].hasChildren
            && !this.dataItem.articlePhrases[index + 1].parentKeyGuid)
        )
      ) {
        this.dataItem.articlePhrases.splice(index, 1);
        this.dataItem.articlePhrases.splice(index + 1, 0, articlePhrase);
        ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
        this.SetChanged(true);
      }
    }
  }

  splitRow(args) {
    const message = args.control.value;
    let selStart = args.control.selectionStart;
    const selEnd = args.control.selectionEnd;

    const messageTr = args.controlTranslated.value;
    let selStartTr = args.controlTranslated.selectionStart;
    const selEndTr = args.controlTranslated.selectionEnd;

    if (selStart === 0 && selEnd === message.length) {
      return;
    }

    if (!selStart || selStart === message.length) {
      return;
    }

    if (selStart === 0 && selEnd !== message.length) {
      selStart = selEnd;
    }

    if (selStartTr === 0 && selEndTr !== messageTr.length) {
      selStartTr = selEndTr;
    }

    if (selStart === selEnd) {
      const str1 = message.substring(0, selStart);
      const str2 = message.substring(selStart, message.length);

      const str1Tr = messageTr.substring(0, selStartTr);
      const str2Tr = messageTr.substring(selStartTr, messageTr.length);

      const index = this.dataItem.articlePhrases.findIndex((x) => x.keyGuid === args.item.keyGuid);
      if (index >= 0) {
        const oldItem = this.dataItem.articlePhrases[index];
        oldItem.text = str1;
        if (messageTr.length > 0 && selStartTr && selStartTr > 0) {
          oldItem.trText = str1Tr;
        }

        this.dataItem.articlePhrases.splice(index, 1, oldItem);

        const newItem = this.getEmptyArticlePhrase(args.item.activityType, args.item);
        newItem.text = str2;

        if (messageTr.length > 0 && selStartTr && selStartTr > 0) {
          newItem.trText = str2Tr;
        }

        this.dataItem.articlePhrases.splice(index + 1, 0, newItem);
      }
    } else {
      const str1 = message.substring(0, selStart);
      const str2 = message.substring(selStart, selEnd);
      const str3 = message.substring(selEnd, message.length);

      const str1Tr = messageTr.substring(0, selStartTr);
      const str2Tr = messageTr.substring(selStartTr, selEnd);
      const str3Tr = messageTr.substring(selEndTr, messageTr.length);

      const index = this.dataItem.articlePhrases.findIndex((x) => {
         return x.keyGuid === args.item.keyGuid;
      });

      if (index >= 0) {

        const oldItem = this.dataItem.articlePhrases[index];
        oldItem.text = str1;
        if (messageTr.length > 0 && selStartTr && selStartTr > 0) {
          oldItem.trText = str1Tr;
        }

        this.dataItem.articlePhrases.splice(index, 1, oldItem);

        const newItem = this.getEmptyArticlePhrase(args.item.activityType, args.item);
        newItem.text = str2;
        if (messageTr.length > 0 && selStartTr && selStartTr > 0) {
          newItem.trText = str2Tr;
        }

        this.dataItem.articlePhrases.splice(index + 1, 0, newItem);

        if (selEnd < message.length) {
          const newItem2 = this.getEmptyArticlePhrase(args.item.activityType, args.item);
          newItem2.text = str3;

          if (messageTr.length > 0 && selStartTr && selStartTr > 0) {
            newItem2.trText = str3Tr;
          }

          this.dataItem.articlePhrases.splice(index + 2, 0, newItem2);
        }
      }
    }

    ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
    this.SetChanged(true);
  }

  unsetSilentRow(articlePhrase: IArticlePhrase) {
    articlePhrase.silent = false;
    this.SetChanged(true);
  }

  setSilentRow(articlePhrase: IArticlePhrase) {
    articlePhrase.silent = true;
    this.SetChanged(true);
  }

  changedItem(event: any) {
    this.SetChanged(true);
  }

  translateAllSentences() {
  }

  getEmptyArticlePhrase(type: ArticlePhraseType = ArticlePhraseType.speaker, articlePhrase?: IArticlePhrase): IArticlePhrase {
      return  ArticleUtils.getEmptyArticlePhrase(this.dataItem, this.textActors,
      this.textDictorActors, this.translatedActors, this.translatedDictorActors, type, articlePhrase);
  }

  setLoader(value) {
    if (value) {
      this.changeLoaderSize = true;
    }

    this.isLoadingResults = value;
  }

  makeAudio() {
    // const retriveSubject$ = new Subject<string>();
    // retriveSubject$
    //   .pipe(
    //     takeUntil(this.ngUnsubscribe$),
    //     concatMap((sessionId: string) => {
    //       return this.articleService.textToSpeachArticleRetrialState(sessionId);
    //     })
    //   ).subscribe(
    //     (result: IWorkJobsStateEntity) => {

    //       console.log('IWorkJobsStateEntity:', result);
    //       if (result) {
    //         if (result.state >= 100 || result.state < 0) {
    //           // send events that process finished
    //           this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeAudioFile_Loader, payload: false });
    //           this.toasterService.success('Audio create comleted', 'Success');
    //           this.getData(this.dataItem.articleID);
    //         }
    //       }
    //     },
    //     error => {
    //       console.log('IWorkJobsStateEntity error:', error);
    //     },
    //     () => {
    //       this.setLoader(false);
    //       console.log('IWorkJobsStateEntity complited');
    //     }
    //   );

    this.setLoader(true);
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeAudioFile_Loader, payload: true });

    const selIds = this.dataItem.articlePhrases.filter(x => x.selected).map(y => y.articlePhraseID);
    const sessionGuid = Guid.newGuid().ToString();

    this.articleService.textToSpeachArticle(this.dataItem.articleID, selIds, sessionGuid)
      .toPromise()
      .then(result => {
        this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeAudioFile_Loader, payload: false });
        this.toasterService.success('Audio create comleted', 'Success');
        this.getData(this.dataItem.articleID);
        this.setLoader(false);
      })
      .catch(er => {
        this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeAudioFile_Loader, payload: false });
        this.setLoader(false);
        console.log('textToSpeachArticle error ', er);
      });
  }

  makeVideo() {
    // const retriveSubjectV$ = new Subject<string>();
    // retriveSubjectV$
    //   .pipe(
    //     takeUntil(this.ngUnsubscribe$),
    //     concatMap((sessionId: string) => {
    //       return this.articleService.textToVideoArticleRetrialState(sessionId);
    //     })
    //   ).subscribe(
    //     (result: IWorkJobsStateEntity) => {

    //       console.log('IWorkJobsStateEntity V:', result);
    //       if (result) {
    //         if (result.state >= 100 || result.state < 0) {
    //           // send events that process finished
    //           this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeVideoFile_Loader, payload: false });
    //           retriveSubjectV$.complete();
    //           if (this.makeVideoArticleIntervalRef) {
    //             clearInterval(this.makeVideoArticleIntervalRef);
    //           }

    //           if (result.state < 0) {
    //             console.log(`make video state session ${result.args} -- ${result.message}`);
    //             this.toasterService.error('make video state session:', 'Error');
    //           } else {
    //             this.toasterService.success('Video create comleted', 'Success');
    //           }

    //           this.getData(this.dataItem.articleID);
    //         }
    //       }
    //     },
    //     error => {
    //       console.log('IWorkJobsStateEntity V error:', error);
    //     },
    //     () => {
    //       this.setLoader(false);
    //       console.log('IWorkJobsStateEntity V complited');
    //     }
    //   );

    this.setLoader(true);
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeVideoFile_Loader, payload: true });

    const selIds = this.dataItem.articlePhrases.filter(x => x.selected).map(y => y.articlePhraseID);
    const sessionGuid = Guid.newGuid().ToString();
    this.articleService.textToVideoArticle(this.dataItem.articleID, selIds, sessionGuid)
      .toPromise()
      .then(result => {
        this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeVideoFile_Loader, payload: false });
        this.toasterService.success('Video create comleted', 'Success');
        this.getData(this.dataItem.articleID);
      })
      .catch(er => {
        this.toasterService.error('make video state session:', 'Error');
        this.navMenuService.navInput.next({ command: navCommandsInput.Article_MakeVideoFile_Loader, payload: false });
        this.setLoader(false);
      });
  }

  Save() {
    if (!this.dataItem.language || !this.dataItem.languageTranslation) {
      this.toasterService.error('Please select languages', '', { positionClass: 'toast-bottom-center' });
      return;
    }

    if (!this.dataItem.name) {
      this.toasterService.error('Please enter article name', '', { positionClass: 'toast-bottom-center' });
      return;
    }

    this.setLoader(true);
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_Save_Loader, payload: true });

    // merge voices list
    this.dataItem.articleActors = [];
    this.textActors.forEach(x => this.dataItem.articleActors.push(x));
    this.translatedActors.forEach(x => this.dataItem.articleActors.push(x));
    this.textDictorActors.forEach(x => this.dataItem.articleActors.push(x));
    this.translatedDictorActors.forEach(x => this.dataItem.articleActors.push(x));

    this.articleService.Save(this.dataItem)
      .toPromise()
      .then(result => {
        if (this.dataItem.articleID === 0) {
          // if new, reload with new ID
          this.router.navigate([`/article/${result}`]);
        } else {
          this.getData(result);
        }
      })
      .catch(er => {
        this.setLoader(false);
        this.navMenuService.navInput.next({ command: navCommandsInput.Article_Save_Loader, payload: false });
      });
  }

  RecalculatePauses() {
    this.setLoader(true);
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_RecalculatePauses_Loader, payload: true });
    this.setLoader(false);
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_RecalculatePauses_Loader, payload: false });
  }

  changedRow(articlePhrase: IArticlePhrase) {
    this.SetChanged(true);
  }

  SetChanged(value) {
    this.isChanged = value;
    this.navMenuService.navInput.next({ command: navCommandsInput.Article_Save_Disabled, payload: !value });
  }

  fillActorsByLanguage() {
    const result = this.globalService.getActorsForArticle(this.dataItem);

    this.dataItem.articleActors = result;
    this.splitVoicesByRoles();
    this.setActorsLabels();
    this.SetChanged(true);
  }

  splitVoicesByRoles() {
    if (this.dataItem.articleActors) {
      this.textActors = this.dataItem.articleActors.filter((x) => x.role === ArticleActorRole.textSpeaker);
      this.textDictorActors = this.dataItem.articleActors.filter((x) => x.role === ArticleActorRole.textDictorSpeaker);
      this.translatedActors = this.dataItem.articleActors.filter((x) => x.role === ArticleActorRole.textTranslatedSpeaker);
      this.translatedDictorActors = this.dataItem.articleActors.filter((x) => x.role === ArticleActorRole.textTranslatedDictorSpeaker);
    } else {
      this.textActors = [];
      this.textDictorActors = [];
      this.translatedActors = [];
      this.translatedDictorActors = [];
    }

    this.refreshActorsTables();
  }

  editSpeakerVoice(articleActor: IArticleActor, articleActors: IArticleActor[]) {
    if (articleActor && articleActors && articleActors.length > 0) {
      // let tmp = articleActors[0];
      const dialogRef = this.dialog.open(
        ArticleActorDialogComponent,
        {
          disableClose: true,
          width: '600px',
          data:
          {
            source: {
              // articleActorID: articleActor.articleActorID,
              // articleID: articleActor.articleID,
              defaultInRole: articleActor.defaultInRole,
              // keyGuid: articleActor.keyGuid,
              // language: articleActor.language,
              name: articleActor.name,
              // role: articleActor.role,
              voiceName: articleActor.voiceName,
              voicePitch: articleActor.voicePitch,
              voiceSpeed: articleActor.voiceSpeed
            },
            baseVoiceNames: this.getVoiceNamesByLn(articleActor.language)
          }
        }
      );

      dialogRef.afterClosed().toPromise()
        .then((result: IArticleActor) => {
          if (result === null) { return; }

          if (result.defaultInRole && result.defaultInRole !== articleActor.defaultInRole) {
            articleActors.forEach((x) => x.defaultInRole = false);
          }

          articleActor.defaultInRole = result.defaultInRole;
          articleActor.name = result.name;
          articleActor.voiceName = result.voiceName;
          articleActor.voicePitch = result.voicePitch;
          articleActor.voiceSpeed = result.voiceSpeed;

          // update phrases with this actor
          const phrases = this.dataItem.articlePhrases.filter(x => x.articleActor.keyGuid === articleActor.keyGuid);
          phrases.forEach(x => {
            x.textSpeechFileName = '';
            x.speachDuration = 0;
            x.hashCode = 0;
            x.articleActor = articleActor;
          });

          const phrasesT = this.dataItem.articlePhrases.filter(x => x.trArticleActor.keyGuid === articleActor.keyGuid);
          phrasesT.forEach(x => {
            x.trTextSpeechFileName = '';
            x.trSpeachDuration = 0;
            x.trHashCode = 0;
            x.trArticleActor = articleActor;
          });

          this.setActorsLabels();
          this.SetChanged(true);
          this.refreshActorsTables();
        }).finally(() => {
        });
    }
  }

  addSpeakerVoice(articleActorRole: ArticleActorRole) {
    const language =
      (articleActorRole === ArticleActorRole.textSpeaker || articleActorRole === ArticleActorRole.textDictorSpeaker)
        ? this.dataItem.language
        : this.dataItem.languageTranslation;

    const articleActors: IArticleActor[] =
      articleActorRole === ArticleActorRole.textSpeaker
        ? this.textActors
        : articleActorRole === ArticleActorRole.textDictorSpeaker
          ? this.textDictorActors
          : articleActorRole === ArticleActorRole.textTranslatedSpeaker
            ? this.translatedActors
            : this.translatedDictorActors;

    const dialogRef = this.dialog.open(
      ArticleActorDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data:
        {
          source: {
            articleActorID: 0,
            articleID: this.dataItem.articleID,
            defaultInRole: false,
            keyGuid: Guid.newGuid().ToString(),
            language,
            name: '',
            role: articleActorRole,
            voiceName: '',
            voicePitch: 0,
            voiceSpeed: 1
          },

          baseVoiceNames: this.getVoiceNamesByLn(language)
        }
      }
    );

    dialogRef.afterClosed().toPromise()
      .then((result: IArticleActor) => {
        if (result === null) { return; }

        if (result.defaultInRole) {
          articleActors.forEach((x) => x.defaultInRole = false);
        }

        articleActors.push(result);
        this.setActorsLabels();
        this.SetChanged(true);
        this.refreshActorsTables();
      }).finally(() => {
      });
  }

  getVoiceNamesByLn(language: string) {
    const voices = this.globalService.GetVoicesByLanguage(language);

    return voices
      .map((x) => x.voice)
      .filter((item, index, self) => {
        return self.indexOf(item) === index;
      });
  }

  refreshActorsTables() {
    setTimeout(() => {
      if (this.textActorsTable) {
        this.textActorsTable.renderRows();
      }
      if (this.translatedActorsTable) {
        this.translatedActorsTable.renderRows();
      }
      if (this.textDictorActorsTable) {
        this.textDictorActorsTable.renderRows();
      }
      if (this.translatedDictorActorsTable) {
        this.translatedDictorActorsTable.renderRows();
      }
    }, 0);
  }

  showHideChildren(articlePhrase: IArticlePhrase) {
    if (!articlePhrase.childrenClosed) {
      articlePhrase.childrenClosed = true;
      this.dataItem.articlePhrases.forEach((x => {
        if (x.parentKeyGuid === articlePhrase.keyGuid) {
          x.hidden = true;
        }
      }));
    } else {
      articlePhrase.childrenClosed = false;
      this.dataItem.articlePhrases.forEach((x => {
        if (x.parentKeyGuid === articlePhrase.keyGuid) {
          x.hidden = false;
        }
      }));
    }

    this.dataItem.articlePhrases = this.dataItem.articlePhrases.sort((x, y) => x.orderNum - y.orderNum);
  }

  deleteChildren(articlePhrase: IArticlePhrase) {
    if (window.confirm('Are sure you want to delete this item ?')) {
      const items = this.dataItem.articlePhrases.filter(x =>
        x.parentKeyGuid !== articlePhrase.keyGuid
      );

      articlePhrase.hasChildren = false;
      articlePhrase.childrenClosed = false;
      this.dataItem.articlePhrases = items;

      ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
      this.SetChanged(true);
    }
  }

  collapseAll() {
    this.dataItem.articlePhrases.forEach((x => {
      if (x.parentKeyGuid) {
        x.hidden = true;
      }

      if (x.hasChildren) {
        x.childrenClosed = true;
      }
    }));

    this.dataItem.articlePhrases = this.dataItem.articlePhrases.sort((x, y) => x.orderNum - y.orderNum);
  }

  insertRowsTmp1() {
    const newRow1 = this.getEmptyArticlePhrase();
    const newRow2 = this.getEmptyArticlePhrase(ArticlePhraseType.dictor);
    const newRow3 = this.getEmptyArticlePhrase(ArticlePhraseType.dictor);
    const newRow4 = this.getEmptyArticlePhrase();
    const newRow5 = this.getEmptyArticlePhrase(ArticlePhraseType.dictor);
    const newRow6 = this.getEmptyArticlePhrase();
    const newRow7 = this.getEmptyArticlePhrase(ArticlePhraseType.dictor);
    const newRow8 = this.getEmptyArticlePhrase();
    const newRow9 = this.getEmptyArticlePhrase(ArticlePhraseType.dictor);
    const newRow10 = this.getEmptyArticlePhrase();
    const newRow11 = this.getEmptyArticlePhrase(ArticlePhraseType.dictor);
    const newRow12 = this.getEmptyArticlePhrase();

    newRow1.text = '<название>';
    newRow2.trText = '<> часть';
    newRow3.trText = 'прослушайте часть пройденного материала и проверьте, на сколько хорошо вы его усвоили.';
    newRow4.text = '<предидущий материал>';
    newRow5.trText = 'прослушайте внимательно новый материал';
    newRow6.text = '<новый материал>';
    newRow7.trText = 'прослушайте еще раз';
    newRow8.text = '<новый материал>';
    newRow9.trText = 'прослушайте фразу';
    newRow10.text = '<---------первая фраза---->';
    newRow11.trText = 'теперь еще раз прослушайте текст целеком';
    newRow12.text = '<новый материал>';

    this.dataItem.articlePhrases.splice(0, 0, newRow12);
    this.dataItem.articlePhrases.splice(0, 0, newRow11);
    this.dataItem.articlePhrases.splice(0, 0, newRow10);
    this.dataItem.articlePhrases.splice(0, 0, newRow9);
    this.dataItem.articlePhrases.splice(0, 0, newRow8);
    this.dataItem.articlePhrases.splice(0, 0, newRow7);
    this.dataItem.articlePhrases.splice(0, 0, newRow6);
    this.dataItem.articlePhrases.splice(0, 0, newRow5);
    this.dataItem.articlePhrases.splice(0, 0, newRow4);
    this.dataItem.articlePhrases.splice(0, 0, newRow3);
    this.dataItem.articlePhrases.splice(0, 0, newRow2);
    this.dataItem.articlePhrases.splice(0, 0, newRow1);

    ArticleUtils.recalculateArticlePhrasesOrder(this.dataItem);
    this.SetChanged(true);
  }

  showVideoPlayerDialog() {
    const dialogRef = this.dialog.open(
      VideoPlayerDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data:
        {
          videoFileName: this.dataItem.videoFileName
        }
      }
    );

    dialogRef.afterClosed().toPromise()
      .then(() => {
        return;
      }).finally(() => {
      });
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}


