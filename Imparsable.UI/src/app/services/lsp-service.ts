// import {inject, OnDestroy, Service} from '@angular/core';
// import {LogLevel} from '@codingame/monaco-vscode-api';
// import {LanguageClientWrapper} from 'monaco-languageclient/lcwrapper';
// import {MonacoVscodeApiWrapper} from 'monaco-languageclient/vscodeApiWrapper';
// import EditorWorker from '@codingame/monaco-vscode-editor-api/esm/vs/editor/editor.worker?worker';
// import {LanguageId, registerCalculatorLanguage} from '../app.config.monaco';
// import * as vscode from 'vscode';
// import {FileSystem} from './file-system';
//
// window.MonacoEnvironment = {
//   getWorker(_moduleId, _label) {
//     return new EditorWorker();
//   }
// };
//
// @Service()
// export class LspService implements OnDestroy {
//   private readonly fs: FileSystem = inject(FileSystem);
//   private vscode?: MonacoVscodeApiWrapper;
//   private calculator?: LanguageClientWrapper;
//   private initialization?: Promise<void>;
//   private intervalHandle?: number;
//
//   public async initialize(): Promise<void> {
//     return this.initialization ??= this.runInitializers();
//   }
//
//   private async runInitializers(): Promise<void> {
//     this.fs.initialize();
//     await this.initializeVsCodeWrapper();
//     await this.initializeClcClient();
//   }
//
//   private async initializeVsCodeWrapper(): Promise<void> {
//     this.vscode = new MonacoVscodeApiWrapper({
//       $type: 'classic',
//       viewsConfig: {$type: 'EditorService'},
//       logLevel: LogLevel.Debug,
//     });
//
//     await this.vscode.start();
//   }
//
//   private async initializeClcClient(): Promise<void> {
//     registerCalculatorLanguage();
//
//     this.calculator = new LanguageClientWrapper({
//       languageId: LanguageId.Calculator,
//       connection: {
//         options: {
//           $type: 'WebSocketUrl',
//           url: 'wss://localhost:5001/lsp/clc',
//         }
//       },
//       clientOptions: {
//         documentSelector: [{language: LanguageId.Calculator}],
//         workspaceFolder: {
//           index: 0,
//           name: 'workspace',
//           uri: vscode.Uri.parse(`file:///workspace`),
//         },
//       }
//     });
//
//     await this.calculator.start();
//
//     this.monitorConnection();
//   }
//
//   private monitorConnection(): void {
//     this.intervalHandle = setInterval(async () => {
//       try {
//         if (this.calculator === undefined || this.calculator.getLanguageClient() === undefined) {
//           console.log("LanguageClient was not initialized...");
//           return;
//         }
//
//         if (!this.calculator.getLanguageClient()!.isRunning()) {
//           await this.calculator.start();
//         }
//       } catch (e) {
//         console.error(e);
//       }
//     }, 5000);
//   }
//
//   ngOnDestroy(): void {
//     if (this.intervalHandle) {
//       clearInterval(this.intervalHandle);
//     }
//     this.calculator?.dispose();
//     this.vscode?.dispose();
//   }
// }
