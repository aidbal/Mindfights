import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { TourDto, TourServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-create-tour',
    templateUrl: './create-tour.component.html',
    styleUrls: ['./create-tour.component.css']
})
export class CreateTourComponent extends AppComponentBase implements OnInit {
    mindfightId: number;
    tour: TourDto = null;
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
        this.tour = new TourDto();
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.mindfightId = +params['mindfightId']; // (+) converts string 'id' to a number
        });
        this.tour.timeToEnterAnswersInSeconds = 300;
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    createTour(): void {
        this.saving = true;
        this.tourService.createTour(this.tour, this.mindfightId).subscribe(() => {
            this.notify.success("Turas sėkmingai sukurtas!", "Atlikta");
            this.router.navigate(['../'], { relativeTo: this.route });
            this.saving = false;
        });
    }
}
