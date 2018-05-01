import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MindfightDto, MindfightServiceProxy } from 'shared/service-proxies/service-proxies';

@Component({
    selector: 'app-mindfight-status-label',
    templateUrl: './mindfight-status-label.component.html',
    styleUrls: ['./mindfight-status-label.component.css']
})
export class MindfightStatusLabelComponent extends AppComponentBase implements OnInit {
    @Input() mindfight: MindfightDto;
    @Input() canEdit;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
    ) {
        super(injector);
    }

    ngOnInit() {
    }

    updateActiveStatus(currentStatus) {
        this.mindfightService.updateActiveStatus(this.mindfight.id, !currentStatus).subscribe(() => {
            if (currentStatus) {
                this.notify.success("Protmūšis '" + this.mindfight.title + "' sėkmingai deaktyvuotas!");
            } else {
                this.notify.success("Protmūšis '" + this.mindfight.title + "' sėkmingai aktyvuotas!")
            }
            this.mindfight.isActive = !currentStatus;
        });
    }
}
