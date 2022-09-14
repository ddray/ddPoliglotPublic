import { Component, ViewChild, AfterViewInit, EventEmitter, OnDestroy, OnInit } from '@angular/core';
import { WordService } from '../../shared/services/word.service';
import { ToastrService } from 'ngx-toastr';
import { AppSettingsService } from '../../shared/services/app-settings.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-words',
  templateUrl: './words.component.html',
  styleUrls: ['./words.component.scss']
})
export class WordsComponent implements OnInit, OnDestroy {
  constructor(
    private wordService: WordService,
    private toastr: ToastrService,
    private appSettingsService: AppSettingsService,
    private dialog: MatDialog,
  ) { }

  ngOnInit(): void {
  }

  ngOnDestroy() {
  }
}
