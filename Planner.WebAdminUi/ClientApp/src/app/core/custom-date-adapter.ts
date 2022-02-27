import { Injectable } from '@angular/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';

@Injectable()
export class CustomDateAdapter extends MomentDateAdapter {

  getFirstDayOfWeek(): number {
    return 1;
  }

}
