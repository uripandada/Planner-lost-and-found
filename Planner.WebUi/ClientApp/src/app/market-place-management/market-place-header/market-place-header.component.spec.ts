import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MarketPlaceHeaderComponent } from './market-place-header.component';

describe('MarketPlaceHeaderComponent', () => {
  let component: MarketPlaceHeaderComponent;
  let fixture: ComponentFixture<MarketPlaceHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MarketPlaceHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MarketPlaceHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
