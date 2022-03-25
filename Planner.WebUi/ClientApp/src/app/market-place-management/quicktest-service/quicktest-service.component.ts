import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-quicktest-service',
  templateUrl: './quicktest-service.component.html',
  styleUrls: ['./quicktest-service.component.css']
})
export class QuicktestServiceComponent implements OnInit {

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
