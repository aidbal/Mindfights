import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TeamDto, TeamServiceProxy, TeamPlayerDto } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';
import { TeamStateService } from 'app/services/team-state.service';

@Component({
    selector: 'app-edit-team',
    templateUrl: './edit-team.component.html',
    styleUrls: ['./edit-team.component.css'],
    animations: [appModuleAnimation()]
})
export class EditTeamComponent extends AppComponentBase implements OnInit {
    team: TeamDto;
    teamId: number;
    teamPlayers: TeamPlayerDto[] = [];
    teamPlayersForNewLeader: TeamPlayerDto[] = [];
    currentUserId: number;
    saving = false;
    private routeSubscriber: any;

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
            this.getTeam(this.teamId);
            this.currentUserId = abp.session.userId;
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    updateTeam(): void {
        this.saving = true;
        this.teamService.updateTeam(this.team, this.teamId).subscribe(() => {
            this.teamStateService.changeTeamName(this.team.name);
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

    deleteTeam(): void {
        abp.message.confirm(
            'Komanda bus ištrinta. Visi vartotojai bus išmesti iš komandos.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.teamService.deleteTeam(this.teamId).subscribe(
                        () => {
                            this.notify.success('Komanda sėkmingai ištrintas!', 'Atlikta');
                            this.teamStateService.changeTeamName(null);
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
}
