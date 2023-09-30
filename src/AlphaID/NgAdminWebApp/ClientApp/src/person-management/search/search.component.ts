import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  searchKeywords = new FormControl('');

  searchResults = '';

  search() {
    this.searchResults = 'Searching...Done!';
  }
}
