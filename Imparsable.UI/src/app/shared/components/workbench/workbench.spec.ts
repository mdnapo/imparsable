import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Workbench } from './workbench';

describe('Workbench', () => {
  let component: Workbench;
  let fixture: ComponentFixture<Workbench>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Workbench],
    }).compileComponents();

    fixture = TestBed.createComponent(Workbench);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
