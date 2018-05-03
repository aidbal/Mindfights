import { TestBed, inject } from '@angular/core/testing';

import { TeamStateService } from './team-state.service';

describe('TeamStateService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [TeamStateService]
    });
  });

  it('should be created', inject([TeamStateService], (service: TeamStateService) => {
    expect(service).toBeTruthy();
  }));
});
