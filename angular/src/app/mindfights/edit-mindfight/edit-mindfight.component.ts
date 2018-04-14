import { Component, OnInit, Injector } from '@angular/core';
import { MindfightDto, RegistrationDto, MindfightServiceProxy, RegistrationServiceProxy, PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
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
    usersAllowedToEvaluate: string = null;
    selectedDate: any = {};
    singleDatepickerOptions: any;
    rangeDatepickerOptions: any;
    selectedMindfightType: string;
    registrations: RegistrationDto[] = [];
    mindfightId: number;
    saving: boolean = false;
    private routeSubscriber: any;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private registrationService: RegistrationServiceProxy,
        private playerService: PlayerServiceProxy,
        private datepickerOptionsService: DatepickerOptionsService,
        private route: ActivatedRoute,
        private location: Location,
        private router: Router
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

    updateMindfight(): void {
        this.saving = true;
        this.mindfight.endTime = this.selectedMindfightType === "2" ? this.selectedDate.endTime : null;
        var evaluatorsString = this.usersAllowedToEvaluate.replace(/\s/g, '');
        var evaluators = evaluatorsString.split(",");
        this.mindfight.usersAllowedToEvaluate = [];
        evaluators.forEach((evaluator) => {
            this.mindfight.usersAllowedToEvaluate.push(evaluator);
        });
        this.mindfightService.updateMindfight(this.mindfight).subscribe(
            () => {
                abp.message.success('Protmūšis sėkmingai atnaujintas!', 'Atlikta');
            },
            () => {
                abp.message.error('Protmūšio nepavyko atnaujinti!', 'Klaida');
            },
            () => {
                this.router.navigate(['../../' + this.mindfightId], { relativeTo: this.route });
                this.saving = false;
            }
        );
    }

    deleteMindfight(): void {
        var that = this;
        abp.message.confirm(
            'Protmūšis bus ištrintas.',
            'Are Jūs esate tikri?',
            function (isConfirmed) {
                if (isConfirmed) {
                    that.mindfightService.deleteMindfight(that.mindfightId).subscribe(
                        () => {
                            abp.message.success('Protmūšis sėkmingai ištrintas!', 'Atlikta');
                        },
                        () => {
                            abp.message.error('Protmūšio nepavyko ištrinti!', 'Klaida');
                        },
                        () => {
                            that.router.navigate(['../../administrate'], { relativeTo: that.route });
                        }
                    );
                }
            }
        );
    }

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            this.usersAllowedToEvaluate = "";
            result.usersAllowedToEvaluate.forEach((evaluator, index) => {
                if (index !== 0) {
                    this.usersAllowedToEvaluate += ", ";
                }
                this.usersAllowedToEvaluate += evaluator;
            });
            this.selectedMindfightType = this.mindfight.endTime !== null ? "2" : "1";
            this.selectedDate.startTime = result.startTime;
            this.selectedDate.endTime = result.endTime;
        });
    }

    getRegistrations(mindfightId): void {
        this.registrationService.getMindfightRegistrations(mindfightId).subscribe((result) => {
            this.registrations = result;
        });
    }

    updateConfirmation(registration): void {
        this.registrationService.updateConfirmation(registration.mindfightId,
            registration.teamId,
            !registration.isConfirmed).subscribe(() => {
                if (registration.isConfirmed) {
                    this.notify.success("Komanda '" + registration.teamName + "' sėkmingai atšaukta!");
                } else {
                    this.notify.success("Komanda '" + registration.teamName + "' sėkmingai patvirtinta!")
                }
                let registrationIndex = this.registrations.findIndex(i => i.mindfightId === registration.mindfightId &&
                    i.teamId === registration.teamId);
                if (registrationIndex) {
                    this.registrations[registrationIndex].isConfirmed = !registration.isConfirmed;
                }
            },
            () => this.notify.error("Klaida keičiant komandos patvirtinimo statusą")
            );
    }

    goToTours() {
        this.location.back();
    }

    goBack() {
        this.location.back();
    }
}
