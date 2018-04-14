import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MindfightCreateUpdateDto, MindfightServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { DatepickerOptionsService } from '../../services/datepickerOptions.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-create-mindfight',
  templateUrl: './create-mindfight.component.html',
  styleUrls: ['./create-mindfight.component.css']
})
export class CreateMindfightComponent extends AppComponentBase implements OnInit {
    mindfight: MindfightCreateUpdateDto = null;
    selectedDate: any = {};
    singleDatepickerOptions: any;
    rangeDatepickerOptions: any;
    selectedMindfightType: string;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private datepickerOptionsService: DatepickerOptionsService,
        private location: Location,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        this.singleDatepickerOptions = this.datepickerOptionsService.getSingleDatepickerOptions();
        this.rangeDatepickerOptions = this.datepickerOptionsService.getRangeDatepickerOptions();
        this.selectedDate = this.datepickerOptionsService.getInitialDate();
        this.mindfight = new MindfightCreateUpdateDto();
    }

    selectedDateEvent(value: any) {
        this.selectedDate.startDate = value.start;
        this.selectedDate.endDate = value.end;
        console.log(this.selectedDate);
    }

    createMindfight(): void {
        if (this.selectedMindfightType === '1') {
            this.mindfight.startTime = this.selectedDate.startDate;
            this.mindfight.endTime = this.selectedDate.endDate;
        } else if (this.selectedMindfightType === '2') {
            this.mindfight.startTime = this.selectedDate.startDate;
            this.mindfight.endTime = null;
        }
        this.mindfightService.createMindfight(this.mindfight).subscribe(() => {
            this.notify.success("Protmūšis sėkmingai sukurtas!");
            this.router.navigate(['../administrate'], { relativeTo: this.route });
        });
        console.log(this.mindfight);
    }

    goBack() {
        this.location.back();
    }
}
