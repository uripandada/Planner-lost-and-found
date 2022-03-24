import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import {ThemePalette} from '@angular/material/core';

@Component({
  selector: 'app-market-place-services',
  templateUrl: './market-place-services.component.html',
  styleUrls: ['./market-place-services.component.css']
})
export class MarketPlaceServicesComponent implements OnInit {
  color: ThemePalette;
  @Input() id: '';
  @Input() image: '';
  @Input() checkStatus: any;
  @Input() isChecked: any;
  @Output() changeEvent = new EventEmitter();
  constructor() {}

  ngOnInit(): void {
    this.color = "primary"
  }

  onChangeEvent(data: any) {
    this.isChecked = data.checked;
    this.changeEvent.emit(data);    
  }

}
