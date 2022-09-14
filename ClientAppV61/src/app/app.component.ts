import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { NavMenuService } from './shared/services/navmenu.service';
import { navCommandsInput, navCommandsOutput } from './shared/interfaces/INavCommands';
import { delay, map, takeUntil } from 'rxjs/operators';
import { environment } from '../environments/environment';
import { Observable, Subject } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { LanguagesSelectDialogComponent } from './shared/components/languages-select-dialog/languages-select-dialog.component';
import { IAppSettings } from './shared/interfaces/IAppSettings';
import { GlobalService } from './shared/services/global.service';
import { AppSettingsService } from './shared/services/app-settings.service';
import { ArticleService } from './shared/services/article.service';
import { AuthorizeService, IUser } from 'src/api-authorization/authorize.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  private ngUnsubscribe$ = new Subject();
  public currUser: IUser;
  public currAppSetting: IAppSettings;

  private curUrl = '';
  public tbArticles_Toolbar_Visible = false;
  public tbArticle_Toolbar_Visible = false;
  public tbArticle_Toolbar_Disabled = false;
  public tbArticle_Toolbar_Save_Loader = false;
  public tbArticle_Toolbar_Save_Disabled = false;
  public tbArticle_Toolbar_MakeAudio_Loader = false;
  public tbArticle_Toolbar_MakeAudio_Disabled = false;
  public tbArticle_Toolbar_MakeVideo_Loader = false;
  public tbArticle_Toolbar_MakeVideo_Disabled = false;
  public tbArticle_Toolbar_RecalculatePauses_Loader = false;

  public tbWords_Toolbar_Visible = false;

  public articleShowToolboxValue = false;
  public articleAudioFileValue = '';
  public articlesAudioUrl: string = environment.articlesAudioUrl;
  public articleVideoFileValue = '';
  public articlesVideoUrl: string = environment.articlesVideoUrl;
  public isAdmin = false;
  public isLessonMaker = false;
  public isSuperAdmin = false;
  public codeVersion = 'v5.000002';

  constructor(
    private router: Router,
    private navMenuService: NavMenuService,
    private authorizeService: AuthorizeService,
    private dialog: MatDialog,
    private globalService: GlobalService,
    private appSettingsService: AppSettingsService,
    private articleService: ArticleService,
  ) { }

  ngOnInit() {

    setTimeout(() => {
      if (this.currUser) {
        this.articleService.getTick().toPromise()
          .then((v: any) => {
            console.log('checkTick Version: ',  v.api_version + ' code:' + this.codeVersion);
          });
      }
    }, 10000);

    // WRONG code. should be subscribe of authorizeService.userSubject
    this.authorizeService.getCurrentUser()
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe((currUser: IUser) => {
        if ((currUser && this.currUser && currUser.id !== this.currUser.id)
          || (!currUser && this.currUser)
          || (currUser && !this.currUser)
        ) {
          this.isAdmin = this.authorizeService.isAdmin();
          this.isLessonMaker = this.authorizeService.isLessonMaker();
          this.isSuperAdmin = this.authorizeService.isSuperAdmin();

          // user changed
          this.currUser = currUser;
          if (currUser) {
            // get languages to this user from brawser store
            const appSetting = this.appSettingsService.getAppSettingsFromStore(this.currUser);
            if (!appSetting) {
              this.changeLanguages(true);
            } else {
              const currAppSetting = this.appSettingsService.getCurrent();
              if (currAppSetting) {
                if (currAppSetting.LearnLanguage.languageID !== appSetting.LearnLanguage.languageID
                  || currAppSetting.NativeLanguage.languageID !== appSetting.NativeLanguage.languageID
                ) {
                  this.appSettingsService.refreshCurrent(appSetting);
                }
              } else {
                this.appSettingsService.refreshCurrent(appSetting);
              }
            }
          }
        }
      });

    this.appSettingsService.currentAppSetting
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe((currAppSetting: IAppSettings) => {
        this.currAppSetting = currAppSetting;
      });

    this.router.events
      .pipe(delay(0))
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe(event => {
        if (event instanceof NavigationEnd) {

          this.tbArticles_Toolbar_Visible = false;

          this.tbArticle_Toolbar_Visible = false;
          this.tbArticle_Toolbar_Disabled = false;
          this.tbArticle_Toolbar_Save_Loader = false;
          this.tbArticle_Toolbar_Save_Disabled = false;
          this.tbArticle_Toolbar_MakeAudio_Loader = false;
          this.tbArticle_Toolbar_MakeAudio_Disabled = false;
          this.tbArticle_Toolbar_MakeVideo_Loader = false;
          this.tbArticle_Toolbar_MakeVideo_Disabled = false;
          this.tbArticle_Toolbar_RecalculatePauses_Loader = false;

          this.tbWords_Toolbar_Visible = false;

          this.curUrl = event.urlAfterRedirects;
          if (this.curUrl.indexOf('/articles') === 0) {
            this.tbArticles_Toolbar_Visible = true;
          } else if (this.curUrl.indexOf('/article/') === 0) {
            this.tbArticle_Toolbar_Visible = true;
          } else if (this.curUrl.indexOf('/words') === 0) {
            this.tbWords_Toolbar_Visible = true;
          } else {
            console.log('other page ss');
          }
        }
      });

    this.navMenuService.navInput
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe(data => {
        if (!data) {
          return;
        }

        if (data.command === navCommandsInput.Article_Save_Loader) {
          this.tbArticle_Toolbar_Disabled = data.payload;
          this.tbArticle_Toolbar_Save_Loader = data.payload;
        } else if (data.command === navCommandsInput.Article_All_Disabled) {
          this.tbArticle_Toolbar_Disabled = data.payload;
        } else if (data.command === navCommandsInput.Article_Save_Disabled) {
          this.tbArticle_Toolbar_Save_Disabled = data.payload;
        } else if (data.command === navCommandsInput.Article_MakeAudioFile_Disabled) {
          this.tbArticle_Toolbar_MakeAudio_Disabled = data.payload;
        } else if (data.command === navCommandsInput.Article_MakeAudioFile_Loader) {
          this.tbArticle_Toolbar_MakeAudio_Loader = data.payload;
          this.tbArticle_Toolbar_Disabled = data.payload;
        } else if (data.command === navCommandsInput.Article_MakeVideoFile_Disabled) {
          this.tbArticle_Toolbar_MakeVideo_Disabled = data.payload;
        } else if (data.command === navCommandsInput.Article_MakeVideoFile_Loader) {
          this.tbArticle_Toolbar_MakeVideo_Loader = data.payload;
          this.tbArticle_Toolbar_Disabled = data.payload;
        } else if (data.command === navCommandsInput.Article_RecalculatePauses_Loader) {
          this.tbArticle_Toolbar_RecalculatePauses_Loader = data.payload;
        } else if (data.command === navCommandsInput.Article_AudioFile_Value) {
          this.articleAudioFileValue = data.payload;
        } else if (data.command === navCommandsInput.Article_VideoFile_Value) {
          this.articleVideoFileValue = data.payload;
        } else {
          console.log('this.navMenuService.navInput.subscribe: unknown command', data);
        }
      });
  }

  changeLanguages(onlyOk = false) {
    const dialogRef = this.dialog.open(
      LanguagesSelectDialogComponent,
      {
        disableClose: true,
        width: '600px',
        data: { onlyOk: onlyOk },
      }
    );

    dialogRef.afterClosed()
      .toPromise()
      .then((dialogResult: IAppSettings) => {
        console.log('dialogResult:', dialogResult);
        if (dialogResult) {
          this.appSettingsService.updateAppSettingsInStore(dialogResult, this.currUser);
        }
      })
      .finally(() => {
      });
  }

  articleSave() {
    this.navMenuService.navOutput.next({ command: navCommandsOutput.Article_Save_Click });
  }

  articleShowToolbox(event) {
    this.navMenuService.navOutput.next({ command: navCommandsOutput.Article_ShowPhraseToolbox_Click, payload: event });
  }

  articleMakeAudio() {
    this.navMenuService.navOutput.next({ command: navCommandsOutput.Article_MakeAudioFile_Click });
  }

  articleRecalculatePauses() {
    this.navMenuService.navOutput.next({ command: navCommandsOutput.Article_RecalculatePauses_Click });
  }

  articleCollapse() {
    this.navMenuService.navOutput.next({ command: navCommandsOutput.Article_CollapseAll_Click });
  }

  articleMakeVideo() {
    this.navMenuService.navOutput.next({ command: navCommandsOutput.Article_MakeVideoFile_Click });
  }

  openShowVideoDialog() {
    this.navMenuService.navOutput.next({ command: navCommandsOutput.Article_OpenVideoDialog_Click });
  }

  redirectToMenageProfile() {
    window.open(environment.baseRoot + '/Identity/Account/Manage', '_self');
  }

  redirectToMaiSite() {
    window.open(environment.baseRoot , '_self');
  }

  redirectToLogout() {
    window.open(environment.baseRoot + '/Identity/Account/LogoutClient', '_self');
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
