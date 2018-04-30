import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MindfightCreateUpdateDto, MindfightServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { DatepickerOptionsService } from '../../services/datepickerOptions.service';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-create-mindfight',
    templateUrl: './create-mindfight.component.html',
    styleUrls: ['./create-mindfight.component.css'],
    animations: [appModuleAnimation()]
})
export class CreateMindfightComponent extends AppComponentBase implements OnInit {
    mindfight: MindfightCreateUpdateDto = null;
    selectedDate: any = {};
    singleDatepickerOptions: any;
    rangeDatepickerOptions: any;
    selectedMindfightType: string;
    saving = false;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private datepickerOptionsService: DatepickerOptionsService,
        private route: ActivatedRoute,
        private router: Router) {
        super(injector);
    }

    ngOnInit() {
        this.singleDatepickerOptions = this.datepickerOptionsService.getSingleDatepickerOptions();
        this.singleDatepickerOptions.startDate = this.datepickerOptionsService.initialDate.startDate;
        this.selectedDate = this.datepickerOptionsService.getInitialDate();
        this.mindfight = new MindfightCreateUpdateDto();
    }

    selectedDateEvent(value: any) {
        this.selectedDate.startDate = value.start;
    }

    createMindfight(): void {
        var that = this;
        this.mindfight.startTime = moment.utc(this.selectedDate.startDate.format('YYYY-MM-DD HH:mm'),
            'YYYY-MM-DD HH:mm');
        that.saving = true;
        this.mindfightService.createMindfight(this.mindfight).subscribe((createdMindfightId) => {
            that.notify.success("Protmūšis sėkmingai sukurtas!");
            that.router.navigate(['../' + createdMindfightId + '/edit'], { relativeTo: that.route });
            that.saving = false;
        });
    }
}
