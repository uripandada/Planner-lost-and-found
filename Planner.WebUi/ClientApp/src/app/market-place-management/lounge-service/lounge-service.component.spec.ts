import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoungeServiceComponent } from './lounge-service.component';

describe('LoungeServiceComponent', () => {
  let component: LoungeServiceComponent;
  let fixture: ComponentFixture<LoungeServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LoungeServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoungeServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
