import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeamsLeaderboardComponent } from './teams-leaderboard.component';

describe('TeamsLeaderboardComponent', () => {
  let component: TeamsLeaderboardComponent;
  let fixture: ComponentFixture<TeamsLeaderboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeamsLeaderboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeamsLeaderboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
