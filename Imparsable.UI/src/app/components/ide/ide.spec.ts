import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Ide2 } from './ide2';

describe('Ide2', () => {
  let component: Ide2;
  let fixture: ComponentFixture<Ide2>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Ide2],
    }).compileComponents();

    fixture = TestBed.createComponent(Ide2);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
