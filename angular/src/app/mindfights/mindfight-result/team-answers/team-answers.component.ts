import { Component, OnInit, Injector } from '@angular/core';
import {
    MindfightDto, TeamAnswerDto, TeamAnswerServiceProxy,
    MindfightServiceProxy, TeamDto, TeamServiceProxy
} from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from 'shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-team-answers',
    templateUrl: './team-answers.component.html',
    styleUrls: ['./team-answers.component.css'],
    animations: [appModuleAnimation()]
})
export class TeamAnswersComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    teamId: number;
    team: TeamDto;
    mindfight: MindfightDto;
    teamAnswers: TeamAnswerDto[] = [];

    constructor(
        injector: Injector,
        private teamAnswerService: TeamAnswerServiceProxy,
        private mindfightService: MindfightServiceProxy,
        private teamService: TeamServiceProxy,
        private activatedRoute: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.activatedRoute.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
            this.teamId = +params['teamId'];
            this.getTeamAnswers(this.mindfightId, this.teamId);
            this.getMindfight(this.mindfightId);
            this.getTeam(this.teamId);
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getTeam(teamId) {
        this.teamService.getTeam(teamId).subscribe((result) => {
            this.team = result;
        });
    }

    getTeamAnswers(mindfightId, teamId) {
        this.teamAnswerService.getAllTeamAnswers(this.mindfightId, this.teamId).subscribe((result) => {
            this.teamAnswers = result;
            console.log(result);
        });
    }

    getMindfight(mindfightId) {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            console.log(result);
        });
    }
}
