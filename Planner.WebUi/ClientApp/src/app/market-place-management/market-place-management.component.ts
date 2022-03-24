import { Component, OnInit } from '@angular/core';

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

  constructor() {}
  
  ngOnInit(): void {
    this.marketPlaces = [
      {
        id: "alexa",
        url: '../../../assets/images/alexa.png',
        isChecked: false
      },
      {
        id: "BACnet",
        url: '../../../assets/images/BACnet.png',
        isChecked: false
      },
      {
        id: "bowo",
        url: '../../../assets/images/bowo.png',
        isChecked: false
      },
      {
        id: "Concierge",
        url: '../../../assets/images/Concierge.png',
        isChecked: false
      },
      {
        id: "Duve",
        url: '../../../assets/images/Duve.png',
        isChecked: false
      },
      {
        id: "LOUNGE",
        url: '../../../assets/images/LOUNGE.png',
        isChecked: false
      },
      {
        id: "MQTT",
        url: '../../../assets/images/MQTT.png',
        isChecked: false
      },
      {
        id: "Niko",
        url: '../../../assets/images/Niko.png',
        isChecked: false
      },
      {
        id: "nocore",
        url: '../../../assets/images/nocore.png',
        isChecked: false
      },
      {
        id: "peekin",
        url: '../../../assets/images/peekin.png',
        isChecked: false
      },
      {
        id: "QUICKtest",
        url: '../../../assets/images/QUICKtest.png',
        isChecked: false
      }
    ]
  }

  changeEvent(data: any) {
    let status = data.checked;
    let id = data.source.id;
    this.marketPlaces.map(marketPlace => {
      if (marketPlace.id == id) {
        marketPlace.isChecked = status;
        console.log(marketPlace);
      }
    })
  }
}
