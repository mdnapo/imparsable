import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef, EventEmitter,
  inject,
  Input,
  OnDestroy, Output,
  ViewChild
} from '@angular/core';
import {EditorApp} from 'monaco-languageclient/editorApp';
import * as monaco from '@codingame/monaco-vscode-editor-api';
import {LspService} from '../../services/lsp-service';
import * as vscode from 'vscode';
import {MatToolbar} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTab, MatTabGroup, MatTabLabel} from '@angular/material/tabs';
import {BehaviorSubject} from 'rxjs';
import {AsyncPipe} from '@angular/common';
import {Diagnostic, DiagnosticSeverity, SourceFile, StdOutput} from '../../app.models';
import {MatBadgeModule} from '@angular/material/badge';

@Component({
  selector: 'app-code-editor',
  imports: [
    MatToolbar,
    MatIconButton,
    MatIcon,
    MatTabGroup,
    MatTab,
    AsyncPipe,
    MatBadgeModule,
    MatTabLabel,
  ],
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
  @Output() onExecute: EventEmitter<any> = new EventEmitter();
  output: BehaviorSubject<StdOutput[]> = new BehaviorSubject([] as StdOutput[]);
  diagnostics: BehaviorSubject<Diagnostic[]> = new BehaviorSubject([] as Diagnostic[]);

  protected selectedTab: number = 0;

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

    await vscode.workspace.openTextDocument(uri);
  }

  emitExecute(): void {
    let code = this.editor!.getModel()!.getValue();
    this.output.next([]);
    this.diagnostics.next([]);
    this.onExecute.emit(code);

    if (this.diagnostics.value.length > 0) {
      this.selectedTab = 1;
    } else {
      this.selectedTab = 0;
    }
  }

  public ngOnDestroy(): void {
    this.editor?.dispose();
  }

  public onOutput(output: string): void {
    this.output.value.push({id: this.output.value.length, text: output});
    this.output.next(this.output.value);
  }

  public onDiagnosticPublished(diagnostic: Diagnostic): void {
    this.diagnostics.value.push(diagnostic);
    this.diagnostics.next(this.diagnostics.value.sort((l, r) => l.marker.column - r.marker.column));
  }

  protected formatDiagnostic(line: Diagnostic): string {
    return `[${DiagnosticSeverity[line.severity]}][line: ${line.marker.line}, col: ${line.marker.column}] ${line.message}`;
  }
}
