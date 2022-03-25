import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MqttServiceComponent } from './mqtt-service.component';

describe('MqttServiceComponent', () => {
  let component: MqttServiceComponent;
  let fixture: ComponentFixture<MqttServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MqttServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MqttServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
