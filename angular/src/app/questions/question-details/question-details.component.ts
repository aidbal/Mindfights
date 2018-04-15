import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TourDto, TourServiceProxy, QuestionDto, QuestionServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-question-details',
  templateUrl: './question-details.component.html',
  styleUrls: ['./question-details.component.css']
})
export class QuestionDetailsComponent extends AppComponentBase implements OnInit {
    mindfightId: number;
    tourId: number;
    questionId: number;
    question: QuestionDto;
    private routeSubscriber: any;

    constructor(
        injector: Injector,
        private questionService: QuestionServiceProxy,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
            this.tourId = +params['tourId'];
            this.questionId = +params['questionId'];
        });
        this.getQuestion(this.questionId);
    }

    getQuestion(questionId): void {
        this.questionService.getQuestion(questionId).subscribe(
            (result) => {
                this.question = result;
                console.log(this.question);
            }
        );
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }
}
