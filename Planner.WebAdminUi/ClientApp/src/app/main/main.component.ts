import { Component } from "@angular/core";
import { LoadingService } from '../core/services/loading.service';

@Component({
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.scss']
})
export class MainComponent {
  constructor(public loading: LoadingService) {

  }
}
