import {AfterViewInit, Component, OnDestroy, ViewChild} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {CodeEditor} from '../code-editor/code-editor';
import {LanguageId} from '../../app.config.monaco';
import {CalculatorVM} from "imp-wasm";
import {Diagnostic, SourceFile} from '../../app.models';


const code: string = `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" . ': ' . area;
print 1 + 2;

for (var x = 0; x < 3; x += 1)
    print x + 1;
`;

@Component({
  selector: 'app-calculator-editor',
  imports: [
    FormsModule,
    CodeEditor
  ],
  templateUrl: './calculator-editor.html',
  styleUrl: './calculator-editor.scss',
})
export class CalculatorEditor implements AfterViewInit, OnDestroy {
  @ViewChild('editor') editor!: CodeEditor;

  private onStdOut: (output: string) =>
    void = (output: string) => this.editor.onOutput(output);

  private onDiagnosticPublished: (diagnostic: Diagnostic) =>
    void = (output: Diagnostic) => this.editor.onDiagnosticPublished(output);

  protected readonly file: SourceFile = {
    name: 'test.clc',
    content: code,
    languageId: LanguageId.Calculator,
  }

  ngAfterViewInit(): void {
    CalculatorVM.onDiagnosticPublished.subscribe(this.onDiagnosticPublished);
    CalculatorVM.onStdOut.subscribe(this.onStdOut);
  }

  ngOnDestroy(): void {
    CalculatorVM.onDiagnosticPublished.unsubscribe(this.onDiagnosticPublished);
    CalculatorVM.onStdOut.unsubscribe(this.onStdOut);
  }

  execute(code: string) {
    CalculatorVM.execute(code);
  }
}
