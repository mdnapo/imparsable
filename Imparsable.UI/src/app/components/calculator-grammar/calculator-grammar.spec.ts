import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculatorGrammar } from './calculator-grammar';

describe('CalculatorGrammar', () => {
  let component: CalculatorGrammar;
  let fixture: ComponentFixture<CalculatorGrammar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalculatorGrammar],
    }).compileComponents();

    fixture = TestBed.createComponent(CalculatorGrammar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
