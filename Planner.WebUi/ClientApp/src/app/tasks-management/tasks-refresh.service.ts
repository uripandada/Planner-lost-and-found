import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

export class TasksRefreshMessage {
  doRefresh: boolean;
}

@Injectable({ providedIn: 'root' })
export class TasksRefreshService {

  constructor() {
    this.refreshGrids$ = new BehaviorSubject<TasksRefreshMessage>({ doRefresh: false });
    this.taskStatus$ = new BehaviorSubject<{ taskId: string, taskConfigurationId: string, statusKey: string }>({ taskId: null, taskConfigurationId: null, statusKey: null });
  }

  public refreshGrids$: BehaviorSubject<TasksRefreshMessage>;
  public taskStatus$: BehaviorSubject<{ taskId: string, taskConfigurationId: string, statusKey: string }>;

  refreshGrids() {
    this.refreshGrids$.next({ doRefresh: true });
  }

  taskStatusChanged(change: { taskId: string, taskConfigurationId: string, statusKey: string }) {
    this.taskStatus$.next(change);
  }
}
