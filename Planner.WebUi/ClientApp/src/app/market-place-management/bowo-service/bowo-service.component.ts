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
  }

  ngOnInit(): void {
    this.statusValue = history.state.option;
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }
}
