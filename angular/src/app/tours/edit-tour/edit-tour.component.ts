import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TourDto, TourServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-edit-tour',
    templateUrl: './edit-tour.component.html',
    styleUrls: ['./edit-tour.component.css']
})
export class EditTourComponent extends AppComponentBase implements OnInit {
    mindfightId: number;
    tourId: number;
    tour: TourDto;
    private routeSubscriber: any;
    saving = false;

    constructor(
        injector: Injector,
        private tourService: TourServiceProxy,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId']; // (+) converts string 'id' to a number
            this.tourId = +params['tourId']; // (+) converts string 'id' to a number
        });
        this.getTour(this.tourId);
    }

    getTour(tourId) {
        this.tourService.getTour(tourId).subscribe((result) => {
            this.tour = result;
            console.log(this.tour);
        }
        );
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    updateTour(): void {
        this.saving = true;
        this.tourService.updateTour(this.tour, this.tourId).subscribe(() => {
            abp.message.success("Turas sėkmingai atnaujintas!", "Atlikta");
            //this.router.navigate(['../'], { relativeTo: this.route });
            this.saving = false;
        });
    }

    deleteTour(): void {
        var that = this;
        abp.message.confirm(
            'Turas bus ištrintas.',
            'Are Jūs esate tikri?',
            function (isConfirmed) {
                if (isConfirmed) {
                    that.tourService.deleteTour(that.tourId).subscribe(
                        () => {
                            abp.message.success('Turas sėkmingai ištrintas!', 'Atlikta');
                        },
                        () => {
                            abp.message.error('Turo nepavyko ištrinti!', 'Klaida');
                        },
                        () => {
                            that.router.navigate(['../../'], { relativeTo: that.route });
                        }
                    );
                }
            }
        );
    }
}
