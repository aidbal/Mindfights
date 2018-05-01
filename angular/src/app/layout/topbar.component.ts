import { Component, Injector, ViewEncapsulation } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { MindfightStateService } from 'app/services/mindfight-state.service';

@Component({
    templateUrl: './topbar.component.html',
    selector: 'top-bar',
    encapsulation: ViewEncapsulation.None
})
export class TopBarComponent extends AppComponentBase {

    constructor(
        injector: Injector,
        private mindfightStateService: MindfightStateService
    ) {
        super(injector);
    }

    hasMindfightStarted() {
        return this.mindfightStateService.mindfightStartedState;
    }
}
