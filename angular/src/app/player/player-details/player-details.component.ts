import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-player-details',
    templateUrl: './player-details.component.html',
    styleUrls: ['./player-details.component.css'],
    animations: [appModuleAnimation()]
})
export class PlayerDetailsComponent extends AppComponentBase implements OnInit {
    playerInfo: PlayerDto;

    constructor(
        injector: Injector,
        private playerService: PlayerServiceProxy,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getPlayerInfo();
    }

    getPlayerInfo(): void {
        this.playerService.getPlayerInfo(abp.session.userId).subscribe((result) => {
            this.playerInfo = result;
            console.log(result);
        });
    };
}
