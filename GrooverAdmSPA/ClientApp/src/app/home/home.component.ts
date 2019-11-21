import { Component, Inject } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  public baseUrl: string;

  constructor(@Inject('BASE_URL') baseUrl: string){
    this.baseUrl = baseUrl;
  }

  public redirect(){
    window.open(this.baseUrl + 'home/auth', 'firebaseAuth');
  }
}
