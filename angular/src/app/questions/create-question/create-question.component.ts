import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { QuestionDto, QuestionServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-create-question',
  templateUrl: './create-question.component.html',
  styleUrls: ['./create-question.component.css']
})
export class CreateQuestionComponent extends AppComponentBase implements OnInit {
    mindfightId: number;
    tourId: number;
    question: QuestionDto = null;
    private routeSubscriber: any;
    saving = false;

    constructor(
        injector: Injector,
        private questionService: QuestionServiceProxy,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.question = new QuestionDto();
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId']; // (+) converts string 'id' to a number
            this.tourId = +params['tourId']; // (+) converts string 'id' to a number
        });
        this.question.timeToAnswerInSeconds = 60;
        this.question.points = 10;
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    createQuestion(): void {
        this.saving = true;
        this.questionService.createQuestion(this.question, this.tourId).subscribe(() => {
            this.notify.success("Klausimas sėkmingai sukurtas!", "Atlikta");
            this.router.navigate(['../'], { relativeTo: this.route });
            this.saving = false;
        });
    }
}
