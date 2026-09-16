import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculatorMemory } from './calculator-memory';

describe('CalculatorMemory', () => {
  let component: CalculatorMemory;
  let fixture: ComponentFixture<CalculatorMemory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalculatorMemory],
    }).compileComponents();

    fixture = TestBed.createComponent(CalculatorMemory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
