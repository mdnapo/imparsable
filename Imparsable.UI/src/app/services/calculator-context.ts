import {Service, signal, WritableSignal} from '@angular/core';
import {editor} from 'monaco-editor';

@Service()
export class CalculatorContext {
  readonly model: WritableSignal<editor.ITextModel | undefined> = signal<editor.ITextModel | undefined>(undefined);
}
