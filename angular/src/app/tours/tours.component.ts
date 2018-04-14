import { Component, OnInit, Injector } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AppComponentBase } from '@shared/app-component-base';
import { Location } from '@angular/common';
import { MindfightDto, MindfightServiceProxy, TourDto, TourServiceProxy } from 'shared/service-proxies/service-proxies';

@Component({
  selector: 'app-tours',
  templateUrl: './tours.component.html',
  styleUrls: ['./tours.component.css']
})
export class ToursComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    mindfight: MindfightDto;
    tours: TourDto[] = [];

    constructor(
        injector: Injector,
        private route: ActivatedRoute,
        private router: Router,
        private location: Location,
        private mindfightService: MindfightServiceProxy,
        private tourService: TourServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId']; // (+) converts string 'id' to a number
        });
        this.getMindfight(this.mindfightId);
        this.getAllTours(this.mindfightId);
    }

    getAllTours(mindfightId): void {
        this.tourService.getAllMindfightTours(mindfightId).subscribe(
            (result) => {
                this.tours = result;
            }
        );
    }

    onOrderChange(orderChangeObject) {
        var currentOrderTourIndex = this.tours.findIndex(tour => tour.orderNumber === orderChangeObject.currentOrderNumber);
        var nextOrderTourIndex = this.tours.findIndex(tour => tour.orderNumber === orderChangeObject.newOrderNumber);
        if (currentOrderTourIndex >= 0 && nextOrderTourIndex >= 0) {
            this.tours[currentOrderTourIndex].orderNumber = orderChangeObject.newOrderNumber;
            this.tours[nextOrderTourIndex].orderNumber = orderChangeObject.currentOrderNumber;
            if (this.tours[nextOrderTourIndex].orderNumber === this.tours.length) {
                this.tours[nextOrderTourIndex].isLastTour = true;
                this.tours[currentOrderTourIndex].isLastTour = false;
            } else if (this.tours[currentOrderTourIndex].orderNumber === this.tours.length) {
                this.tours[currentOrderTourIndex].isLastTour = true;
                this.tours[nextOrderTourIndex].isLastTour = false;
            }
            this.tours.sort(this.compareByOrderNumber);
        }
    }

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
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
