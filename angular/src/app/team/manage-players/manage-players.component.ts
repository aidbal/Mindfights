import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TeamPlayerDto, TeamServiceProxy } from 'shared/service-proxies/service-proxies';
import { TeamStateService } from 'app/services/team-state.service';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-manage-players',
    templateUrl: './manage-players.component.html',
    styleUrls: ['./manage-players.component.css'],
    animations: [appModuleAnimation()]
})
export class ManagePlayersComponent extends AppComponentBase implements OnInit {
    teamId: number;
    teamPlayers: TeamPlayerDto[] = [];
    teamPlayersForNewLeader: TeamPlayerDto[] = [];
    private routeSubscriber: any;
    saving = false;
    newPlayerUsername: string;

    constructor(
        injector: Injector,
        private teamService: TeamServiceProxy,
        private teamStateService: TeamStateService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.teamId = +params['teamId'];
            this.getAllTeamPlayers(this.teamId);
        });
    }

    getAllTeamPlayers(teamId): void {
        this.teamService.getAllTeamPlayers(teamId).subscribe(
            (result) => {
                this.teamPlayers = result;
                this.teamPlayersForNewLeader = result.filter(player => !player.isTeamLeader);
            }
        );
    }

    updateActiveStatus(player): void {
        this.teamService.updateUserActiveStatus(player.id, !player.isActiveInTeam).subscribe(() => {
            this.notify.success("Žaidėjo '" + player.userName + "' statusas sėkmingai pakeistas!");
            let playerIndex = this.teamPlayers.findIndex(i => i.id === player.id);
            if (playerIndex >= 0) {
                this.teamPlayers[playerIndex].isActiveInTeam = !player.isActiveInTeam;
            }
        },
            () => this.notify.error("Klaida keičiant žaidėjo statusą")
        );
    }

    addNewPlayer(): void {
        this.saving = true;
        this.teamService.insertUser(this.teamId, this.newPlayerUsername).subscribe(
            () => {
                this.notify.success("Žaidėjas '" + this.newPlayerUsername + "' sėkmingai pridėtas į komandą!",
                    "Atlikta");
                this.getAllTeamPlayers(this.teamId);
                this.newPlayerUsername = null;
            },
            () => {},
            () => this.saving = false
        );
    }

    changeTeamLeader(user: TeamPlayerDto): void {
        this.saving = true;
        this.teamService.changeTeamLeader(this.teamId, user.id).subscribe(
            () => {
                this.notify.success("Žaidėjas '" + user.userName + "' sėkmingai tapo komandos kapitonu!", "Atlikta");
                this.router.navigate(['../../'], { relativeTo: this.route });
            },
            () => {},
            () => this.saving = false
        );
    }

    removePlayer(playerId, username): void {
        this.saving = true;
        this.teamService.removeUser(this.teamId, playerId).subscribe(
            () => {
                this.notify.success("Žaidėjas '" + username + "' sėkmingai pašalintas!", "Atlikta");
                let playerIndex = this.teamPlayers.findIndex(i => i.id === playerId);
                if (playerIndex >= 0) {
                    this.teamPlayers.splice(playerIndex, 1);
                }
                let playerIndexForLeader = this.teamPlayersForNewLeader.findIndex(i => i.id === playerId);
                if (playerIndexForLeader >= 0) {
                    this.teamPlayersForNewLeader.splice(playerIndexForLeader, 1);
                }
            },
            () => this.notify.error("Klaida šalinant žaidėją"),
            () => this.saving = false
        );
    }
}
