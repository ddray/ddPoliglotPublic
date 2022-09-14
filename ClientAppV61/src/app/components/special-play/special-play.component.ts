import { Component, OnInit, EventEmitter, OnDestroy, AfterViewInit, ViewChild } from '@angular/core';
import { IRatingControlValue } from 'src/app/shared/components/rating/rating.component';
import { Subject, merge, of } from 'rxjs';
import { IWord } from 'src/app/shared/interfaces/IWord';
import { WordService } from 'src/app/shared/services/word.service';
import { AppSettingsService } from 'src/app/shared/services/app-settings.service';
import { takeUntil, startWith, switchMap, map, catchError } from 'rxjs/operators';
import { ISearchListArg } from 'src/app/shared/interfaces/IListArg';
import { ArticleByParamService } from 'src/app/shared/services/articleByParam.service';
import { environment } from 'src/environments/environment';
import { PlayList } from 'src/app/shared/interfaces/IPlailable';
import { MultyplayerComponent } from './multyplayer/multyplayer.component';
import { PlayListUtils } from 'src/app/shared/models/PlayListUtils';
import { IWordUser } from 'src/app/shared/interfaces/IWordUser';

@Component({
  selector: 'app-special-play',
  templateUrl: './special-play.component.html',
  styleUrls: ['./special-play.component.scss']
})
export class SpecialPlayComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('multyplayerComponent') multyplayerComponent: MultyplayerComponent;

  constructor(
    private wordService: WordService,
    private appSettingsService: AppSettingsService,
    private articleByParamService: ArticleByParamService,
  ) {
  }

  private refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  public displayedColumns: string[] = ['rate', 'wordID', 'text', 'grade', 'wordPhraseCountSelected', 'act'];
  public resultsLength = 0;
  public isLoading = true;
  data: IWord[];

  ngOnInit(): void {
  }

  ngAfterViewInit() {
    merge(this.refreshPage, this.appSettingsService.currentAppSetting)
      .pipe(takeUntil(this.ngUnsubscribe$))
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoading = true;
          return this.wordService.getRecomended(
            {
              sort: '',
              order: '',
              page: 0,
              pagesize: 10,
              searchText: '',
              rateFrom: 0,
              rateTo: 0,
              gradeFrom: 0,
              gradeTo: 0,
              recomended: true,
            } as ISearchListArg
          );
        }),
        map(result => {
          this.isLoading = false;
          this.resultsLength = result.count;
          return result.data;
        }),
        catchError((err) => {
          this.isLoading = false;
          return of([]);
        })
      ).subscribe((data: IWord[]) => {
        // get template
        this.setLoader(true);
        this.data = data;
        this.articleByParamService.getByIdWithReadyAudio('153')
          .toPromise()
          .then(async result => {
            const articleByParamData = result.articleByParamDataWithAudio;
            console.log('articleByParamDataWithAudio: ', articleByParamData);
            const playLists: PlayList[] = PlayListUtils.makePlayLists(data, articleByParamData);
            await this.multyplayerComponent.setPlayLists(playLists);
          }).catch(err => {
            console.log('err: ', err);
          }).finally(() => {
            console.log('Finaly!!!');
            this.setLoader(false);
          });
      });
  }

  setLoader(value) {
    this.isLoading = value;
  }

  refresh()
  {
    this.refreshPage.next();
  }

  changeWordGrade(event: IRatingControlValue) {
    const item = {
      wordID: event.itemId,
      languageID: event.item.languageID,
      grade: event.value,
    } as IWordUser;

    // if we need to exclude this word from the lists
    return this.wordService.updateWordUser(item);
  }

  delete(word: IWord) {
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
