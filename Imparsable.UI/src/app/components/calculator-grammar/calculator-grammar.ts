import {Component, inject, OnInit, signal, WritableSignal} from '@angular/core';
import {CalculatorService} from '../../../api/imparsable';

@Component({
  selector: 'app-calculator-grammar',
  imports: [],
  templateUrl: './calculator-grammar.html',
  styleUrl: './calculator-grammar.scss',
})
export class CalculatorGrammar implements OnInit {
  private readonly calc: CalculatorService = inject(CalculatorService);
  protected grammar: WritableSignal<string> = signal('');

  ngOnInit(): void {
    this.calc.grammar()
      .subscribe(res => this.grammar.set(JSON.parse(res)));
  }
}
