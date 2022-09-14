import { Injectable } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { INavCommandInput, INavCommandOutput } from '../interfaces/INavCommands';

@Injectable({
  providedIn: 'root'
})
export class NavMenuService {
  //public navInput = new BehaviorSubject<INavCommandInput>(undefined as INavCommandInput);
  //public navOutput = new BehaviorSubject<INavCommandOutput>(undefined as INavCommandOutput);

  public navInput = new Subject<INavCommandInput>();
  public navOutput = new Subject<INavCommandOutput>();
  constructor() {
  }
}
