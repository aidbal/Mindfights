import { TestBed, inject } from '@angular/core/testing';

import { DatepickerOptionsService } from './datepickerOptions.service';

describe('DatepickerService', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [DatepickerOptionsService]
        });
    });

    it('should be created', inject([DatepickerOptionsService], (service: DatepickerOptionsService) => {
        expect(service).toBeTruthy();
    }));
});
