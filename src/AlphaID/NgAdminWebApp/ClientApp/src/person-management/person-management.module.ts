import { Component, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PersonManagementComponent } from './person-management.component';
import { OverviewComponent } from './overview/overview.component';
import { RouterModule } from '@angular/router';
import { SearchComponent } from './search/search.component';
import { ReactiveFormsModule } from '@angular/forms';



@NgModule({
  declarations: [
    PersonManagementComponent,
    OverviewComponent,
    SearchComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild([
      {
        path: "people", component: PersonManagementComponent, children: [
          { path: "", component: OverviewComponent },
          { path: "search", component: SearchComponent },
        ]
      },
    ]),
    ReactiveFormsModule
  ],
})
export class PersonManagementModule { }
