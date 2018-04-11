import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMindfightComponent } from './edit-mindfight.component';

describe('EditMindfightComponent', () => {
  let component: EditMindfightComponent;
  let fixture: ComponentFixture<EditMindfightComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditMindfightComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMindfightComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
