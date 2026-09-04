import {Component, inject} from '@angular/core';
import {CalculatorContext} from '../../services/calculator-context';
import {AsyncPipe} from '@angular/common';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {MatToolbar} from '@angular/material/toolbar';

@Component({
  selector: 'app-calculator-disassembler',
  imports: [
    AsyncPipe,
    MatIcon,
    MatIconButton,
    MatToolbar
  ],
  templateUrl: './calculator-disassembler.html',
  styleUrl: './calculator-disassembler.scss',
})
export class CalculatorDisassembler {
  protected readonly context: CalculatorContext = inject(CalculatorContext);
}
