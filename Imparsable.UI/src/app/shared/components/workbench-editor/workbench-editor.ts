import {Component, EventEmitter, Input, Output} from '@angular/core';
import {MatIcon} from '@angular/material/icon';
import {MatTab, MatTabGroup, MatTabLabel} from '@angular/material/tabs';
import {MonacoEditorModule} from 'ngx-monaco-editor-v2';
import type {editor} from 'monaco-editor';
import {IdeFile} from '@shared/models/filesystem';

@Component({
  selector: 'app-workbench-editor',
  imports: [
    MatIcon,
    MatTab,
    MatTabGroup,
    MatTabLabel,
    MonacoEditorModule
  ],
  templateUrl: './workbench-editor.html',
  styleUrl: './workbench-editor.scss'
})
export class WorkbenchEditor {
  @Input() file: IdeFile | null = null;

  @Output() initialized: EventEmitter<editor.IStandaloneCodeEditor> =
    new EventEmitter<editor.IStandaloneCodeEditor>();

  @Output() closed: EventEmitter<IdeFile> =
    new EventEmitter<IdeFile>();

  protected closeFile(event: PointerEvent, file: IdeFile): void {
    event.stopPropagation();
    this.closed.emit(file);
  }
}
