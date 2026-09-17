import {computed, inject, Service, signal, WritableSignal} from '@angular/core';
import {CalculatorService} from '@api/imparsable';
import Prism from 'prismjs';
import 'prismjs/components/prism-antlr4';

@Service()
export class GrammarService {
  private readonly calc: CalculatorService = inject(CalculatorService);
  public _grammar: WritableSignal<string> = signal('');

  public readonly grammar = computed(() =>
    Prism.highlight(this._grammar(), Prism.languages['antlr4'], 'antlr4')
  );

  constructor() {
    this.calc.grammar()
      .subscribe(res => this._grammar.set(JSON.parse(res)));
  }
}
