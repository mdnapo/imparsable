import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculatorIde } from './calculator-ide';

describe('CalculatorIde', () => {
  let component: CalculatorIde;
  let fixture: ComponentFixture<CalculatorIde>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalculatorIde],
    }).compileComponents();

    fixture = TestBed.createComponent(CalculatorIde);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
