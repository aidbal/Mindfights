import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TeamServiceProxy, TeamDto, PlayerDto, PlayerServiceProxy, TeamPlayerDto } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-team',
    templateUrl: './team.component.html',
    styleUrls: ['./team.component.css']
})
export class TeamComponent extends AppComponentBase implements OnInit {
    teamId: number;
    team: TeamDto;
    teamPlayers: TeamPlayerDto[] = [];
    teamLeader: TeamPlayerDto;
    activeTeamPlayers: TeamPlayerDto[] = [];
    passiveTeamPlayers: TeamPlayerDto[] = [];
    playerInfo: PlayerDto;

    constructor(
        injector: Injector,
        private teamService: TeamServiceProxy,
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
            if (this.playerInfo.teamId != null) {
                this.teamId = this.playerInfo.teamId;
                this.getPlayerTeam(this.playerInfo.teamId);
            }
        });
    };

    getPlayerTeam(teamId): void {
        this.teamService.getTeam(teamId).subscribe(
            (result) => {
                this.team = result;
                console.log(this.team);
                this.getAllTeamPlayers(this.playerInfo.teamId);
            }
        );
    }

    getAllTeamPlayers(teamId): void {
        this.teamService.getAllTeamPlayers(teamId).subscribe(
            (result) => {
                this.teamPlayers = result;
                this.getActivePassiveTeamPlayers(this.teamPlayers);
            }
        );
    }

    getActivePassiveTeamPlayers(teamPlayers): void {
        var that = this;
        teamPlayers.forEach(function (player) {
            if (player.id === that.team.leaderId) {
                that.teamLeader = player;
            } else if (player.isActiveInTeam) {
                that.activeTeamPlayers.push(player);
            } else {
                that.passiveTeamPlayers.push(player);
            }
        });
    }
}
