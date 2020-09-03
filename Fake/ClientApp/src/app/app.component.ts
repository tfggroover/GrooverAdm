import { Component } from '@angular/core';
import { LoadingService } from './services/loadingService';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {

  public loadingClass = 'main-loader';


  constructor(private loading: LoadingService) {
    this.loading.loadingStatus.subscribe(active => {
      this.loadingClass = active ? '' : 'main-loader';
    });
  }
  title = 'app';
}
