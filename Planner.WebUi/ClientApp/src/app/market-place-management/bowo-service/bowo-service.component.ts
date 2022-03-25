import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bowo-service',
  templateUrl: './bowo-service.component.html',
  styleUrls: ['./bowo-service.component.css']
})
export class BowoServiceComponent implements OnInit {

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
