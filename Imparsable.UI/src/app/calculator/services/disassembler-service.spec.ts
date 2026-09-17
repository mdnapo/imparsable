import { TestBed } from '@angular/core/testing';

import { DisassemblerService } from './disassembler-service';

describe('DisassemblerService', () => {
  let service: DisassemblerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DisassemblerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
