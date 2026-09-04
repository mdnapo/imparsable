import {AfterViewInit, Component, inject, OnDestroy} from '@angular/core';
import {CalculatorContext} from '../../services/calculator-context';
import {Diagnostic, DiagnosticSeverity} from '../../app.models';
import {Calculator} from "imp-wasm";
import {AsyncPipe} from '@angular/common';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatToolbar} from '@angular/material/toolbar';

@Component({
  selector: 'app-calculator-problems',
  imports: [
    AsyncPipe,
    MatIcon,
    MatIconButton,
    MatToolbar
  ],
  templateUrl: './calculator-problems.html',
  styleUrl: './calculator-problems.scss',
})
export class CalculatorProblems implements AfterViewInit, OnDestroy {
  protected readonly context: CalculatorContext = inject(CalculatorContext);

  private diagnosticsSubscription: (diagnostic: Diagnostic) =>
    void = (output: Diagnostic) => this.onDiagnosticPublished(output);

  ngAfterViewInit(): void {
    Calculator.onDiagnosticPublished.subscribe(this.diagnosticsSubscription);
  }

  ngOnDestroy(): void {
    Calculator.onDiagnosticPublished.unsubscribe(this.diagnosticsSubscription);
  }

  protected formatDiagnostic(line: Diagnostic): string {
    return `[${DiagnosticSeverity[line.severity]}][line: ${line.marker.line}, col: ${line.marker.column}] ${line.message}`;
  }

  private onDiagnosticPublished(diagnostic: Diagnostic): void {
    this.context.diagnostics.value.push(diagnostic);
    this.context.diagnostics.next(this.context.diagnostics.value.sort((l, r) => l.marker.column - r.marker.column));
  }
}
