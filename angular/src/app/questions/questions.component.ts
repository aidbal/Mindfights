import { Component, OnInit, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { Location } from '@angular/common';
import { MindfightDto, MindfightServiceProxy, TourDto, TourServiceProxy, QuestionDto, QuestionServiceProxy } from 'shared/service-proxies/service-proxies';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-questions',
    templateUrl: './questions.component.html',
    styleUrls: ['./questions.component.css'],
    animations: [appModuleAnimation()]
})
export class QuestionsComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    mindfight: MindfightDto;
    tourId: number;
    tour: TourDto;
    questions: QuestionDto[] = [];

    constructor(
        injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private location: Location,
        private mindfightService: MindfightServiceProxy,
        private tourService: TourServiceProxy,
        private questionService: QuestionServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId']; // (+) converts string 'id' to a number
            this.tourId = +params['tourId']; // (+) converts string 'id' to a number
        });
        this.getMindfight(this.mindfightId);
        this.getTour(this.tourId);
        this.getAllQuestions(this.tourId);
    }

    getAllQuestions(tourId): void {
        this.questionService.getAllTourQuestions(tourId).subscribe(
            (result) => {
                this.questions = result;
            }
        );
    }

    onOrderChange(orderChangeObject) {
        var currentOrderTourIndex = this.questions.findIndex(tour => tour.orderNumber === orderChangeObject.currentOrderNumber);
        var nextOrderTourIndex = this.questions.findIndex(tour => tour.orderNumber === orderChangeObject.newOrderNumber);
        if (currentOrderTourIndex >= 0 && nextOrderTourIndex >= 0) {
            this.questions[currentOrderTourIndex].orderNumber = orderChangeObject.newOrderNumber;
            this.questions[nextOrderTourIndex].orderNumber = orderChangeObject.currentOrderNumber;
            if (this.questions[nextOrderTourIndex].orderNumber === this.questions.length) {
                this.questions[nextOrderTourIndex].isLastQuestion = true;
                this.questions[currentOrderTourIndex].isLastQuestion = false;
            } else if (this.questions[currentOrderTourIndex].orderNumber === this.questions.length) {
                this.questions[currentOrderTourIndex].isLastQuestion = true;
                this.questions[nextOrderTourIndex].isLastQuestion = false;
            }
            this.questions.sort(this.compareByOrderNumber);
        }
    }

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
        });
    }

    getTour(tourId): void {
        this.tourService.getTour(tourId).subscribe((result) => {
            this.tour = result;
        });
    }

    goBack() {
        this.location.back();
    }

    compareByOrderNumber(a, b) {
        if (a.orderNumber < b.orderNumber)
            return -1;
        if (a.orderNumber > b.orderNumber)
            return 1;
        return 0;
    }
}
