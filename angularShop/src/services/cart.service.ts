import { Injectable } from '@angular/core';
import { Item } from 'src/models/item';

@Injectable({
  providedIn: 'root'
})
export class CartService {

  sharedData: Array<Item>;
  Total = 0;

  constructor() {
    this.sharedData = new Array<Item>();
   }
}