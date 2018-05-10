import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayersLeaderboardComponent } from './players-leaderboard.component';

describe('PlayersLeaderboardComponent', () => {
  let component: PlayersLeaderboardComponent;
  let fixture: ComponentFixture<PlayersLeaderboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlayersLeaderboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlayersLeaderboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
