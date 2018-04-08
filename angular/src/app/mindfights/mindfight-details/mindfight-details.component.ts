import { Component, OnInit, Injector } from '@angular/core';
import { MindfightDto, RegistrationDto, MindfightServiceProxy, RegistrationServiceProxy, PlayerServiceProxy } from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-mindfight-details',
  templateUrl: './mindfight-details.component.html',
  styleUrls: ['./mindfight-details.component.css']
})
export class MindfightDetailsComponent extends AppComponentBase implements OnInit {
    mindfight: MindfightDto;
    registrations: RegistrationDto[] = [];
    mindfightId: number;
    playerTeamId: number;
    private routeSubscriber: any;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private registrationService: RegistrationServiceProxy,
        private playerService: PlayerServiceProxy,
        private route: ActivatedRoute) {
        super(injector);
    }

    ngOnInit() {
        this.getPlayerTeam();
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
            console.log(result);
        });
    }

    getRegistrations(mindfightId): void {
        this.registrationService.getMindfightRegistrations(mindfightId).subscribe((result) => {
            this.registrations = result;
            console.log(result);
        });
    }

    ChangeTeamConfirmationStatus(registrationId): void {
        
    }

    isTeamRegistered(teamId): boolean {
        return _.some(teamId, { userId: abp.session.userId });
    };

    getPlayerTeam(): void {
        this.playerService.getPlayerTeam(abp.session.userId).subscribe((result) => {
            this.playerTeamId = result;
        });
    };

    isMindfightCreator(): boolean {
        return abp.session.userId === this.mindfight.creatorId;
    }

    canEditMindfight(): boolean {
        return this.isMindfightCreator() || abp.auth.isGranted("ManageMindfights");
    }
}
