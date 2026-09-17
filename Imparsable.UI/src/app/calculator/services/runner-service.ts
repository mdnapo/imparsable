import {inject, OnDestroy, Service} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {StdOutput} from '@shared/models/language';
import {Callback} from '@shared/utils/types';
import {Calculator} from 'imp-wasm';
import {WorkbenchService} from '@calculator/services/workbench-service';
import {MemoryService} from '@calculator/services/memory-service';
import {ProblemService} from '@calculator/services/problem-service';
import {FileService} from '@calculator/services/file-service';

@Service()
export class RunnerService implements OnDestroy {
  private readonly workbenchService: WorkbenchService = inject(WorkbenchService);
  private readonly problems: ProblemService = inject(ProblemService);
  private readonly memory: MemoryService = inject(MemoryService);
  private readonly context: FileService = inject(FileService);
  public readonly output: BehaviorSubject<StdOutput[]> = new BehaviorSubject([] as StdOutput[]);

  private readonly outputCallback: Callback<string> =
    (output: string): void => this.onOutput(output);
  private readonly executedCallback: Callback<void> =
    (): void => this.workbenchService.setBottomView.next(0);

  constructor() {
    Calculator.onStdOut.subscribe(this.outputCallback);
    Calculator.onExecuted.subscribe(this.executedCallback);
  }

  ngOnDestroy(): void {
    Calculator.onStdOut.unsubscribe(this.outputCallback);
    Calculator.onExecuted.unsubscribe(this.executedCallback);
  }

  execute(): void {
    if (this.context.file.value === null) return;

    this.memory.clear();
    this.problems.clear();
    this.output.next([]);

    Calculator.execute(this.context.file.value!.getValue());
  }

  private onOutput(output: string): void {
    this.output.value.push({id: this.output.value.length, text: output});
    this.output.next(this.output.value);
  }
}
