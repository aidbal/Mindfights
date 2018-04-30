import { Component, OnInit, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
    MindfightServiceProxy, MindfightDto, MindfightResultDto,
    ResultServiceProxy, PlayerDto, PlayerServiceProxy,
    RegistrationDto, RegistrationServiceProxy
} from 'shared/service-proxies/service-proxies';
import { Location } from '@angular/common';
import { AppComponentBase } from 'shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-mindfight-result',
    templateUrl: './mindfight-result.component.html',
    styleUrls: ['./mindfight-result.component.css'],
    animations: [appModuleAnimation()]
})
export class MindfightResultComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    mindfight: MindfightDto;
    mindfightResults: MindfightResultDto[] = [];
    registrations: RegistrationDto[] = [];
    playerInfo: PlayerDto;
    isTeamRegistered = false;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private mindfightResultService: ResultServiceProxy,
        private registrationService: RegistrationServiceProxy,
        private playerService: PlayerServiceProxy,
        private activatedRoute: ActivatedRoute,
        private location: Location,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getPlayerInfo();
        this.routeSubscriber = this.activatedRoute.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
            this.getMindfight(this.mindfightId);
            this.getMindfightResults(this.mindfightId);
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getRegistrations(mindfightId): void {
        this.registrationService.getMindfightRegistrations(mindfightId).subscribe((result) => {
            this.registrations = result;
            this.registrations.forEach((registration) => {
                if (registration.teamId === this.playerInfo.teamId && registration.isConfirmed) {
                    this.isTeamRegistered = true;
                    return false;
                }
            });
        });
    }

    getPlayerInfo(): void {
        this.playerService.getPlayerInfo(abp.session.userId).subscribe((result) => {
            this.playerInfo = result;
            this.getRegistrations(this.mindfightId);
        });
    };

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            console.log(result);
        });
    }

    getMindfightResults(mindfightId): void {
        this.mindfightResultService.getMindfightResults(mindfightId).subscribe((result) => {
            this.mindfightResults = result;
            console.log(result);
        });
    }
}
