import { Component, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';

@Component({
    selector: 'app-mindfights',
    templateUrl: './mindfights.component.html',
    styleUrls: ['./mindfights.component.css'],
    animations: [appModuleAnimation()]
})
export class MindfightsComponent implements OnInit {
    ngOnInit() {
    }
}
