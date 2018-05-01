import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';
import { Location } from '@angular/common';

@Component({
    selector: 'app-player-details',
    templateUrl: './player-details.component.html',
    styleUrls: ['./player-details.component.css'],
    animations: [appModuleAnimation()]
})
export class PlayerDetailsComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    playerInfo: PlayerDto;
    playerId: number;

    constructor(
        injector: Injector,
        private playerService: PlayerServiceProxy,
        private route: ActivatedRoute,
        private location: Location,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.playerId = +params['playerId'];
            if (isNaN(this.playerId)) {
                this.getPlayerInfo(abp.session.userId);
            } else {
                this.getPlayerInfo(this.playerId);
            }
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getPlayerInfo(playerId): void {
        this.playerService.getPlayerInfo(playerId).subscribe((result) => {
            this.playerInfo = result;
        });
    };

    goBack() {
        this.location.back();
    }
}
