import moment, { Moment } from 'moment';

export class MomentDateHelper {
  static getDateAtMidnight(date: Moment): Moment {
    return moment(date.format('YYYY-MM-DD') + 'T00:00:00+00:00');
  }
}
