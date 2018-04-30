import { Component, OnInit, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MindfightServiceProxy, MindfightDto } from 'shared/service-proxies/service-proxies';
import { Location } from '@angular/common';
import { AppComponentBase } from 'shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
    selector: 'app-evaluate-mindfights.',
    templateUrl: './evaluate-mindfights.component.html',
    styleUrls: ['./evaluate-mindfights.component.css'],
    animations: [appModuleAnimation()]
})
export class EvaluateMindfightsComponent extends AppComponentBase implements OnInit {
    mindfights: MindfightDto[] = [];

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private activatedRoute: ActivatedRoute,
        private location: Location,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        this.getAllowedToEvaluateMindfights();
    }

    getAllowedToEvaluateMindfights(): void {
        this.mindfightService.getAllowedToEvaluateMindfights().subscribe((result) => {
            this.mindfights = result;
            console.log(result);
        });
    }

}
