import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BowoServiceComponent } from './bowo-service.component';

describe('BowoServiceComponent', () => {
  let component: BowoServiceComponent;
  let fixture: ComponentFixture<BowoServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BowoServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BowoServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
