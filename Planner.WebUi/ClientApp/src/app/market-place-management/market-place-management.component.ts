import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-market-place-management',
  templateUrl: './market-place-management.component.html',
  styleUrls: ['./market-place-management.component.scss']
})
export class MarketPlaceManagement implements OnInit {
  sorts = [
    { key: 'NAME_ASC', value: 'Compensation A to Z'  },
    { key: 'NAME_DESC', value: 'Compensation Z to A' },
    { key: 'PRICE_ASC', value: 'Price A to Z' },
    { key: 'PRICE_DESC', value: 'Price Z to A' },
  ];

  public marketPlaces: Array<any>;
  public checkParentStatus: boolean;
  
  constructor(
    private _router: Router
    ) {}
    
    ngOnInit(): void {
      this.marketPlaces = [
        {
          id: "peekin",
          url: '../../../assets/images/peekin.png',
          isChecked: false
        },
        {
        id: "alexa",
        url: '../../../assets/images/alexa.png',
        isChecked: false
      },
      {
        id: "MQTT",
        url: '../../../assets/images/MQTT.png',
        isChecked: false
      },
      {
        id: "BACnet",
        url: '../../../assets/images/BACnet.png',
        isChecked: false
      },
      {
        id: "LOUNGE",
        url: '../../../assets/images/LOUNGE.png',
        isChecked: false
      },
      {
        id: "QUICKtest",
        url: '../../../assets/images/QUICKtest.png',
        isChecked: false
      },
      {
        id: "nocore",
        url: '../../../assets/images/nocore.png',
        isChecked: false
      },
      {
        id: "Concierge",
        url: '../../../assets/images/Concierge.png',
        isChecked: false
      },
      {
        id: "Niko",
        url: '../../../assets/images/Niko.png',
        isChecked: false
      },
      {
        id: "bowo",
        url: '../../../assets/images/bowo.png',
        isChecked: false
      },
      {
        id: "Duve",
        url: '../../../assets/images/Duve.png',
        isChecked: false
      },
    ]
  }

  showMarketPlaceDetails(data: any) {
    switch (data) {
      case 'peekin':
        this._router.navigate(['/market-place-management', 'peekin-service']);
        break;
      case "BACnet":
        this._router.navigate(['/market-place-management', 'bacnet-service']);
        break;
      case "bowo":
        this._router.navigate(['/market-place-management', 'bowo-service']);
        break;
      case "Concierge":
        this._router.navigate(['/market-place-management', 'concierge-service']);
        break;
      case "Duve":
        this._router.navigate(['/market-place-management', 'duve-service']);
        break;
      case "LOUNGE":
        this._router.navigate(['/market-place-management', 'lounge-service']);
        break;
      case 'QUICKtest':
        this._router.navigate(['/market-place-management', 'quicktest-service']);
        break;
      case "nocore":
        this._router.navigate(['/market-place-management', 'nocore-service']);
        break;
      case "Niko":
        this._router.navigate(['/market-place-management', 'niko-service']);
        break;
      case "MQTT":
        this._router.navigate(['/market-place-management', 'mqtt-service']);
        break;
      case "alexa":
        this._router.navigate(['/market-place-management', 'alexa-service']);
        break;
    }
    
  }

  changeEvent(data: any) {
    console.log("change", data);
    let status = data.checked;
    let id = data.source.id;
    this.marketPlaces.map(marketPlace => {
      if (marketPlace.id == id) {
        marketPlace.isChecked = status;
      }
    })
  }
}
