import { Component, OnInit, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PlayerServiceProxy, MindfightServiceProxy, MindfightDto, MindfightResultDto, ResultServiceProxy } from 'shared/service-proxies/service-proxies';
import { Location } from '@angular/common';
import { AppComponentBase } from 'shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-evaluate-mindfight-details',
    templateUrl: './evaluate-mindfight-details.component.html',
    styleUrls: ['./evaluate-mindfight-details.component.css'],
    animations: [appModuleAnimation()]
})
export class EvaluateMindfightDetailsComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;
    mindfightId: number;
    mindfight: MindfightDto;
    mindfightResults: MindfightResultDto[] = [];
    evaluatedCount: number = 0;
    overallCount: number = 0;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private mindfightResultService: ResultServiceProxy,
        private location: Location,
        private activatedRoute: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.routeSubscriber = this.activatedRoute.params.subscribe(params => {
            this.mindfightId = +params['mindfightId'];
            this.getMindfight(this.mindfightId);
            this.getMindfightResults(this.mindfightId);
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getMindfight(mindfightId): void {
        this.mindfightService.getMindfight(mindfightId).subscribe((result) => {
            this.mindfight = result;
            console.log(result);
        });
    }

    getMindfightResults(mindfightId): void {
        this.mindfightResultService.getMindfightResults(mindfightId).subscribe((result) => {
            this.mindfightResults = result;
            this.mindfightResults.forEach((result) => {
                if (result.isEvaluated) {
                    this.evaluatedCount += 1;
                }
                this.overallCount += 1;
            });
            console.log(result);
        });
    }

    completeMindfight(): void {
        this.mindfightService.updateFinishedStatus(this.mindfightId, true).subscribe(() => {
            this.notify.success("Protmūšis sėkmingai užbaigtas", "Atlikta!");
            this.mindfight.isFinished = true;
        });
    }

    goBack() {
        this.location.back();
    }
}
