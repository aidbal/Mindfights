import { Component, OnInit, Injector, Input } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MindfightDto } from 'shared/service-proxies/service-proxies';

@Component({
  selector: 'app-mindfight-card',
  templateUrl: './mindfight-card.component.html',
  styleUrls: ['./mindfight-card.component.css']
})
export class MindfightCardComponent extends AppComponentBase implements OnInit {
    @Input() mindfight: MindfightDto;

    constructor(injector: Injector) {
        super(injector);
    }

    ngOnInit() {
    }

    onRegisterChange(registrationChangeObject): void {
        if (registrationChangeObject.createEvent) {
            this.mindfight.registeredTeamsCount += 1;
        } else {
            this.mindfight.registeredTeamsCount -= 1;
        }
    }
}
