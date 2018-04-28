import { Component, OnInit, Injector } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PlayerServiceProxy, MindfightServiceProxy } from 'shared/service-proxies/service-proxies';
import { Location } from '@angular/common';
import { AppComponentBase } from 'shared/app-component-base';

@Component({
    selector: 'app-mindfight-result',
    templateUrl: './mindfight-result.component.html',
    styleUrls: ['./mindfight-result.component.css']
})
export class MindfightResultComponent extends AppComponentBase implements OnInit {
    private routeSubscriber: any;

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
        //this.getPlayerInfo();
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }
}
