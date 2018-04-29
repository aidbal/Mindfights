import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamAnswersComponent } from './team-answers.component';

describe('TeamAnswersComponent', () => {
  let component: TeamAnswersComponent;
  let fixture: ComponentFixture<TeamAnswersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeamAnswersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamAnswersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
