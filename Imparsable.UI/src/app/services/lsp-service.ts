import {OnDestroy, Service} from '@angular/core';
import {LogLevel} from '@codingame/monaco-vscode-api';
import {RegisteredFileSystemProvider, registerFileSystemOverlay} from '@codingame/monaco-vscode-files-service-override';
import {LanguageClientWrapper} from 'monaco-languageclient/lcwrapper';
import {MonacoVscodeApiWrapper} from 'monaco-languageclient/vscodeApiWrapper';
import EditorWorker from '@codingame/monaco-vscode-editor-api/esm/vs/editor/editor.worker?worker';
import {LanguageId, registerCalculatorLanguage} from '../app.config.monaco';
import * as vscode from 'vscode';
import { IDisposable } from "@codingame/monaco-vscode-editor-api";

window.MonacoEnvironment = {
  getWorker(_moduleId, _label) {
    return new EditorWorker();
  }
};

@Service()
export class LspService implements OnDestroy {
  private vscode?: MonacoVscodeApiWrapper;
  private calculator?: LanguageClientWrapper;
  private initialization?: Promise<void>;
  private fsProvider!: RegisteredFileSystemProvider;
  private fsOverlay?: IDisposable;

  public async initialize(): Promise<void> {
    return this.initialization ??= this.runInitializers();
  }

  private async runInitializers(): Promise<void> {
    this.initializeFileSystem();
    await this.initializeVsCodeWrapper();
    await this.initializeClcClient();
  }

  private initializeFileSystem(): void {
    this.fsProvider = new RegisteredFileSystemProvider(false);
    this.fsOverlay = registerFileSystemOverlay(-1, this.fsProvider);
  }

  private async initializeVsCodeWrapper(): Promise<void> {
    this.vscode = new MonacoVscodeApiWrapper({
      $type: 'classic',
      viewsConfig: {$type: 'EditorService'},
      logLevel: LogLevel.Debug,
    });

    await this.vscode.start();
  }

  private async initializeClcClient(): Promise<void> {
    registerCalculatorLanguage();

    this.calculator = new LanguageClientWrapper({
      languageId: LanguageId.Calculator,
      connection: {
        options: {
          $type: 'WebSocketUrl',
          url: 'wss://localhost:5001/lsp/clc',
        }
      },
      clientOptions: {
        documentSelector: [{language: LanguageId.Calculator}],
        workspaceFolder: {
          index: 0,
          name: 'workspace',
          uri: vscode.Uri.parse(`file:///workspace`)
        },
      }
    });

    await this.calculator.start();
  }

  ngOnDestroy(): void {
    this.calculator?.dispose();
    this.vscode?.dispose();
    this.fsOverlay?.dispose();
  }
}
