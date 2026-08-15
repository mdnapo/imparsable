import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  inject,
  Input,
  OnDestroy,
  ViewChild
} from '@angular/core';
import {EditorApp} from 'monaco-languageclient/editorApp';
import * as monaco from '@codingame/monaco-vscode-editor-api';
import {LspService} from '../../services/lsp-service';
import * as vscode from 'vscode';

export interface SourceFile {
  name: string;
  content: string;
  languageId: string;
}

@Component({
  selector: 'app-code-editor',
  imports: [],
  changeDetection: ChangeDetectionStrategy.Eager,
  templateUrl: './code-editor.html',
  styleUrl: './code-editor.scss',
})
export class CodeEditor implements AfterViewInit, OnDestroy {
  private readonly languageServer: LspService = inject(LspService);
  @ViewChild('editor', {static: true})
  private editorContainer!: ElementRef<HTMLDivElement>;
  private editor?: monaco.editor.IStandaloneCodeEditor;
  @Input() file!: SourceFile;

  public async ngAfterViewInit(): Promise<void> {
    await this.languageServer.initialize();

    const editorApp = new EditorApp({
      editorOptions: {
        automaticLayout: true,
        theme: 'vs',
      },
    });
    await editorApp.start(this.editorContainer.nativeElement);
    this.editor = editorApp.getEditor();

    const uri = monaco.Uri.parse(`file:///workspace/${this.file.name}`);
    const model = monaco.editor.createModel(this.file.content, this.file.languageId, uri);
    monaco.editor.setModelLanguage(model, this.file.languageId);
    this.editor?.setModel(model);

    const document = await vscode.workspace.openTextDocument(uri);
  }

  public ngOnDestroy(): void {
    this.editor?.dispose();
  }
}
