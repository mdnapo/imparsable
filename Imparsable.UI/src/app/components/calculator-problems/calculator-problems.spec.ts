import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculatorProblems } from './calculator-problems';

describe('CalculatorProblems', () => {
  let component: CalculatorProblems;
  let fixture: ComponentFixture<CalculatorProblems>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalculatorProblems],
    }).compileComponents();

    fixture = TestBed.createComponent(CalculatorProblems);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
