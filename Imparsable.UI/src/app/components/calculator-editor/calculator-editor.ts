import {Component, OnInit} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {CodeEditor} from '../code-editor/code-editor';

const code: string = `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" . ': ' . area;
print 1 + "2";
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
export class CalculatorEditor implements OnInit {
    ngOnInit(): void {
    }
}
