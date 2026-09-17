import {inject, OnDestroy, Service} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {WorkbenchService} from '@calculator/services/workbench-service';
import {Callback} from '@shared/utils/types';
import {Calculator} from 'imp-wasm';
import {FileService} from '@calculator/services/file-service';
import {ProblemService} from '@calculator/services/problem-service';

@Service()
export class DisassemblerService implements OnDestroy {
  private readonly workbenchService: WorkbenchService = inject(WorkbenchService);
  private readonly context: FileService = inject(FileService);
  private readonly problems: ProblemService = inject(ProblemService);
  readonly disassembly: BehaviorSubject<string> = new BehaviorSubject("");

  private readonly disassemblyCallback: Callback<string> =
    (output: string): void => this.onDisassembly(output);
  private readonly disassembledCallback: Callback<void> =
    (): void => this.workbenchService.setBottomView.next(1);

  constructor() {
    Calculator.onDisassemble.subscribe(this.disassemblyCallback);
    Calculator.onDisassembled.subscribe(this.disassembledCallback);
  }

  ngOnDestroy(): void {
    Calculator.onDisassemble.unsubscribe(this.disassemblyCallback);
    Calculator.onDisassembled.unsubscribe(this.disassembledCallback);
  }

  private onDisassembly(output: string): void {
    this.disassembly.next(output.trim());
  }

  execute(): void {
    if (this.context.file.value === null) return;

    this.problems.clear();
    this.disassembly.next('');

    Calculator.disassemble(this.context.file.value!.getValue());
  }
}
