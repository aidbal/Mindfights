import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TeamDto, TeamServiceProxy, PlayerDto, PlayerServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-create-team',
  templateUrl: './create-team.component.html',
  styleUrls: ['./create-team.component.css']
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
        console.log(this.team);
        this.teamService.createTeam(this.team).subscribe(() => {
            abp.message.success("Komanda sėkmingai sukurta!", "Atlikta");
            this.router.navigate(['../'], { relativeTo: this.route });
            this.saving = false;
        });
    }
}
