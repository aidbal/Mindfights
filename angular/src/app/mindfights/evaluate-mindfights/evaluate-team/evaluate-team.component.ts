import { Component, OnInit, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
    PlayerServiceProxy, TeamAnswerDto, QuestionDto,
    TeamAnswerServiceProxy, TeamServiceProxy, TeamDto,
    MindfightServiceProxy, MindfightDto, ResultServiceProxy, MindfightResultDto
} from 'shared/service-proxies/service-proxies';
import { Location } from '@angular/common';
import { AppComponentBase } from 'shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-evaluate-team',
    templateUrl: './evaluate-team.component.html',
    styleUrls: ['./evaluate-team.component.css'],
    animations: [appModuleAnimation()]
})
export class EvaluateTeamComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    teamId: number;
    team: TeamDto;
    mindfight: MindfightDto;
    evaluatedTeamAnswers: number = 0;
    teamAnswers: TeamAnswerDto[] = [];
    mindfightResult: MindfightResultDto;

    constructor(
        injector: Injector,
        private teamAnswerService: TeamAnswerServiceProxy,
        private teamService: TeamServiceProxy,
        private mindfightService: MindfightServiceProxy,
        private resultService: ResultServiceProxy,
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
            this.getMindfightResult(this.mindfightId, this.teamId);
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getMindfightResult(mindfightId, teamId) {
        this.resultService.getMindfightTeamResult(mindfightId, teamId).subscribe((result) => {
            this.mindfightResult = result;
        });
    }

    getTeamAnswers(mindfightId, teamId) {
        this.teamAnswerService.getAllTeamAnswers(this.mindfightId, this.teamId).subscribe((result) => {
            this.teamAnswers = result;
            result.forEach((answer) => {
                if (answer.isEvaluated) {
                    this.evaluatedTeamAnswers += 1;
                }
            });
            console.log(result);
        });
    }

    getTeam(teamId) {
        this.teamService.getTeam(teamId).subscribe((result) => {
            this.team = result;
            console.log(result);
        });
    }

    getMindfight (mindfightId) {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            console.log(result);
        });
    }

    completeEvaluate() {
        this.resultService.updateResult(this.mindfightId, this.teamId).subscribe(() => {
            this.notify.success("Sėkmingai baigta vertinti komandą.", "Atlikta!");
        });
    }
}
