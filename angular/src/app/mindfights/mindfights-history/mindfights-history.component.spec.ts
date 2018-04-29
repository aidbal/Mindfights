import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MindfightsHistoryComponent } from './mindfights-history.component';

describe('MindfightsHistoryComponent', () => {
  let component: MindfightsHistoryComponent;
  let fixture: ComponentFixture<MindfightsHistoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MindfightsHistoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MindfightsHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
