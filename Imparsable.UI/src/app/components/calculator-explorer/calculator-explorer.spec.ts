import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CalculatorExplorer } from './calculator-explorer';

describe('CalculatorExplorer', () => {
  let component: CalculatorExplorer;
  let fixture: ComponentFixture<CalculatorExplorer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CalculatorExplorer],
    }).compileComponents();

    fixture = TestBed.createComponent(CalculatorExplorer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
