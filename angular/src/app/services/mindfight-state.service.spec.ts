import { TestBed, inject } from '@angular/core/testing';

import { MindfightStateService } from './mindfight-state.service';

describe('MindfightStateService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MindfightStateService]
    });
  });

  it('should be created', inject([MindfightStateService], (service: MindfightStateService) => {
    expect(service).toBeTruthy();
  }));
});
