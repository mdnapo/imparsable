import { TestBed } from '@angular/core/testing';

import { LanguageServer } from './language-server';

describe('LanguageServer', () => {
  let service: LanguageServer;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LanguageServer);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
