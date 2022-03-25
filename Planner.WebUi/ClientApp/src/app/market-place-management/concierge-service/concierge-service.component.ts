import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-concierge-service',
  templateUrl: './concierge-service.component.html',
  styleUrls: ['./concierge-service.component.css']
})
export class ConciergeServiceComponent implements OnInit {

  constructor(
    private _router: Router
  ) { }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }
}
