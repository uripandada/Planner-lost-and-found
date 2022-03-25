import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-peekin-service',
  templateUrl: './peekin-service.component.html',
  styleUrls: ['./peekin-service.component.css']
})
export class PeekinServiceComponent implements OnInit {

  constructor(
    private _router: Router
  ) { }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }

}
