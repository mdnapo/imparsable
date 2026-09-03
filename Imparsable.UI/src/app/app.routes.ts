import {Routes} from '@angular/router';
import {CodeEditor} from './components/code-editor/code-editor';
import {CalculatorIde} from './components/calculator-ide/calculator-ide';
// import {CalculatorEditor} from './components/calculator-editor/calculator-editor';

export const routes: Routes = [
  {path: "calculator", component: CalculatorIde},
  // {path: "calculator", component: Ide2},
  // {path: "calculator", component: CalculatorEditor}
];
