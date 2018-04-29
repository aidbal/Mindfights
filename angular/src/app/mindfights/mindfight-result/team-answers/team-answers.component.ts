import { Component, OnInit, Injector } from '@angular/core';
import { MindfightDto, TeamAnswerDto, TeamAnswerServiceProxy, MindfightServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from 'shared/app-component-base';

@Component({
    selector: 'app-team-answers',
    templateUrl: './team-answers.component.html',
    styleUrls: ['./team-answers.component.css']
})
export class TeamAnswersComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    teamId: number;
    mindfight: MindfightDto;
    teamAnswers: TeamAnswerDto[] = [];

    constructor(
        injector: Injector,
        private teamAnswerService: TeamAnswerServiceProxy,
        private mindfightService: MindfightServiceProxy,
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
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
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
