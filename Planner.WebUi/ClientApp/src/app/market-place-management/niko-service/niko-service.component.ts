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
  }

  ngOnInit(): void {
    this.statusValue = history.state.option;
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }
}
