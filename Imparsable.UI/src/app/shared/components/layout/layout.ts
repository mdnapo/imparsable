import {Component, inject} from '@angular/core';
import {Router, RouterLink, RouterOutlet} from '@angular/router';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatListItem, MatListItemMeta, MatNavList} from '@angular/material/list';
import {MatSidenav, MatSidenavContainer, MatSidenavContent} from '@angular/material/sidenav';
import {MatToolbar} from '@angular/material/toolbar';

@Component({
  selector: 'app-layout',
  imports: [
    MatIcon,
    MatIconButton,
    MatListItem,
    MatListItemMeta,
    MatNavList,
    MatSidenav,
    MatSidenavContainer,
    MatSidenavContent,
    MatToolbar,
    RouterLink,
    RouterOutlet
  ],
  templateUrl: './layout.html',
  styleUrl: './layout.scss',
})
export class Layout {
  protected readonly router: Router = inject(Router);
  protected openSideNav: boolean = true;
  protected menuIconRotatedState: string = 'default';

  toggleNav(): void {
    this.openSideNav = !this.openSideNav;
    this.menuIconRotatedState = this.menuIconRotatedState === 'default' ? 'rotated' : 'default';
  }

  urlMatches(url: string): boolean {
    const queryParamsIndex = this.router.url.indexOf('?');
    const baseUrl = queryParamsIndex === -1
      ? this.router.url
      : this.router.url.slice(0, queryParamsIndex);
    return baseUrl === url;
  }
}
