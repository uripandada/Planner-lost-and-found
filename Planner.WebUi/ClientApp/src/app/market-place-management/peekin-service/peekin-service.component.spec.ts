import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PeekinServiceComponent } from './peekin-service.component';

describe('PeekinServiceComponent', () => {
  let component: PeekinServiceComponent;
  let fixture: ComponentFixture<PeekinServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PeekinServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PeekinServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
