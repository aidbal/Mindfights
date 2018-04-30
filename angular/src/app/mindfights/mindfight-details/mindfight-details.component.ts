import { Component, OnInit, Injector } from '@angular/core';
import {
    MindfightDto, RegistrationDto, MindfightServiceProxy,
    RegistrationServiceProxy, PlayerServiceProxy, PlayerDto
} from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';
import * as moment from 'moment';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-mindfight-details',
    templateUrl: './mindfight-details.component.html',
    styleUrls: ['./mindfight-details.component.css'],
    animations: [appModuleAnimation()]
})
export class MindfightDetailsComponent extends AppComponentBase implements OnInit {
    mindfight: MindfightDto;
    registrations: RegistrationDto[] = [];
    mindfightId: number;
    playerInfo: PlayerDto;
    private routeSubscriber: any;
    canEvaluate = false;
    canEdit = false;
    hasMindfightStarted = false;
    showPlayButton = false;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
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
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    checkShowPlayButton(): void {
        var that = this;
        this.showPlayButton = false;
        if (
            !this.mindfight.isFinished
            && moment(this.mindfight.startTime).diff(moment().add(10, 'minutes')) <= 0
            && moment(this.mindfight.startTime).add(this.mindfight.prepareTime, 'minutes').diff(moment()) >= 0
            ) {
            const currentRegistrationIndex =
                this.registrations.findIndex(registration => registration.teamId === that.playerInfo.teamId);
            if (currentRegistrationIndex >= 0) {
                if (this.registrations[currentRegistrationIndex].isConfirmed && this.playerInfo.isActiveInTeam) {
                    this.showPlayButton = true;
                }
            }
        }
    }

    checkMindfightStarted(): void {
        if (moment(this.mindfight.startTime).add(this.mindfight.prepareTime, 'minutes').diff(moment()) <= 0) {
            this.hasMindfightStarted = true;
        }
    }

    onRegisterChange(registrationChangeObject): void {
        if (registrationChangeObject.createEvent) {
            this.addNewRegistration(registrationChangeObject.registrationId);
        } else {
            const currentRegistrationIndex =
                this.registrations.findIndex(registration => registration.id === registrationChangeObject.registrationId);
            if (currentRegistrationIndex >= 0) {
                this.registrations.splice(currentRegistrationIndex, 1);
            }
            this.checkShowPlayButton();
        }
    }

    addNewRegistration(registrationId): void {
        this.registrationService.getRegistration(registrationId).subscribe((result) => {
            this.registrations.push(result);
        });
    }

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            this.canEditMindfight();
            this.canEvaluateMindfight();
            this.checkMindfightStarted();
            this.getRegistrations(mindfightId);
        });
    }

    getRegistrations(mindfightId): void {
        this.registrationService.getMindfightRegistrations(mindfightId).subscribe((result) => {
            this.registrations = result;
            this.checkShowPlayButton();
        });
    }

    isPlayerTeamRegistered(registrationTeamId): boolean {
        return this.playerInfo.teamId === registrationTeamId;
    };

    getPlayerInfo(): void {
        this.playerService.getPlayerInfo(abp.session.userId).subscribe((result) => {
            this.playerInfo = result;
        });
    };

    isMindfightCreator(): boolean {
        return abp.session.userId === this.mindfight.creatorId;
    }

    canEditMindfight() {
        if (this.isMindfightCreator() || abp.auth.isGranted("ManageMindfights")) {
            this.canEdit = true;
        }
    }

    canEvaluateMindfight() {
        var that = this;
        if (this.canEditMindfight()) {
            this.canEvaluate = true;
        } else {
            if (this.mindfight.usersAllowedToEvaluate && that.playerInfo) {
                this.mindfight.usersAllowedToEvaluate.forEach((userEmail) => {
                    if (userEmail.toUpperCase() === that.playerInfo.emailAddress.toUpperCase()) {
                        that.canEvaluate = true;
                        return false;
                    }
                });
            }
        }
    }

    goBack() {
        this.location.back();
    }
}
