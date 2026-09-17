import {Component, inject} from '@angular/core';
import {ToolWindow} from "@shared/components/tool-window/tool-window";
import {GrammarService} from '@calculator/services/grammar-service';

@Component({
  selector: 'app-grammar',
  imports: [
    ToolWindow
  ],
  templateUrl: './grammar.html',
  styleUrl: './grammar.scss',
})
export class Grammar {
  protected readonly service: GrammarService = inject(GrammarService);
}
