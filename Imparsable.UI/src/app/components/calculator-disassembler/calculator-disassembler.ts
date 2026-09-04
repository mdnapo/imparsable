import {AfterViewInit, Component, inject, OnDestroy} from '@angular/core';
import {CalculatorContext} from '../../services/calculator-context';
import {Calculator} from "imp-wasm";
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
export class CalculatorDisassembler implements AfterViewInit, OnDestroy {
  protected readonly context: CalculatorContext = inject(CalculatorContext);
  private readonly disassemblyCallback: (output: string) => void = (output: string) => this.onDisassembly(output);

  ngAfterViewInit(): void {
    Calculator.onDisassemble.subscribe(this.disassemblyCallback);
  }

  ngOnDestroy(): void {
    Calculator.onDisassemble.unsubscribe(this.disassemblyCallback);
  }

  protected run() {
    Calculator.disassemble(this.context.model()?.getValue()!);
  }

  public onDisassembly(output: string): void {
    this.context.disassembly.next(output.trim());
  }
}
