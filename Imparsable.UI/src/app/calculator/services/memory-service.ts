import {OnDestroy, Service} from '@angular/core';
import {Subject, Subscription} from 'rxjs';
import {ChartConfiguration, ChartData} from 'chart.js';
import {Callback} from '@shared/utils/types';
import {Calculator} from 'imp-wasm';

@Service()
export class MemoryService implements OnDestroy {
  private readonly subscriptions: Subscription[] = [];
  readonly allocated: Subject<number> = new Subject<number>();
  readonly reclaimed: Subject<number> = new Subject<number>();
  readonly compressed: Subject<number> = new Subject<number>();

  private readonly allocatedCallback: Callback<number> = (value: number): void => this.allocated.next(value);
  private readonly reclaimedCallback: Callback<number> = (value: number): void => this.reclaimed.next(value);
  private readonly compressedCallback: Callback<number> = (value: number): void => this.compressed.next(value);

  private index = 0;


  public update: Subject<void> = new Subject<void>();
  public readonly data: ChartData<'line'> = {
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

  public readonly options: ChartConfiguration<'line'>['options'] = {
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


  constructor() {
    Calculator.onAllocated.subscribe(this.allocatedCallback);
    Calculator.onReclaimed.subscribe(this.reclaimedCallback);
    Calculator.onCompressed.subscribe(this.compressedCallback);

    this.subscriptions.push(
      this.allocated.subscribe(value => this.add(0, value)),
      this.reclaimed.subscribe(value => this.add(1, value)),
      this.compressed.subscribe(value => this.add(2, value))
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
    Calculator.onAllocated.unsubscribe(this.allocatedCallback);
    Calculator.onReclaimed.unsubscribe(this.reclaimedCallback);
    Calculator.onCompressed.unsubscribe(this.compressedCallback);

  }

  private add(dataset: number, value: number): void {
    this.data.labels!.push(++this.index);

    for (let index = 0; index < this.data.datasets.length; index++)
      this.data.datasets[index].data.push(index === dataset ? value : null);

    this.update.next();
  }

  public clear(): void {
    this.index = 0;
    this.data.labels = [];
    this.data.datasets.forEach(dataset => dataset.data = []);
  }
}
