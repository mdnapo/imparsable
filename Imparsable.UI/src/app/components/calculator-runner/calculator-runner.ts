import {AfterViewInit, Component, inject, OnDestroy} from '@angular/core';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {BehaviorSubject} from 'rxjs';
import {StdOutput} from '../../app.models';
import {Calculator} from "imp-wasm";
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
export class CalculatorRunner implements AfterViewInit, OnDestroy {
  private readonly context: CalculatorContext = inject(CalculatorContext);
  protected output: BehaviorSubject<StdOutput[]> = new BehaviorSubject([] as StdOutput[]);
  private subscription: (output: string) => void = (output: string) => this.onOutput(output);

  ngAfterViewInit(): void {
    Calculator.onStdOut.subscribe(this.subscription);
  }

  ngOnDestroy(): void {
    Calculator.onStdOut.unsubscribe(this.subscription);
  }

  protected run() {
    console.log(this.context.model()?.getValue());
    Calculator.execute(this.context.model()?.getValue()!);
  }

  private onOutput(output: string): void {
    this.output.value.push({id: this.output.value.length, text: output});
    this.output.next(this.output.value);
  }
}
