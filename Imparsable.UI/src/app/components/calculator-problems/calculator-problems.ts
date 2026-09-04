import {Component, inject} from '@angular/core';
import {CalculatorContext} from '../../services/calculator-context';
import {Diagnostic, DiagnosticSeverity} from '../../app.models';
import {AsyncPipe} from '@angular/common';
import {MatToolbar} from '@angular/material/toolbar';

@Component({
  selector: 'app-calculator-problems',
  imports: [
    AsyncPipe,
    MatToolbar
  ],
  templateUrl: './calculator-problems.html',
  styleUrl: './calculator-problems.scss',
})
export class CalculatorProblems {
  protected readonly context: CalculatorContext = inject(CalculatorContext);

  protected formatDiagnostic(line: Diagnostic): string {
    return `[${DiagnosticSeverity[line.severity]}][line: ${line.marker.line}, col: ${line.marker.column}] ${line.message}`;
  }
}
