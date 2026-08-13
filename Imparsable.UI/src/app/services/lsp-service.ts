import {OnDestroy, Service} from '@angular/core';
import {MonacoVscodeApiWrapper} from 'monaco-languageclient/vscodeApiWrapper';
import {LogLevel} from '@codingame/monaco-vscode-api';
import {configureDefaultWorkerFactory} from 'monaco-languageclient/workerFactory';
import * as monaco from '@codingame/monaco-vscode-editor-api';
import {
  RegisteredFileSystemProvider,
  registerFileSystemOverlay
} from '@codingame/monaco-vscode-files-service-override';
import {LanguageClientWrapper,} from 'monaco-languageclient/lcwrapper';
import * as vscode from 'vscode';

import EditorWorker from '@codingame/monaco-vscode-editor-api/esm/vs/editor/editor.worker?worker';

window.MonacoEnvironment = {
  getWorker(_moduleId, _label) {
    return new EditorWorker();
  }
};

@Service()
export class LspService implements OnDestroy {
  private vscodeApi?: MonacoVscodeApiWrapper;
  private clcClient?: LanguageClientWrapper;
  private vscodeApiInit?: Promise<void>;
  private clcClientInit?: Promise<void>;

  public initialize(): void {
    monaco.languages.register({
      id: 'clc',
      extensions: ['.clc'],
      aliases: ['CLC']
    });
    const fileSystemProvider = new RegisteredFileSystemProvider(false);
    // fileSystemProvider.registerFile(new RegisteredMemoryFile(helloUri, helloCode));
    registerFileSystemOverlay(1, fileSystemProvider);
    // this.vscodeApiInit ??= this.initializeCore();
    // return this.vscodeApiInit;
  }

  // private async initializeCore(): Promise<void> {
  //   await this.initializeVsCodeWrapper();
  //   await this.initializeClcClient();
  // }

  public async initializeVsCodeWrapper(): Promise<void> {
    this.vscodeApi = new MonacoVscodeApiWrapper({
      $type: 'extended',
      viewsConfig: {$type: 'EditorService'},
      logLevel: LogLevel.Debug,
      // monacoWorkerFactory: configureDefaultWorkerFactory
    });

    this.vscodeApiInit ??= this.vscodeApi.start();

    return this.vscodeApiInit;
  }

  public async initializeClcClient(): Promise<void> {
    this.clcClient = new LanguageClientWrapper({
      languageId: 'clc',
      connection: {
        options: {
          $type: 'WebSocketUrl',
          url: 'wss://localhost:5001/lsp/clc',
        }
      },
      clientOptions: {
        documentSelector: [{ language: 'clc' }],
        workspaceFolder: {
          index: 0,
          name: 'workspace',
          uri: vscode.Uri.parse(`file:///workspace`)
        }
      }
    });

    this.clcClientInit ??= this.clcClient.start();

    return this.clcClientInit;
  }

  ngOnDestroy(): void {
    this.clcClient?.dispose();
    this.vscodeApi?.dispose();
  }
}
