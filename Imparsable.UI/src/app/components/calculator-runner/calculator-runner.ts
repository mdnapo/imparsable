import {Component, inject} from '@angular/core';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {AsyncPipe} from '@angular/common';
import {CalculatorContext} from '../../services/calculator-context';

@Component({
  selector: 'app-calculator-runner',
  imports: [
    MatToolbar,
    MatIconButton,
    MatIcon,
    AsyncPipe
  ],
  templateUrl: './calculator-runner.html',
  styleUrl: './calculator-runner.scss',
})
export class CalculatorRunner {
  protected readonly context: CalculatorContext = inject(CalculatorContext);
}
