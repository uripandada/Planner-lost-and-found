import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NikoServiceComponent } from './niko-service.component';

describe('NikoServiceComponent', () => {
  let component: NikoServiceComponent;
  let fixture: ComponentFixture<NikoServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NikoServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NikoServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
