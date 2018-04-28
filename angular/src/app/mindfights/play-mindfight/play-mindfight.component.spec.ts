import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlayMindfightComponent } from './play-mindfight.component';

describe('PlayMindfightComponent', () => {
  let component: PlayMindfightComponent;
  let fixture: ComponentFixture<PlayMindfightComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlayMindfightComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlayMindfightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
