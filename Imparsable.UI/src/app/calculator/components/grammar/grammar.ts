import {Component, computed, inject, OnInit, signal, WritableSignal} from '@angular/core';
import {ToolWindow} from "@shared/components/tool-window/tool-window";
import {CalculatorService} from '@api/imparsable';
import Prism from 'prismjs';

@Component({
  selector: 'app-grammar',
  imports: [
    ToolWindow
  ],
  templateUrl: './grammar.html',
  styleUrl: './grammar.scss',
})
export class Grammar implements OnInit {
  private readonly calc: CalculatorService = inject(CalculatorService);
  private grammar: WritableSignal<string> = signal('');

  protected readonly highlightedGrammar = computed(() =>
    Prism.highlight(this.grammar(), Prism.languages['antlr4'], 'antlr4')
  );

  ngOnInit(): void {
    this.calc.grammar()
      .subscribe(res => this.grammar.set(JSON.parse(res)));
  }
}
