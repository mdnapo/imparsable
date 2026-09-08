import {Routes} from '@angular/router';
import {CalculatorIde} from './components/calculator-ide/calculator-ide';
import {CalculatorGrammar} from './components/calculator-grammar/calculator-grammar';

export const routes: Routes = [
  {path: "calculator", component: CalculatorIde},
  {path: "calculator/grammar", component: CalculatorGrammar},
];
