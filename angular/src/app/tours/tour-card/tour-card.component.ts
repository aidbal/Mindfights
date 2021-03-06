import { Component, OnInit, Injector, Input, Output, EventEmitter } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TourDto, TourServiceProxy } from 'shared/service-proxies/service-proxies';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-tour-card',
    templateUrl: './tour-card.component.html',
    styleUrls: ['./tour-card.component.css'],
    animations: [appModuleAnimation()]
})
export class TourCardComponent extends AppComponentBase implements OnInit {
    @Output() notifyOrderChange: EventEmitter<any> = new EventEmitter();
    @Input() tour: TourDto;
    @Input() mindfightId: number;
    saving = false;
    orderChangeObject = {
        tourId: null,
        currentOrderNumber: null,
        newOrderNumber: null
    };

    constructor(
        injector: Injector,
        private tourService: TourServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.orderChangeObject.tourId = this.tour.id;
        this.orderChangeObject.currentOrderNumber = this.tour.orderNumber;
    }

    moveTourOrderDown(): void {
        this.orderChangeObject.currentOrderNumber = this.tour.orderNumber;
        var newOrderNumber = this.tour.orderNumber + 1;
        this.saving = true;
        this.tourService.updateOrderNumber(this.tour.id, newOrderNumber).subscribe(
            () => {
                this.orderChangeObject.newOrderNumber = newOrderNumber;
                this.notify.success("Eilės numeris sėkmingai atnaujintas!", "Atlikta");
                this.notifyOrderChange.emit(this.orderChangeObject);
                this.saving = false;
            });
    }

    moveTourOrderUp(): void {
        if (this.tour.orderNumber - 1 != 0) {
            this.orderChangeObject.currentOrderNumber = this.tour.orderNumber;
            var newOrderNumber = this.tour.orderNumber - 1;
            this.saving = true;
            this.tourService.updateOrderNumber(this.tour.id, newOrderNumber).subscribe(
                () => {
                    this.orderChangeObject.newOrderNumber = newOrderNumber;
                    this.notify.success("Eilės numeris sėkmingai atnaujintas!", "Atlikta");
                    this.notifyOrderChange.emit(this.orderChangeObject);
                    this.saving = false;
                });
        }
    }
}
