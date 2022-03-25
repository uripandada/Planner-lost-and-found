import { ComponentFixture, TestBed } from '@angular/core/testing';

import { QuicktestServiceComponent } from './quicktest-service.component';

describe('QuicktestServiceComponent', () => {
  let component: QuicktestServiceComponent;
  let fixture: ComponentFixture<QuicktestServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ QuicktestServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(QuicktestServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
