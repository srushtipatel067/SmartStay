import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Offers } from './offers';

describe('Offers', () => {
  let component: Offers;
  let fixture: ComponentFixture<Offers>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Offers],
    }).compileComponents();

    fixture = TestBed.createComponent(Offers);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
