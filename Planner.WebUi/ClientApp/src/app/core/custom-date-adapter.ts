import { Injectable } from '@angular/core';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { NativeDateAdapter } from '@angular/material/core';

@Injectable()
export class CustomDateAdapter extends MomentDateAdapter {

  getFirstDayOfWeek(): number {
    return 1;
  }

}
