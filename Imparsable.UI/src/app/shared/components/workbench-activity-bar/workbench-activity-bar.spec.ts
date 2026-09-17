import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkbenchActivityBar } from './workbench-activity-bar';

describe('WorkbenchActivityBar', () => {
  let component: WorkbenchActivityBar;
  let fixture: ComponentFixture<WorkbenchActivityBar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkbenchActivityBar],
    }).compileComponents();

    fixture = TestBed.createComponent(WorkbenchActivityBar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
