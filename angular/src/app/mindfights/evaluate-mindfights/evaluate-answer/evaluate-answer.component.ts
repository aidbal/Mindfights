import { Component, OnInit, Injector, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TeamAnswerServiceProxy, TeamAnswerDto, QuestionDto, QuestionServiceProxy } from 'shared/service-proxies/service-proxies';
import { AppComponentBase } from 'shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-evaluate-answer',
    templateUrl: './evaluate-answer.component.html',
    styleUrls: ['./evaluate-answer.component.css'],
    animations: [appModuleAnimation()]
})
export class EvaluateAnswerComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    teamId: number;
    questionId: number;
    teamAnswer: TeamAnswerDto;
    question: QuestionDto;

    constructor(
        injector: Injector,
        private teamAnswerService: TeamAnswerServiceProxy,
        private questionService: QuestionServiceProxy,
        private activatedRoute: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.activatedRoute.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
            this.teamId = +params['teamId'];
            this.questionId = +params['questionId'];
            this.getQuestion(this.questionId);
            this.getTeamAnswer(this.questionId, this.teamId);
        });
    }

    getQuestion (questionId) {
        this.questionService.getQuestion(questionId).subscribe((result) => {
            this.question = result;
        });
    }

    getTeamAnswer (questionId, teamId) {
        this.teamAnswerService.getTeamAnswer(questionId, teamId).subscribe((result) => {
            this.teamAnswer = result;
        });
    }
    
    evaluateAnswer() {
        this.teamAnswer.evaluatorComment = (this.teamAnswer.evaluatorComment && this.teamAnswer.evaluatorComment.length > 0)
            ? this.teamAnswer.evaluatorComment
            : '';
        this.teamAnswerService.updateIsEvaluated(
            this.questionId,
            this.teamId,
            this.teamAnswer.evaluatorComment,
            this.teamAnswer.earnedPoints,
            true
        ).subscribe(() => {
            this.notify.success("Atsakymas sėkmingai įvertintas!", "Atlikta");
            this.router.navigate(['../../'], { relativeTo: this.activatedRoute });
        });
    }
}
