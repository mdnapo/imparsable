import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {AsyncPipe} from '@angular/common';
import {CalculatorContext} from '../../services/calculator-context';
import {Subscription} from 'rxjs';

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
export class CalculatorRunner implements OnInit, OnDestroy {
  protected readonly context: CalculatorContext = inject(CalculatorContext);
  private readonly subscription: Subscription = new Subscription();

  ngOnInit(): void {
    this.subscription.add(this.context.allocated.subscribe({next: value => console.log(`Allocated ${value} bytes`)}));
    this.subscription.add(this.context.reclaimed.subscribe({next: value => console.log(`Reclaimed ${value} bytes`)}));
    this.subscription.add(this.context.compressed.subscribe({next: value => console.log(`Compressed ${value} bytes`)}));
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
