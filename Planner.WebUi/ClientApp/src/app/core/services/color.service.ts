import { BehaviorSubject, Observable } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class ColorService {
  constructor() {
  }

  public GenerateRandomPastelColorHex() {

    let h = 360 * Math.random();
    let s = 45 + 40 * Math.random();
    let l = 80 + 10 * Math.random();

    return this._hslToHex(h, s, l);
  }

  private _hslToHex(hueDegrees, saturationPercent, lightnessPercent) {
    let l = lightnessPercent / 100;
    const a = saturationPercent * Math.min(l, 1 - l) / 100;
    const f = n => {
      const k = (n + hueDegrees / 30) % 12;
      const color = l - a * Math.max(Math.min(k - 3, 9 - k, 1), -1);
      return Math.round(255 * color).toString(16).padStart(2, '0');   // convert to Hex and prefix "0" if needed
    };
    return `#${f(0)}${f(8)}${f(4)}`;
  }
}
