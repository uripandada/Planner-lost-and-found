import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NocoreServiceComponent } from './nocore-service.component';

describe('NocoreServiceComponent', () => {
  let component: NocoreServiceComponent;
  let fixture: ComponentFixture<NocoreServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NocoreServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NocoreServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
