import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { ArticleService } from 'src/app/shared/services/article.service';
import { MatDialog } from '@angular/material/dialog';
import { AuthorizeService, IUser } from 'src/api-authorization/authorize.service';
import { GlobalService } from 'src/app/shared/services/global.service';
import { AppSettingsService } from 'src/app/shared/services/app-settings.service';
import { Subject } from 'rxjs';
import { IArticle } from 'src/app/shared/interfaces/IArticle';
import { environment } from 'src/environments/environment';
import { takeUntil } from 'rxjs/operators';
import { navCommandsInput } from 'src/app/shared/interfaces/INavCommands';
import { IArticlePhrase } from 'src/app/shared/interfaces/IArticlePhrase';

@Component({
  selector: 'app-article-special-play',
  templateUrl: './article-special-play.component.html',
  styleUrls: ['./article-special-play.component.scss']
})
export class ArticleSpecialPlayComponent  implements OnInit, OnDestroy{
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private articleService: ArticleService,
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
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;

  public audios: Array<string>;
  public phraseText = '';
  public trPhraseText = '';

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
  }

  getData(id) {
    this.setLoader(true);

    if (id > 0) {
      this.articleService.getById(id)
        .toPromise()
        .then(result => {
          result.articlePhrases = result.articlePhrases.sort((x, y) => x.orderNum - y.orderNum);
          this.dataItem = result;
          const arr = new Array<string>();
          this.dataItem.articlePhrases.forEach((x) => {
            if (x.textSpeechFileName) {
              arr.push(this.removeExt(x.textSpeechFileName));
            }

            if (x.trTextSpeechFileName) {
              arr.push(this.removeExt(x.trTextSpeechFileName));
            }
          });

          this.audios = [... new Set(arr)];
          console.log('Loaded: ', this.dataItem);
          // console.log('arr', arr);
        }).catch(err => {
          console.log('err: ', err);
        }).finally(() => {
          this.setLoader(false);
        });
    }
  }

  removeExt(filename: string) {
    const arr = filename.split('.');
    return arr[0];
  }

  async play() {
    for await (const phrase of this.dataItem.articlePhrases) {
      await this.playPhrase(phrase);
    }
  }

  async playPhrase(phrase: IArticlePhrase) {
    if (phrase.text) {
      await this.playText(phrase.text, phrase.textSpeechFileName, phrase.pause, false);
    }

    if (phrase.trText) {
      await this.playText(phrase.trText, phrase.trTextSpeechFileName, phrase.trPause, true);
    }
  }

  async playText(text: string, audioFileName: string, pause: number, isTr = false) {
      this.phraseText = text;
      await this.playAudio(audioFileName);
      await this.delay(pause);
  }

  playAudio(fileName: string) {
    return new Promise<void>((resolve, reject) => {
      const aa = new Audio();
      aa.src = `${this.phrasesAudioUrl}/${fileName}`;
      aa.load();
      aa.onended = () => {
        resolve();
      };
      aa.play();
    });
  }

  async playDelay(pause) {
    await this.delay(pause);
  }

  delay(pause) {
    return new Promise<void>((resolve, reject) => setTimeout(() => {
      resolve();
    } , pause * 1000));
  }

  setLoader(value) {
    this.isLoadingResults = value;
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
