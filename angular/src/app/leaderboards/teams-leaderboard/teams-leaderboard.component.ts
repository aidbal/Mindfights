import { Component, OnInit, Injector, ChangeDetectorRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';
import * as $ from 'jquery';
import 'datatables.net';
import 'datatables.net-bs4';
import { TeamServiceProxy, TeamDto } from 'shared/service-proxies/service-proxies';

@Component({
    selector: 'app-teams-leaderboard',
    templateUrl: './teams-leaderboard.component.html',
    styleUrls: ['./teams-leaderboard.component.css'],
    animations: [appModuleAnimation()]
})
export class TeamsLeaderboardComponent extends AppComponentBase implements OnInit {
    dataTable: any;
    teams: TeamDto[] = [];

    constructor(
        injector: Injector,
        private teamService: TeamServiceProxy,
        private chRef: ChangeDetectorRef
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getAllTeams();
    }

    getAllTeams() {
        this.teamService.getAllTeams().subscribe(
            (result) => {
                this.teams = result;
                this.chRef.detectChanges();
                const table: any = $('#teamsTable');
                this.dataTable = table.DataTable({
                    "language": {
                        "url": "//cdn.datatables.net/plug-ins/1.10.16/i18n/Lithuanian.json"
                    },
                    "order": [[2,"desc"], [0, "asc"]]
                });
            });
    }
}
