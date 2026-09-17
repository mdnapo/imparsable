import {OnDestroy, Service, Signal, signal, WritableSignal} from '@angular/core';
import {Observable, Subject} from 'rxjs';
import type {editor, IDisposable} from 'monaco-editor';
import {Optional} from '@shared/utils/types';

export type ActionRegistration = () => editor.IActionDescriptor;

@Service()
export class WorkbenchService implements OnDestroy {
  private readonly _editor: WritableSignal<Optional<editor.IStandaloneCodeEditor>> = signal(undefined);
  public readonly editor: Signal<Optional<editor.IStandaloneCodeEditor>> = this._editor.asReadonly();

  private actions: ActionRegistration[] = [];
  private disposables: IDisposable[] = [];

  private readonly _editorInitialized: Subject<void> = new Subject<void>();
  public readonly editorInitialized: Observable<void> = this._editorInitialized.asObservable();

  public readonly setLeftView: Subject<number> = new Subject<number>();
  public readonly setRightView: Subject<number> = new Subject<number>();
  public readonly setBottomView: Subject<number> = new Subject<number>();

  ngOnDestroy(): void {
    this.disposables.forEach(disposable => disposable.dispose());
  }

  setEditor(editor: editor.IStandaloneCodeEditor): void {
    this._editor.set(editor);
    this.actions.forEach(action => this.disposables.push(editor.addAction(action())));
    this._editorInitialized.next();
  }

  registerAction(action: ActionRegistration): void {
    this.actions.push(action);
  }
}
