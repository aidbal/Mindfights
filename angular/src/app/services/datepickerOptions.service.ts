import { Injectable } from '@angular/core';
import * as moment from 'moment';

@Injectable()
export class DatepickerOptionsService {
    dateFormat = "YYYY-MM-DD";
    dateTimeFormat = this.dateFormat + " HH:mm";
    singleDatepickerOptions = {};
    rangeDatepickerOptions = {};
    initialDate = {
        startDate: moment().add(1, 'days'),
        endDate: moment().add(2, 'days').endOf('day')
    };

    private singleDatePickerOption = {
        "singleDatePicker": true
    };

    private datepickerOptions = {
        "timePicker": true,
        "timePicker24Hour": true,
        "locale": {
            "format": this.dateTimeFormat,
            "separator": " - ",
            "applyLabel": "Patvirtinti",
            "cancelLabel": "Atšaukti",
            "fromLabel": "Nuo",
            "toLabel": "Iki",
            "customRangeLabel": "Nurodyti laikotarpį",
            "weekLabel": "Sav",
            "daysOfWeek": [
                "S",
                "P",
                "A",
                "T",
                "K",
                "Pn",
                "Š"
            ],
            "monthNames": [
                "Sausis",
                "Vasaris",
                "Kovas",
                "Balandis",
                "Gegužė",
                "Birželis",
                "Liepa",
                "Rugpjūtis",
                "Rugsėjis",
                "Spalis",
                "Lapkritis",
                "Gruodis"
            ],
            "firstDay": 1
        },
        "alwaysShowCalendars": true,
        "opens": "right"
    };

    constructor() {
        Object.assign(this.singleDatepickerOptions, this.datepickerOptions, this.singleDatePickerOption);
    }

    getSingleDatepickerOptions() {
        return this.singleDatepickerOptions;
    }

    getInitialDate() {
        return this.initialDate;
    }

    getDatetimeStringFromMoment(datetime) {
        return datetime.format(this.dateTimeFormat);
    }
}
