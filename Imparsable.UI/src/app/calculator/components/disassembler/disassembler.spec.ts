import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Disassembler } from './disassembler';

describe('Disassembler', () => {
  let component: Disassembler;
  let fixture: ComponentFixture<Disassembler>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Disassembler],
    }).compileComponents();

    fixture = TestBed.createComponent(Disassembler);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
