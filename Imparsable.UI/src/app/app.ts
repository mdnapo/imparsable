import {Component, ChangeDetectionStrategy, inject, OnInit} from '@angular/core';
import { Layout } from './components/layout/layout';
import { RouterOutlet } from '@angular/router';
import {LspService} from './services/lsp-service';

@Component({
  selector: 'app-root',
  imports: [Layout, RouterOutlet],
  templateUrl: './app.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './app.scss',
})
export class App implements OnInit {
  private readonly languageServer: LspService = inject(LspService);

  ngOnInit(): void {
    this.languageServer.initialize();
  }
}
