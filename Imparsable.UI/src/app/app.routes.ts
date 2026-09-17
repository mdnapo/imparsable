import {Routes} from '@angular/router';
import {Ide} from '@calculator/components/ide/ide';

export const routes: Routes = [
  {path: "calculator", component: Ide},
  {path: "**", redirectTo: "/calculator"},
];
