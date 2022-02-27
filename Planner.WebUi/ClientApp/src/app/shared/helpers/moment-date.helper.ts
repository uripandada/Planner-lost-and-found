import moment, { Moment } from 'moment';

export class MomentDateHelper {
  static getDateAtMidnight(date: Moment): Moment {
    return moment(this.getIsoDate(date) + 'T00:00:00+00:00');
  }

  static getDateTime(date: Moment, time: string): Moment {
    return moment(`${this.getIsoDate(date)}T${time}:00+00:00`);
  }

  static getIsoDate(date: Moment): string {
    return date.format('YYYY-MM-DD');
  }
  static getIsoDateTime(date: Moment): string {
    return date.format('YYYY-MM-DDTHH:mm:ss');
  }
  static getIsoDateTimeWithTime(date: Moment, timeString: string): string {
    return date.format(this.getIsoDate(date) + 'T' + timeString + ':00');
  }

  static isDateToday(date: Moment): boolean {
    let d: Moment = this.getDateAtMidnight(date);
    return d.isSame(moment(), "day");
  }

  static nowDate(): Moment {
    return moment().startOf('day');
  }
}
