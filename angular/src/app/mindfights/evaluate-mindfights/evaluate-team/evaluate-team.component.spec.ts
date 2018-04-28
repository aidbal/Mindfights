import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluateTeamComponent } from './evaluate-team.component';

describe('EvaluateTeamComponent', () => {
  let component: EvaluateTeamComponent;
  let fixture: ComponentFixture<EvaluateTeamComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EvaluateTeamComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EvaluateTeamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
