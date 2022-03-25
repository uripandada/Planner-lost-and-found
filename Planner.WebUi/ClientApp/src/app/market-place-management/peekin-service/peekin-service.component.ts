import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-peekin-service',
  templateUrl: './peekin-service.component.html',
  styleUrls: ['./peekin-service.component.css']
})
export class PeekinServiceComponent implements OnInit {

  public statusValue: any

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
  ) {
    this.statusValue = this._router.getCurrentNavigation().extras.state.option;
  }
  
  ngOnInit(): void {
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
