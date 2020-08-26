import { Component, Inject } from '@angular/core';
import { HttpClient, HttpBackend } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public HomeComponent(handler: HttpBackend, @Inject('BASE_URL') baseUrl: string) {
    const http = new HttpClient(handler);
    http.get(baseUrl + 'home/auth').subscribe(result => {
      console.log(result);
    }, error => console.error(error));
  }

  public onSignInButtonClick() {
    // Open the Auth flow in a popup.
    window.open('/home/auth', 'firebaseAuth', 'height=315,width=400');
  }
}
