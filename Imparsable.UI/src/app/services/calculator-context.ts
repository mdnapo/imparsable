import {OnDestroy, Service, signal, WritableSignal} from '@angular/core';
import {BehaviorSubject, Subject} from 'rxjs';
import {Diagnostic, StdOutput} from '../app.models';
import {Calculator} from "imp-wasm";
import {IdeFile, IdeTree} from '../app.filesystem';
import {readCalculatorWorkspace} from '../app.config.filesystem';

type Callback<T> = (value: T) => void;

@Service()
export class CalculatorContext implements OnDestroy {
  readonly files: WritableSignal<IdeTree> = signal(new IdeTree());
  readonly errors: WritableSignal<number> = signal(0);
  readonly file: BehaviorSubject<IdeFile | null> = new BehaviorSubject<IdeFile | null>(null);
  readonly output: BehaviorSubject<StdOutput[]> = new BehaviorSubject([] as StdOutput[]);
  readonly diagnostics: BehaviorSubject<Diagnostic[]> = new BehaviorSubject([] as Diagnostic[]);
  readonly disassembly: BehaviorSubject<string> = new BehaviorSubject("");
  readonly failure: Subject<void> = new Subject<void>();
  readonly executed: Subject<void> = new Subject<void>();
  readonly disassembled: Subject<void> = new Subject<void>();

  private readonly outputCallback: Callback<string> =
    (output: string) => this.onOutput(output);

  private readonly diagnosticsSubscription: Callback<Diagnostic> =
    (output: Diagnostic) => this.onDiagnosticPublished(output);

  private readonly disassemblyCallback: Callback<string> =
    (output: string) => this.onDisassembly(output);

  private readonly executedCallback: Callback<void> = () => this.executed.next();
  private readonly disassembledCallback: Callback<void> = () => this.disassembled.next();
  private readonly failedCallback: Callback<void> = () => this.failure.next();

  constructor() {
    Calculator.onStdOut.subscribe(this.outputCallback);
    Calculator.onDiagnosticPublished.subscribe(this.diagnosticsSubscription);
    Calculator.onDisassemble.subscribe(this.disassemblyCallback);
    Calculator.onExecuted.subscribe(this.executedCallback);
    Calculator.onDisassembled.subscribe(this.disassembledCallback);
    Calculator.onFailure.subscribe(this.failedCallback);

    readCalculatorWorkspace()
      .then(entries => this.files.update(tree => tree.load(entries, '/workspace')));
  }

  ngOnDestroy(): void {
    Calculator.onStdOut.unsubscribe(this.outputCallback);
    Calculator.onDiagnosticPublished.unsubscribe(this.diagnosticsSubscription);
    Calculator.onDisassemble.unsubscribe(this.disassemblyCallback);
    Calculator.onExecuted.unsubscribe(this.executedCallback);
    Calculator.onDisassembled.unsubscribe(this.disassembledCallback);
    Calculator.onFailure.unsubscribe(this.failedCallback);
  }

  private onDiagnosticPublished(diagnostic: Diagnostic): void {
    this.diagnostics.value.push(diagnostic);
    this.diagnostics.next(this.diagnostics.value.sort((l, r) => l.marker.column - r.marker.column));
  }

  public execute(): void {
    if (this.file.value === null) return;

    this.output.next([]);
    this.diagnostics.next([]);
    this.errors.set(0);
    Calculator.execute(this.file.value!.getValue());
  }

  public disassemble(): void {
    if (this.file.value === null) return;

    this.output.next([]);
    this.diagnostics.next([]);
    this.errors.set(0);
    Calculator.disassemble(this.file.value!.getValue());
  }

  private onOutput(output: string): void {
    this.output.value.push({id: this.output.value.length, text: output});
    this.output.next(this.output.value);
  }

  private onDisassembly(output: string): void {
    this.disassembly.next(output.trim());
  }

  public openFile(file: IdeFile): void {
    this.file.next(file);
  }

  public closeFile(file: IdeFile): void {
    if (this.file.value === file) {
      this.file.next(null);
    }
    file.dispose();
  }
}
