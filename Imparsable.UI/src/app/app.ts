import {Component, ChangeDetectionStrategy, OnInit, AfterViewInit} from '@angular/core';
import {Layout} from './components/layout/layout';
import {RouterOutlet} from '@angular/router';

// import {LspService} from './services/lsp-service';

@Component({
  selector: 'app-root',
  imports: [Layout, RouterOutlet],
  templateUrl: './app.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './app.scss',
})
export class App implements OnInit, AfterViewInit {
  // private readonly languageServer: LspService = inject(LspService);

  async ngOnInit(): Promise<void> {
    // await this.languageServer.initialize();
    // this.languageServer.initialize().then().catch(console.error);
  }

  ngAfterViewInit(): void {
    // this.languageServer.initialize().then().catch(console.error);
  }
}
