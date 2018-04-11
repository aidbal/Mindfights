import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrateMindfightsComponent } from './administrate-mindfights.component';

describe('AdministrateMindfightsComponent', () => {
  let component: AdministrateMindfightsComponent;
  let fixture: ComponentFixture<AdministrateMindfightsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdministrateMindfightsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrateMindfightsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
