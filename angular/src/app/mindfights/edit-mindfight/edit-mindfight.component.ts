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
    registrations: RegistrationDto[] = [];
    mindfightId: number;
    saving: boolean = false;
    private routeSubscriber: any;
    loaded = false;

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
        this.mindfight = new MindfightDto();
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
        });
        this.getMindfight(this.mindfightId);
        this.getRegistrations(this.mindfightId);
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    updateMindfight(): void {
        this.saving = true;
        let evaluatorsString = this.usersAllowedToEvaluate.replace(/\s/g, '');
        let evaluators = evaluatorsString.split(",");
        this.mindfight.usersAllowedToEvaluate = [];
        this.mindfight.startTime = moment.utc(this.selectedDate.startDate.format('YYYY-MM-DD HH:mm'),
            'YYYY-MM-DD HH:mm');
        evaluators.forEach((evaluator) => {
            this.mindfight.usersAllowedToEvaluate.push(evaluator);
        });
        this.mindfightService.updateMindfight(this.mindfight).subscribe(
            () => {
                this.notify.success('Protmūšis sėkmingai atnaujintas!', 'Atlikta');
            },
            () => {
                this.notify.error('Protmūšio nepavyko atnaujinti!', 'Klaida');
            },
            () => {
                this.router.navigate(['../../' + this.mindfightId], { relativeTo: this.route });
                this.saving = false;
            }
        );
    }

    deleteMindfight(): void {
        abp.message.confirm(
            'Protmūšis bus ištrintas.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.mindfightService.deleteMindfight(this.mindfightId).subscribe(
                        () => {
                            this.notify.success('Protmūšis sėkmingai ištrintas!', 'Atlikta');
                        },
                        () => {
                            this.notify.error('Protmūšio nepavyko ištrinti!', 'Klaida');
                        },
                        () => {
                            this.router.navigate(['../../administrate'], { relativeTo: this.route });
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
            this.selectedDate.startDate = result.startTime;
            this.singleDatepickerOptions.startDate = this.datepickerOptionsService.getDatetimeStringFromMoment(result.startTime);
            this.loaded = true;
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
                if (registrationIndex >= 0) {
                    this.registrations[registrationIndex].isConfirmed = !registration.isConfirmed;
                }
            },
            () => this.notify.error("Klaida keičiant komandos patvirtinimo statusą")
            );
    }

    selectedDateEvent(value: any) {
        this.selectedDate.startDate = value.start;
    }

    goToTours() {
        this.location.back();
    }

    goBack() {
        this.location.back();
    }
}
