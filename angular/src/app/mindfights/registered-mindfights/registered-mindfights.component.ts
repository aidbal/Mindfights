import { Component, OnInit, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';
import { PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';

@Component({
    selector: 'app-registered-mindfights',
    templateUrl: './registered-mindfights.component.html',
    styleUrls: ['./registered-mindfights.component.css'],
    animations: [appModuleAnimation()]
})
export class RegisteredMindfightsComponent extends AppComponentBase implements OnInit {
    playerInfo: PlayerDto;

    constructor(
        injector: Injector,
        private playerService: PlayerServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getPlayerInfo();
    }

    getPlayerInfo(): void {
        this.playerService.getPlayerInfo(abp.session.userId).subscribe((result) => {
            this.playerInfo = result;
        });
    };
}
