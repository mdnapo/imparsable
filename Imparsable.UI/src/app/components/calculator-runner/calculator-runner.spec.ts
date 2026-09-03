import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculatorRunner } from './calculator-runner';

describe('CalculatorRunner', () => {
  let component: CalculatorRunner;
  let fixture: ComponentFixture<CalculatorRunner>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalculatorRunner],
    }).compileComponents();

    fixture = TestBed.createComponent(CalculatorRunner);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
