import { Component, OnInit, Injector, Input } from '@angular/core';
import { MindfightServiceProxy, MindfightPublicDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-mindfights-list',
  templateUrl: './mindfights-list.component.html',
  styleUrls: ['./mindfights-list.component.css']
})
export class MindfightsListComponent extends AppComponentBase implements OnInit {
    mindfights: any;
    @Input() type: string;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy) {
        super(injector);
    }

    ngOnInit() {
        if (this.type === 'evaluating') {
            this.getAllowedToEvaluateMindfights();
        } else if (this.type === 'created') {
            this.getMyCreatedMindfights();
        } else if (this.type === 'upcoming') {
            this.getUpcomingMindfights();
        }
    }

    getUpcomingMindfights(): void {
        this.mindfightService.getUpcomingMindfights().subscribe((result) => {
            this.mindfights = result;
        });
    }

    getAllowedToEvaluateMindfights(): void {
        this.mindfightService.getAllowedToEvaluateMindfights().subscribe((result) => {
            this.mindfights = result;
        });
    }

    getMyCreatedMindfights(): void {
        this.mindfightService.getMyCreatedMindfights().subscribe((result) => {
            this.mindfights = result;
        });
    }
}
