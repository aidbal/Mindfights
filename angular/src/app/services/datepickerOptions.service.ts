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
        endDate: moment().add(1, 'days').endOf('day')
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
        "opens": "right",
        "minDate": moment().add(1, 'days').format(this.dateFormat),
        "startDate": this.initialDate.startDate.format(this.dateTimeFormat),
        "endDate": this.initialDate.endDate.format(this.dateTimeFormat)
    };

    constructor() {
        Object.assign(this.singleDatepickerOptions, this.datepickerOptions, this.singleDatePickerOption);
        Object.assign(this.rangeDatepickerOptions, this.datepickerOptions);
    }

    getSingleDatepickerOptions() {
        return this.singleDatepickerOptions;
    }

    getRangeDatepickerOptions() {
        return this.rangeDatepickerOptions;
    }

    getInitialDate() {
        return this.initialDate;
    }
}
