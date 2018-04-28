import { Component, OnInit, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
    PlayerServiceProxy, TeamAnswerDto, QuestionDto,
    TeamAnswerServiceProxy, TeamServiceProxy, TeamDto,
    MindfightServiceProxy, MindfightDto
} from 'shared/service-proxies/service-proxies';
import { Location } from '@angular/common';
import { AppComponentBase } from 'shared/app-component-base';

@Component({
    selector: 'app-evaluate-team',
    templateUrl: './evaluate-team.component.html',
    styleUrls: ['./evaluate-team.component.css']
})
export class EvaluateTeamComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    teamId: number;
    team: TeamDto;
    mindfight: MindfightDto;
    teamAnswers: TeamAnswerDto[] = [];

    constructor(
        injector: Injector,
        private teamAnswerService: TeamAnswerServiceProxy,
        private teamService: TeamServiceProxy,
        private mindfightService: MindfightServiceProxy,
        private activatedRoute: ActivatedRoute,
        private location: Location,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.activatedRoute.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
            this.teamId = +params['teamId'];
            this.getTeamAnswers(this.mindfightId, this.teamId);
            this.getTeam(this.teamId);
            this.getMindfight(this.mindfightId);
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getTeamAnswers = function (mindfightId, teamId) {
        this.teamAnswerService.getAllTeamAnswers(this.mindfightId, this.teamId).subscribe((result) => {
            this.teamAnswers = result;
            console.log(result);
        });
    }

    getTeam = function (teamId) {
        this.teamService.getTeam(teamId).subscribe((result) => {
            this.team = result;
            console.log(result);
        });
    }

    getMindfight = function (mindfightId) {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            console.log(result);
        });
    }
}
