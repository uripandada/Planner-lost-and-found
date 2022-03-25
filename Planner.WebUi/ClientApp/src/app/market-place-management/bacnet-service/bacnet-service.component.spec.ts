import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BacnetServiceComponent } from './bacnet-service.component';

describe('BacnetServiceComponent', () => {
  let component: BacnetServiceComponent;
  let fixture: ComponentFixture<BacnetServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BacnetServiceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BacnetServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
