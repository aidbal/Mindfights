import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MindfightsListComponent } from './mindfights-list.component';

describe('MindfightsListComponent', () => {
  let component: MindfightsListComponent;
  let fixture: ComponentFixture<MindfightsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MindfightsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MindfightsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
