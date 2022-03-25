import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-peekin-service',
  templateUrl: './peekin-service.component.html',
  styleUrls: ['./peekin-service.component.css']
})
export class PeekinServiceComponent implements OnInit {

  public statusValue: boolean;
  @Output() clickEvent = new EventEmitter();

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
  ) { }
  
  ngOnInit(): void {
    this.statusValue = history.state.option;
  }

  backPage() {
    this._router.navigate(['/market-place-management']);
  }

  changeActivate(value: any){
    switch (value) {
      case 1:
        console.log("dfdfdfdfdfd");
        break;
      case 2:
        console.log("hhhhhhhhh");
        break;
      default:
        break;
    }
  }

}
