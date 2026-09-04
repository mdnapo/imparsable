import {OnDestroy, Service, signal, WritableSignal} from '@angular/core';
import {editor} from 'monaco-editor';
import {BehaviorSubject} from 'rxjs';
import {Diagnostic, DiagnosticSeverity, StdOutput} from '../app.models';
import {Calculator} from "imp-wasm";

type Callback<T> = (value: T) => void;

@Service()
export class CalculatorContext implements OnDestroy {
  readonly model: WritableSignal<editor.ITextModel | undefined> = signal(undefined);
  readonly errors: WritableSignal<number> = signal(0);
  readonly output: BehaviorSubject<StdOutput[]> = new BehaviorSubject([] as StdOutput[]);
  readonly diagnostics: BehaviorSubject<Diagnostic[]> = new BehaviorSubject([] as Diagnostic[]);
  readonly disassembly: BehaviorSubject<string> = new BehaviorSubject("");
  readonly failure: BehaviorSubject<boolean> = new BehaviorSubject(false);

  private readonly outputCallback: Callback<string> =
    (output: string) => this.onOutput(output);

  private readonly diagnosticsSubscription: Callback<Diagnostic> =
    (output: Diagnostic) => this.onDiagnosticPublished(output);

  private readonly disassemblyCallback: Callback<string> =
    (output: string) => this.onDisassembly(output);

  constructor() {
    Calculator.onStdOut.subscribe(this.outputCallback);
    Calculator.onDiagnosticPublished.subscribe(this.diagnosticsSubscription);
    Calculator.onDisassemble.subscribe(this.disassemblyCallback);
  }

  ngOnDestroy(): void {
    Calculator.onStdOut.unsubscribe(this.outputCallback);
    Calculator.onDiagnosticPublished.unsubscribe(this.diagnosticsSubscription);
    Calculator.onDisassemble.unsubscribe(this.disassemblyCallback);
  }

  private onDiagnosticPublished(diagnostic: Diagnostic): void {
    this.diagnostics.value.push(diagnostic);
    this.diagnostics.next(this.diagnostics.value.sort((l, r) => l.marker.column - r.marker.column));
    this.notifyError();
  }

  public run(): void {
    this.output.next([]);
    this.diagnostics.next([]);
    this.errors.set(0);
    Calculator.execute(this.model()?.getValue()!);
  }

  public disassemble(): void {
    this.output.next([]);
    this.diagnostics.next([]);
    Calculator.disassemble(this.model()?.getValue()!);
  }

  private notifyError() {
    const errors = this.diagnostics.value.filter(x => x.severity == DiagnosticSeverity.ERROR);
    if (errors.length > 0) {
      this.errors.set(errors.length);
      this.failure.next(true);
    }
  }

  private onOutput(output: string): void {
    this.output.value.push({id: this.output.value.length, text: output});
    this.output.next(this.output.value);
  }

  private onDisassembly(output: string): void {
    this.disassembly.next(output.trim());
  }
}
