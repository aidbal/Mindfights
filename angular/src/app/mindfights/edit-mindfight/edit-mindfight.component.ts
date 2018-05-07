import { Component, OnInit, Injector } from '@angular/core';
import { MindfightDto, MindfightServiceProxy, PlayerServiceProxy } from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import * as moment from 'moment';
import { DatepickerOptionsService } from '../../services/datepickerOptions.service';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-edit-mindfight',
    templateUrl: './edit-mindfight.component.html',
    styleUrls: ['./edit-mindfight.component.css'],
    animations: [appModuleAnimation()]
})
export class EditMindfightComponent extends AppComponentBase implements OnInit {
    mindfight: MindfightDto = null;
    usersAllowedToEvaluate: string = null;
    selectedDate: any = {};
    singleDatepickerOptions: any;
    mindfightId: number;
    saving: boolean = false;
    private routeSubscriber: any;
    loaded = false;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
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

    selectedDateEvent(value: any) {
        this.selectedDate.startDate = value.start;
    }
}
