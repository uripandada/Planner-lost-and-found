import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-niko-service',
  templateUrl: './niko-service.component.html',
  styleUrls: ['./niko-service.component.css']
})
export class NikoServiceComponent implements OnInit {

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
