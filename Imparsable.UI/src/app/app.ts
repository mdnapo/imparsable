import {Component, ChangeDetectionStrategy, OnInit, inject} from '@angular/core';
import {Layout} from './components/layout/layout';
import {RouterOutlet} from '@angular/router';
import {MatIconRegistry} from '@angular/material/icon';

@Component({
  selector: 'app-root',
  imports: [Layout, RouterOutlet],
  templateUrl: './app.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private readonly iconRegistry = inject(MatIconRegistry);

  ngOnInit(): void {
    this.iconRegistry.setDefaultFontSetClass('material-symbols-outlined');
  }
}
