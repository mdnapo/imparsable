import {AfterViewInit, Component, inject, OnDestroy, ViewChild} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {CodeEditor} from '../code-editor/code-editor';
import {LanguageId} from '../../app.config.monaco';
import {Calculator} from "imp-wasm";
import {Diagnostic, SourceFile} from '../../app.models';
import {FileSystem} from '../../services/file-system';


const code: string = `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" + ': ' + area;
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
  private readonly fs: FileSystem = inject(FileSystem);

  private onStdOut: (output: string) =>
    void = (output: string) => this.editor.onOutput(output);

  private onDiagnosticPublished: (diagnostic: Diagnostic) =>
    void = (output: Diagnostic) => this.editor.onDiagnosticPublished(output);

  private onDisassemble: (disassembly: string) =>
    void = (disassembly: string) => this.editor.onDisassembly(disassembly);

  protected file!: SourceFile;

  ngAfterViewInit(): void {
    this.file = this.fs.registerFile('test.clc', code, LanguageId.Calculator);
    Calculator.onDisassemble.subscribe(this.onDisassemble);
    Calculator.onDiagnosticPublished.subscribe(this.onDiagnosticPublished);
    Calculator.onStdOut.subscribe(this.onStdOut);
  }

  ngOnDestroy(): void {
    Calculator.onDisassemble.unsubscribe(this.onDisassemble);
    Calculator.onDiagnosticPublished.unsubscribe(this.onDiagnosticPublished);
    Calculator.onStdOut.unsubscribe(this.onStdOut);
  }

  execute(code: string) {
    Calculator.execute(code);
  }

  disassemble(code: string) {
    Calculator.disassemble(code);
  }
}
