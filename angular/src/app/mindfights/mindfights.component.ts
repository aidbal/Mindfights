import { Component, OnInit, Injector } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
    selector: 'app-mindfights',
    templateUrl: './mindfights.component.html',
    styleUrls: ['./mindfights.component.css'],
    animations: [appModuleAnimation()]
})
export class MindfightsComponent extends AppComponentBase implements OnInit {

    constructor(
        injector: Injector
    ) {
        super(injector);
    }

    ngOnInit() {
    }
}
