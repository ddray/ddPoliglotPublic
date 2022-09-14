import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { IWord } from 'src/app/shared/interfaces/IWord';
import { IWordPhrase } from 'src/app/shared/interfaces/IWordPhrase';
import { IWordsForPhraseState } from 'src/app/shared/interfaces/IWordsForPhraseState';
import { AppSettingsService } from 'src/app/shared/services/app-settings.service';
import { WordsForPhraseStateService } from 'src/app/shared/services/wordsForPhraseState.service';
import {CdkDragDrop, moveItemInArray, transferArrayItem} from '@angular/cdk/drag-drop';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { IAppSettings } from 'src/app/shared/interfaces/IAppSettings';
import { OnDestroy } from '@angular/core';

@Component({
  selector: 'app-special-lessons',
  templateUrl: './special-lessons.component.html',
  styleUrls: ['./special-lessons.component.css']
})
export class SpecialLessonsComponent implements OnInit, OnDestroy {
  public data: IWordsForPhraseState;
  public used: IWord[];
  public removed: IWord[];
  private ngUnsubscribe$ = new Subject();

  constructor(
    private wordsForPhraseStateService: WordsForPhraseStateService,
    private toastr: ToastrService,
    private appSettingsService: AppSettingsService,
  ) { }

  ngOnInit(): void {
    this.appSettingsService.currentAppSetting
      .pipe(takeUntil(this.ngUnsubscribe$))
      .subscribe((currAppSetting: IAppSettings) => {
        if (currAppSetting) {
          this.getNext();
        }
      });
  }

  getNext() {
    this.wordsForPhraseStateService.GetNext()
    .toPromise()
    .then((wordsForPhraseState: IWordsForPhraseState) => {
      this.data = wordsForPhraseState;
      this.used = [];
      this.removed = [];
    });
  }

  saveAndNext() {
    this.wordsForPhraseStateService.Save(this.data).toPromise()
    .then(() => {
      console.log('saved');
      this.getNext();
    });
  }

  removeFromWait(word: IWord) {
    this.data.waiting = this.data.waiting.filter((x) => x.wordID !== word.wordID);
    this.removed.push(word);
  }

  removeFromRemoved(word) {
    this.removed = this.removed.filter((x) => x.wordID !== word.wordID);
    this.data.waiting.push(word);
  }

  selectFromWait(word) {
    this.data.waiting = this.data.waiting.filter((x) => x.wordID !== word.wordID);
    this.used.push(word);
  }

  removeFromUsed(word) {
    this.used = this.used.filter((x) => x.wordID !== word.wordID);
    this.data.waiting.push(word);
  }

  drop(event: CdkDragDrop<string[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data,
                        event.container.data,
                        event.previousIndex,
                        event.currentIndex);
    }
  }

  goToRate() {
    this.wordsForPhraseStateService.GoToRate(this.data)
    .toPromise()
    .then((wordsForPhraseState: IWordsForPhraseState) => {
      this.data = wordsForPhraseState;
      this.used = [];
      this.removed = [];
    });
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}

