import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TeamDto, TeamServiceProxy, PlayerDto, PlayerServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';


@Component({
  selector: 'app-create-team',
  templateUrl: './create-team.component.html',
    styleUrls: ['./create-team.component.css'],
  animations: [appModuleAnimation()]
})
export class CreateTeamComponent extends AppComponentBase implements OnInit {
    team: TeamDto;
    saving = false;

    constructor(
        injector: Injector,
        private teamService: TeamServiceProxy,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.team = new TeamDto();
    }
    
    createTeam(): void {
        this.saving = true;
        this.teamService.createTeam(this.team).subscribe(() => {
            this.notify.success("Komanda sėkmingai sukurta!", "Atlikta");
            this.router.navigate(['../'], { relativeTo: this.route });
            this.saving = false;
        });
    }
}
