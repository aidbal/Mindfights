import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';
import * as $ from 'jquery';
import 'datatables.net';
import 'datatables.net-bs4';
import { PlayerServiceProxy, PlayerDto } from 'shared/service-proxies/service-proxies';

@Component({
    selector: 'app-players-leaderboard',
    templateUrl: './players-leaderboard.component.html',
    styleUrls: ['./players-leaderboard.component.css'],
    animations: [appModuleAnimation()]
})
export class PlayersLeaderboardComponent extends AppComponentBase implements OnInit {
    dataTable: any;
    players: PlayerDto[] = [];

    constructor(
        injector: Injector,
        private playerService: PlayerServiceProxy,
        private chRef: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getAllPlayers();
    }

    getAllPlayers() {
        this.playerService.getAllPlayers().subscribe(
            (result) => {
                this.players = result;
                this.chRef.detectChanges();
                const table: any = $('#playersTable');
                this.dataTable = table.DataTable({
                    "language": {
                        "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Lithuanian.json"
                    },
                    "order": [[2, "desc"], [0, "asc"]]
                });
            });
    }
}
