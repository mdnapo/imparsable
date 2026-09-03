import {Routes} from '@angular/router';
import {CodeEditor} from './components/code-editor/code-editor';
import {Ide} from './components/ide/ide';
import {Ide2} from './components/ide2/ide2';
import {CalculatorIde} from './components/calculator-ide/calculator-ide';
// import {CalculatorEditor} from './components/calculator-editor/calculator-editor';

export const routes: Routes = [
  {path: "calculator", component: CalculatorIde},
  // {path: "calculator", component: Ide2},
  // {path: "calculator", component: CalculatorEditor}
];
