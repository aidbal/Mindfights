import { Component, OnInit } from '@angular/core';
import { appModuleAnimation } from 'shared/animations/routerTransition';

@Component({
  selector: 'app-administrate-mindfights',
  templateUrl: './administrate-mindfights.component.html',
    styleUrls: ['./administrate-mindfights.component.css'],
  animations: [appModuleAnimation()]
})

export class AdministrateMindfightsComponent implements OnInit {
    ngOnInit() {
    }
}
