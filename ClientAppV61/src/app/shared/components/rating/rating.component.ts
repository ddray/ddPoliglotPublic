import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-rating',
  templateUrl: './rating.component.html',
  styleUrls: ['./rating.component.scss']
})
export class RatingComponent implements OnInit {
  @Input() rating: number;
  @Input() itemId: number;
  @Input() itemRef: any;
  @Output() ratingClick: EventEmitter<any> = new EventEmitter<any>();

  inputName: string;

  ngOnInit() {
    this.inputName = this.itemId + '_rating';
  }

  onClick(rating: number): void {
    this.rating = rating;
    this.ratingClick.emit({
      item: this.itemRef,
      itemId: this.itemId,
      value: rating
    } as IRatingControlValue);
  }
}

export interface IRatingControlValue {
  item: any;
  itemId: number;
  value: number;
}
