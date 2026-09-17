import {Service, signal, WritableSignal} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {readCalculatorWorkspace} from '@config/filesystem';
import {IdeFile, IdeTree} from '@shared/models/filesystem';

@Service()
export class FileService {
  readonly files: WritableSignal<IdeTree> = signal(new IdeTree());
  readonly file: BehaviorSubject<IdeFile | null> = new BehaviorSubject<IdeFile | null>(null);

  constructor() {
    readCalculatorWorkspace()
      .then(entries => this.files.update(tree => tree.load(entries, '/workspace')));
  }

  public openFile(file: IdeFile): void {
    this.file.next(file);
  }

  public closeFile(file: IdeFile): void {
    if (this.file.value === file) {
      this.file.next(null);
    }
    file.dispose();
  }
}

