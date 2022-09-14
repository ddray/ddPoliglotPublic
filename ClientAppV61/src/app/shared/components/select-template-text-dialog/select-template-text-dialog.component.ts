import { Component, OnInit, Inject, ViewChild, OnDestroy, AfterViewInit, EventEmitter } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator } from '@angular/material/paginator';
import { AppSettingsService } from '../../services/app-settings.service';
import { Subject, merge, of } from 'rxjs';
import { takeUntil, startWith, switchMap, map, catchError } from 'rxjs/operators';
import { IListResult } from '../../interfaces/IListResult';
import { ISearchListArg } from '../../interfaces/IListArg';
import { MixParamTextTempService } from '../../services/mixParamTextTemp.service';
import { IMixParamTextTemp } from '../../interfaces/IMixParamTextTemp';

@Component({
  selector: 'app-select-template-text-dialog',
  templateUrl: './select-template-text-dialog.component.html',
  styleUrls: ['./select-template-text-dialog.component.scss']
})
export class SelectTemplateTextDialogComponent implements OnInit, OnDestroy, AfterViewInit {
  public items: Array<IMixParamTextTemp>;
  public updatedItem: IMixParamTextTemp;
  public resultsLength = 0;
  public isLoading = false;
  public selectedItems = new Array <IMixParamTextTemp>();
  public displayedColumns: string[] = ['text', 'act'];
  public selectable = true;
  public removable = true;
  public formMode = false;

  private refreshPage = new EventEmitter<any>();
  private ngUnsubscribe$ = new Subject();

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<SelectTemplateTextDialogComponent>,
    private mixParamTextTempService: MixParamTextTempService,
    private appSettingsService: AppSettingsService,
  ) {
  }

  async ngOnInit() {
    this.isLoading = true;
    this.formMode = false;
  }

  ngAfterViewInit() {
    this.paginator.pageIndex = 0;
    this.refreshPage.emit();
    merge(this.paginator.page, this.refreshPage)
      .pipe(takeUntil(this.ngUnsubscribe$))
      .pipe(
        startWith({}),
        switchMap(() => {
          setTimeout(() => {
            this.isLoading = true;
          }, 0);
          return this.mixParamTextTempService.getFiltered(
            {
              str1: this.data.tempKey,
              page: this.paginator.pageIndex,
              pagesize: this.paginator.pageSize,
            } as ISearchListArg
          );
        }),
        map((result: IListResult) => {
          this.resultsLength = result.count;
          return result.data;
        }),
        catchError((e) => {
          setTimeout(() => { this.isLoading = false; }, 0);
          return of([]);
        })
      ).subscribe(data => {
        this.items = data;

        this.items.forEach((x) => {
          if (this.selectedItems.find(f => f.mixParamTextTempID === x.mixParamTextTempID)) {
            x.selected = true;
          }

          x.textHtml = x.text.replace(/\r?\n/g, '<br />');
        });

        setTimeout(() => { this.isLoading = false; }, 0);
      });
  }

  updateCancel() {
    this.updatedItem = this.getEmptyItem();
    this.formMode = false;
  }

  updateOk() {
    this.isLoading = true;

    this.mixParamTextTempService.Save(this.updatedItem)
      .toPromise()
      .then(() => {
        this.isLoading = false;
        this.updatedItem = this.getEmptyItem();
        this.refreshPage.emit();
        this.formMode = false;
      });
  }

  getEmptyItem(): IMixParamTextTemp {
    return {
      mixParamTextTempID: 0,
      text: '',
      keyTemp: this.data.tempKey,
      learnLanguageID: this.appSettingsService.getCurrent().LearnLanguage.languageID,
      nativeLanguageID: this.appSettingsService.getCurrent().NativeLanguage.languageID,
    } as IMixParamTextTemp;
  }

  addNew() {
    this.formMode = true;
    this.updatedItem = this.getEmptyItem();
  }

  update(item: IMixParamTextTemp) {
    this.updatedItem = item;
    this.formMode = true;
  }

  delete(item: IMixParamTextTemp) {
    if (!window.confirm('Are sure you want to delete this item ?')) {
      return;
    }

    this.isLoading = true;
    this.mixParamTextTempService.Delete(item)
      .toPromise()
      .then(() => {
        this.isLoading = false;
        this.updatedItem = this.getEmptyItem();
        this.paginator.pageIndex = 0;
        this.refreshPage.emit();
      });
  }

  removeFromSelection(item: IMixParamTextTemp) {
    this.selectedItems = this.selectedItems.filter(x => x.mixParamTextTempID !== item.mixParamTextTempID);
    this.items.forEach((x) => {
      if (x.mixParamTextTempID === item.mixParamTextTempID) {
        x.selected = false;
      }
    });
  }

  addToSelected(item: IMixParamTextTemp) {
    this.selectedItems.push(item);
    this.items.forEach((x) => {
      if (x.mixParamTextTempID === item.mixParamTextTempID) {
        x.selected = true;
      }
    });
  }

  cutText(text: string, len: number = 40) {
    let result: string;

    if (!text || text.length <= len) {
      return text;
    }

    result = text.substring(0, len) + ' ...';
    return result;
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    console.log('this.selectedItems', this.selectedItems);

    if (!this.selectedItems) {
      alert('select text');
      return;
    }

    this.dialogRef.close(this.selectedItems);
  }

  ngOnDestroy() {
    this.ngUnsubscribe$.next();
    this.ngUnsubscribe$.complete();
  }
}
