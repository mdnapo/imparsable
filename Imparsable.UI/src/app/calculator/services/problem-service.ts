import {OnDestroy, Service, signal, WritableSignal} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {Diagnostic, DiagnosticSeverity} from '@shared/models/language';
import {Callback} from '@shared/utils/types';
import {Calculator} from 'imp-wasm';

@Service()
export class ProblemService implements OnDestroy {
  readonly diagnostics: BehaviorSubject<Diagnostic[]> = new BehaviorSubject([] as Diagnostic[]);
  readonly errors: WritableSignal<number> = signal(0);

  private readonly diagnosticsSubscription: Callback<Diagnostic> =
    (output: Diagnostic): void => this.onDiagnosticPublished(output);

  constructor() {
    Calculator.onDiagnosticPublished.subscribe(this.diagnosticsSubscription);
  }

  ngOnDestroy(): void {
    Calculator.onDiagnosticPublished.unsubscribe(this.diagnosticsSubscription);
  }

  private onDiagnosticPublished(diagnostic: Diagnostic): void {
    this.diagnostics.value.push(diagnostic);
    this.diagnostics.next(this.diagnostics.value.sort((l, r) => l.marker.column - r.marker.column));
    this.errors.set(this.diagnostics.value.filter(x => x.severity == DiagnosticSeverity.ERROR).length);
  }

  clear(): void {
    this.diagnostics.next([]);
    this.errors.set(0);
  }
}
