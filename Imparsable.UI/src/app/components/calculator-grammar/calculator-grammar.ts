import {Component, computed, inject, OnInit, signal, WritableSignal} from '@angular/core';
import {CalculatorService} from '../../../api/imparsable';
import Prism from 'prismjs';
import 'prismjs/components/prism-antlr4';
import 'prism-themes/themes/prism-vs.min.css';

@Component({
  selector: 'app-calculator-grammar',
  imports: [],
  templateUrl: './calculator-grammar.html',
  styleUrl: './calculator-grammar.scss',
})
export class CalculatorGrammar implements OnInit {
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
