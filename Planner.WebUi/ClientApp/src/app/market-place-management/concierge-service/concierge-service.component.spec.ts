import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConciergeServiceComponent } from './concierge-service.component';

describe('ConciergeServiceComponent', () => {
  let component: ConciergeServiceComponent;
  let fixture: ComponentFixture<ConciergeServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ConciergeServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ConciergeServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
