import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MindfightDto, MindfightServiceProxy } from 'shared/service-proxies/service-proxies';
import * as moment from 'moment';

@Component({
    selector: 'app-mindfight-status-label',
    templateUrl: './mindfight-status-label.component.html',
    styleUrls: ['./mindfight-status-label.component.css']
})
export class MindfightStatusLabelComponent extends AppComponentBase implements OnInit {
    @Input() mindfight: MindfightDto;
    @Input() canEdit;
    mindfightStarted = false;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        this.checkMindfightStatus();
    }

    updateActiveStatus(currentStatus) {
        this.mindfightService.updateActiveStatus(this.mindfight.id, !currentStatus).subscribe(() => {
            if (currentStatus) {
                this.notify.success("Protmūšis '" + this.mindfight.title + "' sėkmingai deaktyvuotas!");
            } else {
                this.notify.success("Protmūšis '" + this.mindfight.title + "' sėkmingai aktyvuotas!")
            }
            this.mindfight.isActive = !currentStatus;
            this.checkMindfightStatus();
        });
    }

    checkMindfightStatus() {
        if (moment(this.mindfight.startTime).add(this.mindfight.prepareTime, 'minutes').diff(moment()) <= 0) {
            this.mindfightStarted = true;
        }
    }
}
