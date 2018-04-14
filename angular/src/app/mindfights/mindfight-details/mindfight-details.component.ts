import { Component, OnInit, Injector } from '@angular/core';
import { MindfightDto, RegistrationDto, MindfightServiceProxy, RegistrationServiceProxy, PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute, Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-mindfight-details',
  templateUrl: './mindfight-details.component.html',
  styleUrls: ['./mindfight-details.component.css']
})
export class MindfightDetailsComponent extends AppComponentBase implements OnInit {
    mindfight: MindfightDto;
    registrations: RegistrationDto[] = [];
    mindfightId: number;
    playerInfo: PlayerDto;
    private routeSubscriber: any;

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
        this.getPlayerTeam();
        this.routeSubscriber = this.activatedRoute.params.subscribe(params => {
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
            console.log(result);
        });
    }

    getRegistrations(mindfightId): void {
        this.registrationService.getMindfightRegistrations(mindfightId).subscribe((result) => {
            this.registrations = result;
            console.log(result);
        });
    }

    isPlayerTeamRegistered(registrationTeamId): boolean {
        return this.playerInfo.teamId === registrationTeamId;
    };

    getPlayerTeam(): void {
        this.playerService.getPlayerInfo(abp.session.userId).subscribe((result) => {
            this.playerInfo = result;
        });
    };

    isMindfightCreator(): boolean {
        return abp.session.userId === this.mindfight.creatorId;
    }

    canEditMindfight(): boolean {
        return this.isMindfightCreator() || abp.auth.isGranted("ManageMindfights");
    }

    goToEdit(mindfightId) {
        this.router.navigate(['../edit', mindfightId], { relativeTo: this.activatedRoute });
    }

    goBack() {
        this.location.back();
    }
}
