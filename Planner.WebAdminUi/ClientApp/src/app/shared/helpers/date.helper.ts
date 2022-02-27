import moment, { Moment } from 'moment';

export class DateHelper {
  static getDate(date: Date) {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate());
  }

  static get today() {
    return DateHelper.getDate(new Date());
  }

  static addDays(date: Date, daysCount: number): Date {
    const result = new Date(date);
    result.setDate(result.getDate() + daysCount);
    return result;
  }

  static addHours(date: Date, hoursCount: number): Date {
    const result = new Date(date);
    result.setHours(result.getHours() + hoursCount);
    return result;
  }

  static addMomentTime(date: Moment, time: string): Moment {
    return moment(`${this.getMomentDateString(date)}T${time}`);
  }

  static addTime(date: Date, time: string): Date {
    return new Date(`${this.getDateString(date)}T${time}`);
  }

  static getMomentDateString(date: Moment): string {
    return `${date.year()}-${('0' + (date.month() + 1)).slice(-2)}-${('0' + date.date()).slice(-2)}`;
  }

  static getDateString(date: Date): string {
    return `${date.getFullYear()}-${('0' + (date.getMonth() + 1)).slice(-2)}-${('0' + date.getDate()).slice(-2)}`;
  }

  static getDateTimeString(date: Date) {
    return `${this.getDateString(date)}T${('0' + date.getHours()).slice(-2)}:${('0' + date.getMinutes()).slice(-2)}:${('0' + date.getSeconds()).slice(-2)}`;
  }

  static getTime(date: Date): string {
    return `${('0' + date.getHours()).slice(-2)}:${('0' + date.getMinutes()).slice(-2)}`;
  }

  static getMomentTime(date: Moment): string {
    return date.format("HH:mm");
  }

  static getMinutes(time: string) {
    const parts = time.split(':');
    return 60 * Number.parseInt(parts[0], 10) + Number.parseInt(parts[1], 10);
  }

  static getWeekDates(date: Date): { startOfTheWeek: Date, endOfTheWeek: Date } {
    var dt = <Date>(date || new Date()); // current date of week
    var currentWeekDay = dt.getDay();
    var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1;
    var wkStart = new Date(new Date(dt).setDate(dt.getDate() - lessDays));
    var wkEnd = new Date(new Date(wkStart).setDate(wkStart.getDate() + 6));

    return {
      endOfTheWeek: wkEnd,
      startOfTheWeek: wkStart
    };
  }

  static getWeekMomentDates(date: Moment): { startOfTheWeek: Moment, endOfTheWeek: Moment } {
    let currentWeekDay = date.isoWeekday();
    let subtractDays = currentWeekDay - 1;
    let weekStart: Moment = date.subtract(subtractDays, 'days').clone();
    let weekEnd: Moment = weekStart.clone().add(6, 'days');
    
    return {
      endOfTheWeek: weekEnd,
      startOfTheWeek: weekStart
    };
  }

  static getUtcTodayDateMoment(): Moment {
    let date: Moment = moment(new Date());
    return moment.utc(date.startOf('day').format('LL')).startOf('day');
  }

  static getUtcMoment(date: Moment): Moment {
    return moment.utc(date.clone().startOf('day').format('LL')).startOf('day');
  }

  static getUtcDate(date: Moment): Date {
    return moment.utc(date.clone().startOf('day').format('LL')).startOf('day').toDate();
  }
}
