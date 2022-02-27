import { BehaviorSubject, Observable } from "rxjs";
import { Injectable } from "@angular/core";

@Injectable({ providedIn: 'root' })
export class RoomTypeService {
  public getRoomTypes(): Array<{ key: string, value: string, group: string }> {
    return [
      { key: "HOTEL", value: "Hotel room", group: "HOTEL_HOSTEL" },
      { key: "HOSTEL", value: "Hostel room", group: "HOTEL_HOSTEL" },
      { key: "APPARTMENT", value: "Appartment", group: "APPARTMENT" },
    ];
  }
}
