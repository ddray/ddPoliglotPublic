import { ErrorStateMatcher } from '@angular/material/core';
import { FormControl, FormGroupDirective, NgForm } from '@angular/forms';

export class Guid {
  constructor(public guid: string) {
    this._guid = guid;
  }

  // tslint:disable-next-line:variable-name
  private _guid: string;

  static newGuid() {
    let r = 0;
    const result: string = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'
    .replace(/[xy]/g, (c) => {
      let v: any;
      // tslint:disable-next-line:no-bitwise
      r = Math.random() * 16 | 0, v = (c === 'x') ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });

    return new Guid(result);
  }

  public ToString(): string {
    return this._guid;
  }
}

export class Utils {
  static getHashCode(str: string): number {
    let hash = 0, i, chr;
    if (str.length === 0) {
      return hash;
    }

    for (i = 0; i < str.length; i++) {
      chr = str.charCodeAt(i);
      hash = ((hash << 5) - hash) + chr;
      hash & hash; // Convert to 32bit integer
    }

    return hash;
  }

  static getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    if (min === max)
      return max;
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }
}

/** Error when invalid control is dirty, touched, or submitted. */
export class MyErrorStateMatcher implements ErrorStateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && (control.invalid || form.invalid) && (control.dirty || control.touched || isSubmitted));
  }
}
