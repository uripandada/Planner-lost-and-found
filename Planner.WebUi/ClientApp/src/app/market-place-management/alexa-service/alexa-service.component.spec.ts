import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AlexaServiceComponent } from './alexa-service.component';

describe('AlexaServiceComponent', () => {
  let component: AlexaServiceComponent;
  let fixture: ComponentFixture<AlexaServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AlexaServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AlexaServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
