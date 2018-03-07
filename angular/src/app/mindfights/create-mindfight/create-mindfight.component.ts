import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-mindfight',
  templateUrl: './create-mindfight.component.html',
  styleUrls: ['./create-mindfight.component.css']
})
export class CreateMindfightComponent implements OnInit {

    constructor() { }

    event = [];

    selectedMindfightType;

  ngOnInit() {
  }

}
