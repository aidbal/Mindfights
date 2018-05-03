import { Component, OnInit, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { AppAuthService } from '@shared/auth/app-auth.service';
import { PlayerDto, PlayerServiceProxy } from 'shared/service-proxies/service-proxies';
import { TeamStateService } from 'app/services/team-state.service';

@Component({
    templateUrl: './sidebar-user-area.component.html',
    selector: 'sidebar-user-area',
    styleUrls: ['./sidebar-user-area.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class SideBarUserAreaComponent extends AppComponentBase implements OnInit {
    playerInfo: PlayerDto;
    shownLoginName: string = "";

    constructor(
        injector: Injector,
        private _authService: AppAuthService,
        private playerService: PlayerServiceProxy,
        private teamStateService: TeamStateService
    ) {
        super(injector);
    }

    ngOnInit() {
        this.shownLoginName = this.appSession.getShownLoginName();
        this.getPlayerInfo(abp.session.userId);
        this.teamStateService.nameChange.subscribe(name => {
            this.playerInfo.teamName = name;
        });
    }

    getPlayerInfo(playerId): void {
        this.playerService.getPlayerInfo(playerId).subscribe((result) => {
            this.playerInfo = result;
        });
    };

    logout(): void {
        this._authService.logout();
    }
}
