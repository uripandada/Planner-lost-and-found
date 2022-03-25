import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nocore-service',
  templateUrl: './nocore-service.component.html',
  styleUrls: ['./nocore-service.component.css']
})
export class NocoreServiceComponent implements OnInit {

  constructor(
    private _router: Router
  ) { }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }
}
