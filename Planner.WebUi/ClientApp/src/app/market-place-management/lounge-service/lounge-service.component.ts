import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lounge-service',
  templateUrl: './lounge-service.component.html',
  styleUrls: ['./lounge-service.component.css']
})
export class LoungeServiceComponent implements OnInit {

  constructor(
    private _router: Router
  ) { }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }

}
