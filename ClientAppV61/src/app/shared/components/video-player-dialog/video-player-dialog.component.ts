import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-video-player-dialog',
  templateUrl: './video-player-dialog.component.html',
  styleUrls: ['./video-player-dialog.component.scss']
})
export class VideoPlayerDialogComponent implements OnInit {
  public isLoading = false;
  public resultData: any;
  public articleVideoFileValue = '';
  public articlesVideoUrl: string = environment.articlesVideoUrl;

  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<VideoPlayerDialogComponent>,
  ) { }

  ngOnInit() {
    this.articleVideoFileValue = this.data.videoFileName;
  }

  cancel() {
    this.dialogRef.close(null);
  }

  ok() {
    this.dialogRef.close(this.resultData);
  }
}
