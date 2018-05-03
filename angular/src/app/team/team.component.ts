import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TeamServiceProxy, TeamDto, PlayerDto, PlayerServiceProxy, TeamPlayerDto } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';
import { Location } from '@angular/common';
import { TeamStateService } from 'app/services/team-state.service';

@Component({
    selector: 'app-team',
    templateUrl: './team.component.html',
    styleUrls: ['./team.component.css'],
    animations: [appModuleAnimation()]
})
export class TeamComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    teamId: number;
    team: TeamDto;
    teamPlayers: TeamPlayerDto[] = [];
    teamLeader: TeamPlayerDto;
    activeTeamPlayers: TeamPlayerDto[] = [];
    passiveTeamPlayers: TeamPlayerDto[] = [];
    playerInfo: PlayerDto;
    isTeamLeader = false;
    isMyTeam = false;

    constructor(
        injector: Injector,
        private teamService: TeamServiceProxy,
        private playerService: PlayerServiceProxy,
        private teamStateService: TeamStateService,
        private location: Location,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.teamId = +params['teamId'];

            if (isNaN(this.teamId)) {
                this.getPlayerInfo(abp.session.userId);
                this.isMyTeam = true;
            } else {
                this.getPlayerTeam(this.teamId);
            }
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    leaveCurrentTeam() {
        abp.message.confirm(
            'Paliksite šią komandą.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.teamService.leaveCurrentTeam().subscribe(
                        () => {
                            this.notify.success('Sėkmingai palikote komandą!', 'Atlikta');
                            this.team = null;
                            this.teamStateService.changeTeamName(null);
                        },
                        () => {
                            this.notify.error('Komandos palikti nepavyko!', 'Klaida');
                        }
                    );
                }
            }
        );
    }

    getPlayerInfo(playerId): void {
        this.playerService.getPlayerInfo(playerId).subscribe((result) => {
            this.playerInfo = result;
            if (this.playerInfo.teamId !== null && this.isMyTeam) {
                this.teamId = this.playerInfo.teamId;
                this.getPlayerTeam(this.playerInfo.teamId);
            }
            if (result.isTeamLeader) {
                this.isTeamLeader = true;
            }
            if (!this.isMyTeam) {
                if (this.teamId === this.playerInfo.teamId) {
                    this.isMyTeam = true;
                }
            }
        });
    };

    getPlayerTeam(teamId): void {
        this.teamService.getTeam(teamId).subscribe(
            (result) => {
                this.team = result;
                this.getAllTeamPlayers(this.teamId);
                if (!this.isMyTeam) {
                    this.getPlayerInfo(abp.session.userId);
                }
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
        teamPlayers.forEach(player => {
            if (player.id === this.team.leaderId) {
                if (this.teamLeader == null) {
                    this.teamLeader = player;
                }
            } else if (player.isActiveInTeam) {
                this.activeTeamPlayers.push(player);
            } else {
                this.passiveTeamPlayers.push(player);
            }
        });
    }

    goBack() {
        this.location.back();
    }
}
