import {inject, OnDestroy, Service, signal, WritableSignal} from '@angular/core';
import {BehaviorSubject, Subscription} from 'rxjs';
import {readCalculatorWorkspace} from '@config/filesystem';
import {IdeFile, IdeTree} from '@shared/models/filesystem';
import {WorkbenchService} from '@calculator/services/workbench-service';

@Service()
export class FileService implements OnDestroy {
  private readonly workbench: WorkbenchService = inject(WorkbenchService);
  private readonly subscriptions: Subscription[] = [];
  readonly files: WritableSignal<IdeTree> = signal(new IdeTree());
  readonly file: BehaviorSubject<IdeFile | null> = new BehaviorSubject<IdeFile | null>(null);

  constructor() {
    this.subscriptions.push(
      this.workbench.editorInitialized.subscribe({
        next: () => this.updateModel()
      })
    );

    readCalculatorWorkspace()
      .then(entries => this.files.update(tree => tree.load(entries, '/workspace')));
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }

  private updateModel(): void {
    const file = this.file.value;
    const editor = this.workbench.editor();

    if (!file || !editor)
      return;

    editor.setModel(file.getModel());
  }

  public openFile(file: IdeFile): void {
    this.file.next(file);
    this.updateModel();
  }

  public closeFile(file: IdeFile): void {
    if (this.file.value === file) {
      this.file.next(null);
    }
    file.dispose();
  }
}

