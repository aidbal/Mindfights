import { Injectable, Output, EventEmitter } from '@angular/core';

@Injectable()
export class TeamStateService {
    @Output() nameChange: EventEmitter<boolean> = new EventEmitter();
    teamName;

    changeTeamName(teamName) {
        this.teamName = teamName;
        this.nameChange.emit(this.teamName);
    }
}
