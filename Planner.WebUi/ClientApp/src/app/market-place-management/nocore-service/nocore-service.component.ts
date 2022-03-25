import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-nocore-service',
  templateUrl: './nocore-service.component.html',
  styleUrls: ['./nocore-service.component.css']
})
export class NocoreServiceComponent implements OnInit {

  public statusValue: any

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
  ) { 
  }

  ngOnInit(): void {
    this.statusValue = history.state.option;
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }
}
