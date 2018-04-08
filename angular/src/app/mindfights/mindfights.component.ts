import { Component, OnInit, Injector } from '@angular/core';
import { MindfightServiceProxy, MindfightPublicDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    selector: 'app-mindfights',
    templateUrl: './mindfights.component.html',
    styleUrls: ['./mindfights.component.css']
})
export class MindfightsComponent extends AppComponentBase implements OnInit {
    mindfights: MindfightPublicDto[] = [];
    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        this.getMindfights();
    }

    getMindfights(): void {
        this.mindfightService.getUpcomingMindfights().subscribe((result) => {
            this.mindfights = result;
            console.log(result);
        });
    }
}
