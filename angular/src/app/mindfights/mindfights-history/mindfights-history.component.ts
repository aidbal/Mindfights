import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from 'shared/app-component-base';
import { MindfightResultDto, ResultServiceProxy } from 'shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
    selector: 'app-mindfights-history',
    templateUrl: './mindfights-history.component.html',
    styleUrls: ['./mindfights-history.component.css']
})
export class MindfightsHistoryComponent extends AppComponentBase implements OnInit {
    @Input() teamId: number;
    @Input() userId: number;
    mindfightResults: MindfightResultDto[] = [];

    constructor(
        injector: Injector,
        private mindfightResultService: ResultServiceProxy,
        private activatedRoute: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.teamId) {
            this.getTeamMindfightResults(this.teamId);
        } else if (this.userId) {
            this.getUserMindfightResults(this.userId);
        }
    }

    getTeamMindfightResults(teamId) {
        this.mindfightResultService.getTeamResults(teamId).subscribe((result) => {
            this.mindfightResults = result;
            console.log(result);
        });
    }

    getUserMindfightResults(userId) {
        this.mindfightResultService.getUserResults(userId).subscribe((result) => {
            this.mindfightResults = result;
            console.log(result);
        });
    }
}
