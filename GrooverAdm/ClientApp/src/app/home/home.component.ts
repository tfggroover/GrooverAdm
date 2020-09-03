import { Component, Inject, SecurityContext } from '@angular/core';
import { HttpClient, HttpBackend, HttpHeaders } from '@angular/common/http';
import { faSpotify } from '@fortawesome/free-brands-svg-icons';
import { SafeResourceUrl, DomSanitizer } from '@angular/platform-browser';
import { AuthorizeService } from 'src/api-authorization/authorize.service';
import { map } from 'rxjs/internal/operators/map';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  faspoty = faSpotify;
  public timer;
  url: string;
  public isAuthenticated: boolean;
  public userName: string;
  constructor(sanitizer: DomSanitizer, @Inject('BASE_URL') public baseUrl: string, public authService: AuthorizeService) {
    this.url = sanitizer.sanitize(SecurityContext.URL, baseUrl + 'home/auth');
    this.authService.userChanged.subscribe(u => {
      this.isAuthenticated = !!u;
      this.userName = u?.name;
    });
    this.authService.trySignInSilent();
  }

  public onSignInButtonClick() {
    // Open the Auth flow in a popup.
    this.authService.signIn();
  }

}
