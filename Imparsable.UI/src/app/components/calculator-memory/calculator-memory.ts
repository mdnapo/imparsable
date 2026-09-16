import {Component, inject, OnDestroy, OnInit} from '@angular/core';
import {ChartConfiguration, ChartData} from 'chart.js';
import {BaseChartDirective} from 'ng2-charts';
import {Subscription} from 'rxjs';
import {CalculatorContext} from '../../services/calculator-context';
import {ViewChild} from '@angular/core';
import {MatToolbar} from '@angular/material/toolbar';

@Component({
  selector: 'app-calculator-memory',
  imports: [
    BaseChartDirective,
    MatToolbar
  ],
  templateUrl: './calculator-memory.html',
  styleUrl: './calculator-memory.scss'
})
export class CalculatorMemory implements OnInit, OnDestroy {
  @ViewChild(BaseChartDirective)
  private chart?: BaseChartDirective<'line'>;
  private readonly context: CalculatorContext = inject(CalculatorContext);
  private readonly subscriptions: Subscription[] = [];

  protected readonly data: ChartData<'line'> = {
    labels: [],
    datasets: [
      {
        label: 'Allocated',
        data: []
      },
      {
        label: 'Reclaimed',
        data: []
      },
      {
        label: 'Compressed',
        data: []
      }
    ]
  };

  protected readonly options: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    animation: false,
    scales: {
      y: {
        beginAtZero: true,
        title: {
          display: true,
          text: 'Bytes'
        }
      }
    }
  };

  private index = 0;

  ngOnInit(): void {
    this.subscriptions.push(
      this.context.allocated.subscribe(value => this.add(0, value)),
      this.context.reclaimed.subscribe(value => this.add(1, value)),
      this.context.compressed.subscribe(value => this.add(2, value)),
      this.context.executed.subscribe(() => this.reset())
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  private add(dataset: number, value: number): void {
    this.data.labels!.push(++this.index);

    for (let index = 0; index < this.data.datasets.length; index++)
      this.data.datasets[index].data.push(index === dataset ? value : null);

    this.chart?.update('none');
  }

  private reset(): void {
    // this.index = 0;
    // this.data.labels = [];
    // this.data.datasets.forEach(dataset => dataset.data = []);
    // this.chart?.update('none');
  }
}
