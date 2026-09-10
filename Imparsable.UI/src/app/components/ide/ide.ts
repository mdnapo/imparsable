import {Component, EventEmitter, Input, Output, signal, WritableSignal} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatTabsModule} from '@angular/material/tabs';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MonacoEditorModule} from 'ngx-monaco-editor-v2';
import {IdeWidget} from '../../app.models';
import {NgComponentOutlet} from '@angular/common';
import {MatBadge} from '@angular/material/badge';
import type {editor} from 'monaco-editor';
import {IdeFile} from '../../app.filesystem';

@Component({
  selector: 'app-ide',
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatToolbarModule,
    MonacoEditorModule,
    NgComponentOutlet,
    MatBadge,
  ],
  templateUrl: './ide.html',
  styleUrl: './ide.scss',
})
export class Ide {
  @Input() sideViews: IdeWidget[] = [];
  protected sideView: WritableSignal<IdeWidget | undefined> = signal(undefined);
  protected sideViewWidth = 250;
  protected readonly sideViewMinWidth = 160;
  protected readonly sideViewMaxWidth = 800;

  @Input() bottomViews: IdeWidget[] = [];
  protected bottomView: WritableSignal<IdeWidget | undefined> = signal(undefined);
  protected bottomViewHeight = 300;
  protected readonly bottomViewMinHeight = 100;
  protected readonly bottomViewMaxHeight = 600;

  @Input() file: IdeFile | null = null;
  @Output() onInit: EventEmitter<editor.IStandaloneCodeEditor> = new EventEmitter<editor.IStandaloneCodeEditor>();
  @Output() onCloseFile: EventEmitter<IdeFile> = new EventEmitter<IdeFile>();

  public setSideView(view: IdeWidget): void {
    if (this.sideView()?.id === view.id) {
      this.toggleSideView(view);
    } else {
      this.sideView.set(view);
    }
  }

  protected toggleSideView(view: IdeWidget): void {
    this.sideView.update(current => current?.id === view.id ? undefined : view);
  }

  public setBottomView(view: IdeWidget): void {
    this.bottomView.set(view);
  }

  protected toggleBottomView(view: IdeWidget): void {
    this.bottomView.update(current => current?.id === view.id ? undefined : view);
  }

  protected startSideViewResize(event: PointerEvent, element: HTMLElement): void {
    const target = event.currentTarget as HTMLElement;
    target.setPointerCapture(event.pointerId);

    const startX = event.clientX;
    const startWidth = this.sideViewWidth;
    let nextWidth = startWidth;
    let frame = 0;

    const move = (event: PointerEvent): void => {
      nextWidth = Math.max(
        this.sideViewMinWidth,
        Math.min(this.sideViewMaxWidth, startWidth + event.clientX - startX)
      );

      if (frame !== 0)
        return;

      frame = requestAnimationFrame(() => {
        element.style.setProperty('--side-view-width', `${nextWidth}px`);
        frame = 0;
      });
    };

    const stop = (): void => {
      if (frame !== 0)
        cancelAnimationFrame(frame);

      this.sideViewWidth = nextWidth;

      target.releasePointerCapture(event.pointerId);

      target.removeEventListener('pointermove', move);
      target.removeEventListener('pointerup', stop);
    };

    target.addEventListener('pointermove', move);
    target.addEventListener('pointerup', stop);
  }

  protected startBottomViewResize(event: PointerEvent, element: HTMLElement): void {
    const target = event.currentTarget as HTMLElement;
    target.setPointerCapture(event.pointerId);

    const startY = event.clientY;
    const startHeight = this.bottomViewHeight;
    let nextHeight = startHeight;
    let frame = 0;

    const move = (event: PointerEvent): void => {
      nextHeight = Math.max(
        this.bottomViewMinHeight,
        Math.min(this.bottomViewMaxHeight, startHeight + startY - event.clientY)
      );

      if (frame !== 0)
        return;

      frame = requestAnimationFrame(() => {
        element.style.setProperty('--bottom-view-height', `${nextHeight}px`);
        frame = 0;
      });
    };

    const stop = (): void => {
      if (frame !== 0)
        cancelAnimationFrame(frame);

      this.bottomViewHeight = nextHeight;

      target.releasePointerCapture(event.pointerId);

      target.removeEventListener('pointermove', move);
      target.removeEventListener('pointerup', stop);
    };

    target.addEventListener('pointermove', move);
    target.addEventListener('pointerup', stop);
  }

  protected closeFile($event: PointerEvent, file: IdeFile): void {
    $event.stopPropagation();
    this.onCloseFile.emit(file);
  }
}
