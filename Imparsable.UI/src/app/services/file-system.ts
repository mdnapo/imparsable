import {OnDestroy, Service} from '@angular/core';
import {
  RegisteredFileSystemProvider,
  RegisteredMemoryFile,
  registerFileSystemOverlay
} from '@codingame/monaco-vscode-files-service-override';
import {IDisposable} from '@codingame/monaco-vscode-editor-api';
import * as monaco from '@codingame/monaco-vscode-editor-api';
import {SourceFile} from '../app.models';

@Service()
export class FileSystem implements OnDestroy {
  private fsProvider!: RegisteredFileSystemProvider;
  private fsOverlay!: IDisposable;

  public initialize(): void {
    this.fsProvider = new RegisteredFileSystemProvider(false);
    this.fsOverlay = registerFileSystemOverlay(-1, this.fsProvider);
  }

  public registerFile(name: string, code: string, languageId: string): SourceFile {
    const uri = monaco.Uri.parse(`file:///workspace/${name}`);
    this.fsProvider.registerFile(new RegisteredMemoryFile(uri, code));
    return {uri: uri, name: name, content: code, languageId: languageId};
  }

  ngOnDestroy(): void {
    this.fsOverlay?.dispose();
  }
}
