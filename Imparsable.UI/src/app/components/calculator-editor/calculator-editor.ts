import {Component} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {CodeEditor, SourceFile} from '../code-editor/code-editor';
import {LanguageId} from '../../app.config.monaco';

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
export class CalculatorEditor {
  protected readonly file: SourceFile = {
    name: 'test.clc',
    content: code,
    languageId: LanguageId.Calculator,
  }
}
