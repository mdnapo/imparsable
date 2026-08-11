import { Component, ChangeDetectionStrategy } from '@angular/core';
import {EditorComponent} from 'ngx-monaco-editor-v2';
import {editor} from 'monaco-editor';
import IStandaloneEditorConstructionOptions = editor.IStandaloneEditorConstructionOptions;
import {FormsModule} from '@angular/forms';

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
  code = `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" . ': ' . area;
print 1 + "2";
`

  protected options: IStandaloneEditorConstructionOptions = {
    language: 'clc',
    automaticLayout: true,
  };
}
