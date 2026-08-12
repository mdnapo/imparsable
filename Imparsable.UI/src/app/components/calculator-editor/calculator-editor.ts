import {Component, ChangeDetectionStrategy, model} from '@angular/core';
import {EditorComponent, NgxEditorModel} from 'ngx-monaco-editor-v2';
import {editor, Uri} from 'monaco-editor';
import IStandaloneEditorConstructionOptions = editor.IStandaloneEditorConstructionOptions;
import {FormsModule} from '@angular/forms';

const code: string =  `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" . ': ' . area;
print 1 + "2";
`;

@Component({
  selector: 'app-calculator-editor',
  imports: [
    EditorComponent,
    FormsModule
  ],
  templateUrl: './calculator-editor.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './calculator-editor.scss',
})
export class CalculatorEditor {
  protected options: IStandaloneEditorConstructionOptions = {
    automaticLayout: true,
  };

  protected model: NgxEditorModel = {
    value: code,
    language: 'clc',
    uri: Uri.parse("file:///workspace/test.clc")
  };
}
