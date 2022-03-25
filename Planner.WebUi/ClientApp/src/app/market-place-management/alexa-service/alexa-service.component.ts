import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-alexa-service',
  templateUrl: './alexa-service.component.html',
  styleUrls: ['./alexa-service.component.css']
})
export class AlexaServiceComponent implements OnInit {

  public statusValue: any

  constructor(
    private _router: Router
  ) {
  }

  ngOnInit(): void {
    this.statusValue = history.state.option;
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }

  changeActivate(value: any){
    switch (value) {
      case 1:
        console.log("dfdfdfdfdfd");
        break;
      case 2:
        console.log("hhhhhhhhh");
        break;
      default:
        break;
    }
  }

}
