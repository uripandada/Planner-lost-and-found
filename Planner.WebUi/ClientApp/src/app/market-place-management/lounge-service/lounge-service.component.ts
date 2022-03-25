import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lounge-service',
  templateUrl: './lounge-service.component.html',
  styleUrls: ['./lounge-service.component.css']
})
export class LoungeServiceComponent implements OnInit {

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
