import {AfterViewInit, Component, ElementRef, inject, OnDestroy, ViewChild} from '@angular/core';
import {EditorApp} from 'monaco-languageclient/editorApp';
import * as monaco from '@codingame/monaco-vscode-editor-api';
import {LspService} from '../../services/lsp-service';
import * as vscode from 'vscode';

const code: string = `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" . ': ' . area;
print 1 + "2";
`;

@Component({
  selector: 'app-code-editor',
  imports: [],
  templateUrl: './code-editor.html',
  styleUrl: './code-editor.scss',
})
export class CodeEditor implements AfterViewInit, OnDestroy {
  private readonly languageServer: LspService = inject(LspService);
  @ViewChild('editor', {static: true})
  private editorContainer!: ElementRef<HTMLDivElement>;
  private editor?: monaco.editor.IStandaloneCodeEditor;

  public async ngAfterViewInit(): Promise<void> {
    await this.languageServer.initialize();

    const editorApp = new EditorApp({
      editorOptions: {
        automaticLayout: true,
        theme: 'vs'
      }
    });
    await editorApp.start(this.editorContainer.nativeElement);
    this.editor = editorApp.getEditor();

    const uri = monaco.Uri.parse('file:///workspace/test.clc');
    const model = monaco.editor.createModel(code, 'clc', uri);
    monaco.editor.setModelLanguage(model, 'clc');
    this.editor?.setModel(model);

    const document = await vscode.workspace.openTextDocument(uri);
  }

  public ngOnDestroy(): void {
    this.editor?.dispose();
  }
}
