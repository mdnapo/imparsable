import {Routes} from '@angular/router';
import {CodeEditor} from './components/code-editor/code-editor';
import {Ide} from './components/ide/ide';
// import {CalculatorEditor} from './components/calculator-editor/calculator-editor';

export const routes: Routes = [
  {path: "calculator", component: Ide},
  // {path: "calculator", component: CalculatorEditor}
];
