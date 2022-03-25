import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bacnet-service',
  templateUrl: './bacnet-service.component.html',
  styleUrls: ['./bacnet-service.component.css']
})
export class BacnetServiceComponent implements OnInit {

  constructor(
    private _router: Router
  ) { }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }
}
