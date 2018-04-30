import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TourDto, TourServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-edit-tour',
    templateUrl: './edit-tour.component.html',
    styleUrls: ['./edit-tour.component.css'],
    animations: [appModuleAnimation()]
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
            this.notify.success("Turas sėkmingai atnaujintas!", "Atlikta");
            this.router.navigate(['../'], { relativeTo: this.route });
            this.saving = false;
        });
    }

    deleteTour(): void {
        abp.message.confirm(
            'Turas bus ištrintas.',
            'Are Jūs esate tikri?',
            isConfirmed => {
                if (isConfirmed) {
                    this.tourService.deleteTour(this.tourId).subscribe(
                        () => {
                            this.notify.success('Turas sėkmingai ištrintas!', 'Atlikta');
                        },
                        () => {
                            this.notify.error('Turo nepavyko ištrinti!', 'Klaida');
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
