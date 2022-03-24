import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-market-place-services',
  templateUrl: './market-place-services.component.html',
  styleUrls: ['./market-place-services.component.css']
})
export class MarketPlaceServicesComponent implements OnInit {
  
  @Input() image: '';
  @Input() checkStatus: any;
  @Input() isChecked: any;
  constructor() {}

  ngOnInit(): void {
    this.isChecked = false;
  }

}
