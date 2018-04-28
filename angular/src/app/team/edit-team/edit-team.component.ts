import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TeamDto, TeamServiceProxy, TeamPlayerDto } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-edit-team',
    templateUrl: './edit-team.component.html',
    styleUrls: ['./edit-team.component.css']
})
export class EditTeamComponent extends AppComponentBase implements OnInit {
    team: TeamDto;
    teamId: number;
    teamPlayers: TeamPlayerDto[] = [];
    currentUserId: number;
    saving = false;
    private routeSubscriber: any;

    constructor(
        injector: Injector,
        private teamService: TeamServiceProxy,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.teamId = +params['teamId'];
            this.getTeam(this.teamId);
            this.getAllTeamPlayers(this.teamId);
            this.currentUserId = abp.session.userId;
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    updateTeam(): void {
        this.saving = true;
        this.teamService.updateTeam(this.team, this.teamId).subscribe(() => {
            this.notify.success("Komanda sėkmingai atnaujinta!", "Atlikta");
            this.router.navigate(['../../'], { relativeTo: this.route });
            this.saving = false;
        });
    }

    getTeam(teamId): void {
        this.teamService.getTeam(teamId).subscribe((team) => {
            this.team = team;
        });
    }

    getAllTeamPlayers(teamId): void {
        this.teamService.getAllTeamPlayers(teamId).subscribe(
            (result) => {
                this.teamPlayers = result;
            }
        );
    }

    updateActiveStatus(player): void {
        this.teamService.updateUserActiveStatus(player.id, !player.isActiveInTeam).subscribe(() => {
            this.notify.success("Žaidėjo '" + player.userName + "' statusas sėkmingai pakeistas!");
            let playerIndex = this.teamPlayers.findIndex(i => i.id === player.id);
            if (playerIndex) {
                this.teamPlayers[playerIndex].isActiveInTeam = !player.isActiveInTeam;
            }
        },
            () => this.notify.error("Klaida keičiant žaidėjo statusą")
        );
    }

    deleteTeam(): void {
        abp.message.confirm(
            'Komanda bus ištrinta. Visi vartotojai bus išmesti iš komandos.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.teamService.deleteTeam(this.teamId).subscribe(
                        () => {
                            this.notify.success('Komanda sėkmingai ištrintas!', 'Atlikta');
                        },
                        () => {
                            this.notify.error('Komandos nepavyko ištrinti!', 'Klaida');
                        },
                        () => {
                            this.router.navigate(['../../'], { relativeTo: this.route });
                        }
                    );
                }
            }
        );
    }

    addNewPlayer(username): void {
        this.teamService.insertUser(this.teamId, username).subscribe(
            () => {
                this.notify.success("Žaidėjas '" + username + "' sėkmingai pridėtas į komandą!", "Atlikta");
                this.getAllTeamPlayers(this.teamId);
                username = '';
            }
        );
    }

    removePlayer(playerId, username): void {
        abp.message.confirm(
            'Žaidėjas bus pašalintas iš komandos.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.teamService.removeUser(this.teamId, playerId).subscribe(
                        () => {
                            this.notify.success("Žaidėjas '" + username + "' sėkmingai pašalintas!", "Atlikta");
                            let playerIndex = this.teamPlayers.findIndex(i => i.id === playerId);
                            if (playerIndex) {
                                this.teamPlayers.splice(playerIndex, 1);
                            }
                        }
                    );
                }
            }
        );
    }
}
