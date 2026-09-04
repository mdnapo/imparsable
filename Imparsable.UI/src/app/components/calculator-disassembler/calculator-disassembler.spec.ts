import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculatorDisassembler } from './calculator-disassembler';

describe('CalculatorDisassembler', () => {
  let component: CalculatorDisassembler;
  let fixture: ComponentFixture<CalculatorDisassembler>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalculatorDisassembler],
    }).compileComponents();

    fixture = TestBed.createComponent(CalculatorDisassembler);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
