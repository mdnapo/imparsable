import {Component, inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {BaseChartDirective} from "ng2-charts";
import {ToolWindow} from "@shared/components/tool-window/tool-window";
import {Subscription} from 'rxjs';
import {MemoryService} from '@calculator/services/memory-service';

@Component({
  selector: 'app-memory',
  imports: [
    BaseChartDirective,
    ToolWindow
  ],
  templateUrl: './memory.html',
  styleUrl: './memory.scss',
})
export class Memory implements OnInit, OnDestroy {
  @ViewChild(BaseChartDirective)
  private chart?: BaseChartDirective<'line'>;
  protected readonly service: MemoryService = inject(MemoryService);
  private readonly subscription: Subscription = new Subscription();

  ngOnInit(): void {
    this.subscription.add(
      this.service.update.subscribe({
        next: () => this.chart?.update('none')
      })
    );
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
