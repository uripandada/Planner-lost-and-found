import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-alexa-service',
  templateUrl: './alexa-service.component.html',
  styleUrls: ['./alexa-service.component.css']
})
export class AlexaServiceComponent implements OnInit {

  constructor(
    private _router: Router
  ) { }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }

}
