import { Component, OnInit, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { appModuleAnimation } from 'shared/animations/routerTransition';
import { ActivatedRoute, Router } from '@angular/router';
import { PlayerDto, PlayerServiceProxy, City, AccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { DatepickerOptionsService } from 'app/services/datepickerOptions.service';
import * as moment from 'moment';

@Component({
    selector: 'app-edit-player',
    templateUrl: './edit-player.component.html',
    styleUrls: ['./edit-player.component.css'],
    animations: [appModuleAnimation()]
})
export class EditPlayerComponent extends AppComponentBase implements OnInit {
    startDate = moment("1990 01 01", "YYYY MM DD");
    minDate = moment("1900 01 01", "YYYY MM DD");
    maxDate = moment();
    private routeSubscriber: any;
    playerInfo: PlayerDto;
    playerId: number;
    registrationCities: City[] = [];
    loaded = false;
    saving = false

    constructor(
        injector: Injector,
        private playerService: PlayerServiceProxy,
        private accountService: AccountServiceProxy,
        private datepickerOptionsService: DatepickerOptionsService,
        private route: ActivatedRoute,
        private router: Router
    ) {
        super(injector);
    }

    ngOnInit() {

        this.getRegistrationCities();
        this.routeSubscriber = this.route.params.subscribe(params => {
            this.playerId = +params['playerId'];
            if (!(this.playerId === abp.session.userId || abp.auth.isGranted("Pages.Users"))) {
                this.message.warn("Neturite teisių redaguoti vartotojo duomenų!");
                this.router.navigate(['/app/player']);
            }
            this.getPlayerInfo(this.playerId);
        });
    }

    ngOnDestroy() {
        this.routeSubscriber.unsubscribe();
    }

    getRegistrationCities() {
        this.accountService.getRegistrationCities().subscribe((cities) => {
            this.registrationCities = cities;
        });
    }

    getPlayerInfo(playerId) {
        this.playerService.getPlayerInfo(playerId).subscribe((result) => {
            this.playerInfo = result;
            this.startDate = moment(result.birthdate, "YYYY MM DD");
            this.loaded = true;
        });
    };

    updatePlayerInfo() {
        this.saving = true;
        this.playerInfo.birthdate = moment.utc(this.playerInfo.birthdate.format('YYYY-MM-DD HH:mm'),
            'YYYY-MM-DD HH:mm');
        this.playerService.updatePlayerInfo(this.playerInfo, this.playerId).subscribe(() => {
            this.notify.success("Sėkmingai atnaujinti vartotojo duomenys", "Atlikta!");
            this.router.navigate(['/app/player/' + this.playerId]);
            this.saving = false;
        });
    }
}
