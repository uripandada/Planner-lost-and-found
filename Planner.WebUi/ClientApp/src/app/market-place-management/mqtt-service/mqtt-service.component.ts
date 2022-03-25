import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-mqtt-service',
  templateUrl: './mqtt-service.component.html',
  styleUrls: ['./mqtt-service.component.css']
})
export class MqttServiceComponent implements OnInit {

  public statusValue: any

  constructor(
    private _router: Router
  ) { 
    this.statusValue = this._router.getCurrentNavigation().extras.state.option;
  }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }

}
