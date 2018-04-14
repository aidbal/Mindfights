import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TourDto, TourServiceProxy, QuestionDto, QuestionServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-tour-details',
  templateUrl: './tour-details.component.html',
  styleUrls: ['./tour-details.component.css']
})
export class TourDetailsComponent extends AppComponentBase implements OnInit {
    mindfightId: number;
    tourId: number;
    tour: TourDto;
    questions: QuestionDto[] = [];
    private routeSubscriber: any;

    constructor(
        injector: Injector,
        private tourService: TourServiceProxy,
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
        });

        this.tourService.getTour(this.tourId).subscribe(
            (result) => {
                this.tour = result;
                console.log(this.tour);
            }
        );

        this.questionService.getAllTourQuestions(this.tourId).subscribe(
            (result) => {
                this.questions = result;
                console.log(this.questions);
            }
        );
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }
}
