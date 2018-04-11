import { Component, OnInit, Injector } from '@angular/core';
import { MindfightDto, RegistrationDto, MindfightServiceProxy, RegistrationServiceProxy, PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import * as moment from 'moment';
import { DatepickerOptionsService } from '../../services/datepickerOptions.service';

@Component({
    selector: 'app-edit-mindfight',
    templateUrl: './edit-mindfight.component.html',
    styleUrls: ['./edit-mindfight.component.css']
})
export class EditMindfightComponent extends AppComponentBase implements OnInit {
    mindfight: MindfightDto = null;
    selectedDate: any = {};
    singleDatepickerOptions: any;
    rangeDatepickerOptions: any;
    selectedMindfightType: string;
    registrations: RegistrationDto[] = [];
    mindfightId: number;
    private routeSubscriber: any;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private registrationService: RegistrationServiceProxy,
        private playerService: PlayerServiceProxy,
        private datepickerOptionsService: DatepickerOptionsService,
        private route: ActivatedRoute,
        private location: Location
    ) {
        super(injector);
    }

    ngOnInit() {
        this.singleDatepickerOptions = this.datepickerOptionsService.getSingleDatepickerOptions();
        this.rangeDatepickerOptions = this.datepickerOptionsService.getRangeDatepickerOptions();
        this.selectedDate = this.datepickerOptionsService.getInitialDate();
        this.mindfight = new MindfightDto();
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId']; // (+) converts string 'id' to a number
        });
        this.getMindfight(this.mindfightId);
        this.getRegistrations(this.mindfightId);
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            this.selectedMindfightType = this.mindfight.endTime !== null ? "2" : "1";
            this.selectedDate.startTime = result.startTime;
            this.selectedDate.endTime = result.endTime;
            console.log(this.selectedMindfightType);
        });
    }

    getRegistrations(mindfightId): void {
        this.registrationService.getMindfightRegistrations(mindfightId).subscribe((result) => {
            this.registrations = result;
        });
    }

    updateConfirmation(registration): void {
        this.registrationService.updateConfirmation(registration.mindfightId,
            registration.teamId, !registration.isConfirmed).subscribe(() => {
                if (registration.isConfirmed) {
                    this.notify.success("Komanda '" + registration.teamName + "' sėkmingai atšaukta!");
                } else {
                    this.notify.success("Komanda '" + registration.teamName + "' sėkmingai patvirtinta!")
                }
                this.getRegistrations(this.mindfightId);
            },
                () => this.notify.error("Klaida keičiant komandos patvirtinimo statusą")
            );
    }

    goBack() {
        this.location.back();
    }
}
