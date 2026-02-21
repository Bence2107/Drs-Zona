import {Component, OnInit} from '@angular/core';
import {RouterLink, RouterLinkActive, RouterOutlet} from '@angular/router';
import {HeaderComponent} from './components/header/header.component';
import {MatSidenav, MatSidenavContainer, MatSidenavContent} from '@angular/material/sidenav';
import {MatIcon} from '@angular/material/icon';
import {BreakpointObserver } from '@angular/cdk/layout';

import {MatDivider} from '@angular/material/list';
import {
  MatAccordion,
  MatExpansionPanel,
  MatExpansionPanelHeader,
  MatExpansionPanelTitle
} from '@angular/material/expansion';
import {MatIconButton} from '@angular/material/button';
import {AuthService} from './services/auth.service';
import {CustomSnackbarComponent} from './components/custom-snackbar/custom-snackbar.component';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, MatSidenavContainer, MatSidenav, RouterLink, RouterLinkActive, MatIcon, MatSidenavContent, MatDivider, MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle, MatIconButton],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit{
  title = 'drs-zona';
  isSmallScreen = false;
  isMobileOverlay = false;

  constructor(private breakpointObserver: BreakpointObserver, private authService: AuthService, private snackBar: MatSnackBar) {
     this.authService.loadProfile();
  }

  ngOnInit() {
    this.breakpointObserver.observe(['(max-width: 1280px)'])
      .subscribe(result => {
        this.isSmallScreen = result.matches;
      });

    this.breakpointObserver.observe(['(max-width: 990px)'])
      .subscribe(result => {
        this.isMobileOverlay = result.matches;
      });
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }

  logout() {
    this.snackBar.openFromComponent(CustomSnackbarComponent, {
      data: { message: 'Kijelentkezve', actionLabel: 'Rendben' },
      duration: 3000,
      horizontalPosition: 'center',
    });
    return this.authService.logout();
  }

  get username(): string | null {
    return this.authService.currentProfile()?.username ?? null;
  }

  get hasProfileData(): boolean {
    return !!this.authService.currentProfile();
  }

  isAuthor(): boolean {
    return this.authService.currentProfile()?.role === 'Author';
  }
}
