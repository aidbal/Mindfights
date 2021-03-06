import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { QuestionDto, QuestionServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-edit-question',
    templateUrl: './edit-question.component.html',
    styleUrls: ['./edit-question.component.css'],
    animations: [appModuleAnimation()]
})
export class EditQuestionComponent extends AppComponentBase implements OnInit {
    mindfightId: number;
    tourId: number;
    questionId: number;
    question: QuestionDto;
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
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId']; // (+) converts string 'id' to a number
            this.tourId = +params['tourId']; // (+) converts string 'id' to a number
            this.questionId = +params['questionId']; // (+) converts string 'id' to a number
        });
        this.getQuestion(this.questionId);
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getQuestion(questionId): void {
        this.questionService.getQuestion(questionId).subscribe(
            (result) => {
                this.question = result;
            }
        );
    }

    updateQuestion(): void {
        this.saving = true;
        this.questionService.updateQuestion(this.question, this.questionId).subscribe(() => {
            this.notify.success("Klausimas sėkmingai atnaujintas!", "Atlikta");
            this.router.navigate(['../'], { relativeTo: this.route });
            this.saving = false;
        });
    }

    deleteQuestion(): void {
        abp.message.confirm(
            'Klausimas bus ištrintas.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.questionService.deleteQuestion(this.questionId).subscribe(
                        () => {
                            this.notify.success('Klausimas sėkmingai ištrintas!', 'Atlikta');
                        },
                        () => {
                            this.notify.error('Klausimo nepavyko ištrinti!', 'Klaida');
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
