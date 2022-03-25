import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-quicktest-service',
  templateUrl: './quicktest-service.component.html',
  styleUrls: ['./quicktest-service.component.css']
})
export class QuicktestServiceComponent implements OnInit {

  constructor(
    private _router: Router
  ) { }

  ngOnInit(): void {
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }
}
