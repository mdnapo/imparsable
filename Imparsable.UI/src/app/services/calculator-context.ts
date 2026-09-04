import {Service, signal, WritableSignal} from '@angular/core';
import {editor} from 'monaco-editor';
import {BehaviorSubject} from 'rxjs';
import {Diagnostic, StdOutput} from '../app.models';

@Service()
export class CalculatorContext {
  readonly model: WritableSignal<editor.ITextModel | undefined> = signal<editor.ITextModel | undefined>(undefined);
  readonly output: BehaviorSubject<StdOutput[]> = new BehaviorSubject([] as StdOutput[]);
  readonly diagnostics: BehaviorSubject<Diagnostic[]> = new BehaviorSubject([] as Diagnostic[]);
  readonly disassembly: BehaviorSubject<string> = new BehaviorSubject("");
}
