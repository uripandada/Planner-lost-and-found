import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
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
    private _router: Router,
    private _route: ActivatedRoute,
    ) {}
    
    ngOnInit(): void {
      this.marketPlaces = [
        {
          id: "peekin",
          url: '../../../assets/images/peekin.png',
          isChecked: true
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

  showMarketPlaceDetails(data: any, status: any) {
    switch (data) {
      case 'peekin':
        this._router.navigate(['/market-place-management/peekin-service'], { state: { option: status.isChecked }});
        break;
      case "BACnet":
        this._router.navigate(['/market-place-management/bacnet-service'], { state: { option: status.isChecked }});
        break;
      case "bowo":
        this._router.navigate(['/market-place-management/bowo-service'], { state: { option: status.isChecked }});
        break;
      case "Concierge":
        this._router.navigate(['/market-place-management/concierge-service'], { state: { option: status.isChecked }});
        break;
      case "Duve":
        this._router.navigate(['/market-place-management/duve-service'], { state: { option: status.isChecked }});
        break;
      case "LOUNGE":
        this._router.navigate(['/market-place-management/lounge-service'], { state: { option: status.isChecked }});
        break;
      case 'QUICKtest':
        this._router.navigate(['/market-place-management/quicktest-service'], { state: { option: status.isChecked }});
        break;
      case "nocore":
        this._router.navigate(['/market-place-management/nocore-service'], { state: { option: status.isChecked }});
        break;
      case "Niko":
        this._router.navigate(['/market-place-management/niko-service'], { state: { option: status.isChecked }});
        break;
      case "MQTT":
        this._router.navigate(['/market-place-management/mqtt-service'], { state: { option: status.isChecked }});
        break;
      case "alexa":
        this._router.navigate(['/market-place-management/alexa-service'], { state: { option: status.isChecked }});
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
