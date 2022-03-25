import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-concierge-service',
  templateUrl: './concierge-service.component.html',
  styleUrls: ['./concierge-service.component.css']
})
export class ConciergeServiceComponent implements OnInit {

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
}
