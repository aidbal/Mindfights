import { Component, OnInit, Injector, Input } from '@angular/core';
import { MindfightServiceProxy, PlayerDto, PlayerServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';

@Component({
  selector: 'app-mindfights-list',
  templateUrl: './mindfights-list.component.html',
  styleUrls: ['./mindfights-list.component.css']
})
export class MindfightsListComponent extends AppComponentBase implements OnInit {
    mindfights: any;
    @Input() type: string;
    playerInfo: PlayerDto;

    constructor(
        injector: Injector,
        private mindfightService: MindfightServiceProxy,
        private playerService: PlayerServiceProxy
    ) {
        super(injector);
    }

    ngOnInit() {
        if (this.type === 'created') {
            this.getMyCreatedMindfights();
        } else if (this.type === 'upcoming') {
            this.getUpcomingMindfights();
        }
    }

    getPlayerInfo(): void {
        this.playerService.getPlayerInfo(abp.session.userId).subscribe((result) => {
            this.playerInfo = result;
        });
    };

    getUpcomingMindfights(): void {
        this.mindfightService.getUpcomingMindfights().subscribe((result) => {
            this.mindfights = result;
        });
    }
    
    getMyCreatedMindfights(): void {
        this.mindfightService.getMyCreatedMindfights().subscribe((result) => {
            this.mindfights = result;
        });
    }
}
