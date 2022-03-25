import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DuveServiceComponent } from './duve-service.component';

describe('DuveServiceComponent', () => {
  let component: DuveServiceComponent;
  let fixture: ComponentFixture<DuveServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DuveServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DuveServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
