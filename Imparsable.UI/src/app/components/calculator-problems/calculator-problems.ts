import {Component, inject} from '@angular/core';
import {AsyncPipe} from '@angular/common';
import {MatIcon} from '@angular/material/icon';
import {MatToolbar} from '@angular/material/toolbar';
import {CalculatorContext} from '../../services/calculator-context';
import {Diagnostic, DiagnosticSeverity} from '../../app.models';

@Component({
  selector: 'app-calculator-problems',
  imports: [
    AsyncPipe,
    MatIcon,
    MatToolbar,
  ],
  templateUrl: './calculator-problems.html',
  styleUrl: './calculator-problems.scss',
})
export class CalculatorProblems {
  protected readonly context: CalculatorContext = inject(CalculatorContext);

  protected severityIcon(diagnostic: Diagnostic): string {
    switch (diagnostic.severity) {
      case DiagnosticSeverity.ERROR:
        return 'error';
      case DiagnosticSeverity.WARNING:
        return 'warning';
      default:
        return 'info';
    }
  }

  protected severityClass(diagnostic: Diagnostic): string {
    return `severity-${DiagnosticSeverity[diagnostic.severity].toLowerCase()}`;
  }
}
