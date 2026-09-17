import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkbenchEditor } from './workbench-editor';

describe('WorkbenchEditor', () => {
  let component: WorkbenchEditor;
  let fixture: ComponentFixture<WorkbenchEditor>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WorkbenchEditor],
    }).compileComponents();

    fixture = TestBed.createComponent(WorkbenchEditor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
