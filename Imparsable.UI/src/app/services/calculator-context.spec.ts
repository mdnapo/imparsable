import { TestBed } from '@angular/core/testing';

import { CalculatorContext } from './calculator-context';

describe('CalculatorContext', () => {
  let service: CalculatorContext;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CalculatorContext);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
