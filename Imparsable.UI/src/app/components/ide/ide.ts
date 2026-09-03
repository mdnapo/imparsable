import {Component, Input} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatTabsModule} from '@angular/material/tabs';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MonacoEditorModule} from 'ngx-monaco-editor-v2';
import {IdeWidget} from '../../app.models';
import {NgComponentOutlet} from '@angular/common';

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
  ],
  templateUrl: './ide.html',
  styleUrl: './ide.scss',
})
export class Ide {
  @Input() side: IdeWidget[] = [];
  @Input() sideView?: IdeWidget;
  protected sideViewWidth = 240;

  @Input() bottom: IdeWidget[] = [];
  @Input() bottomView?: IdeWidget;
  protected bottomViewHeight = 200;

  protected code = [
    'let answer = 40 + 2;',
    '',
    'print(answer);',
  ].join('\n');

  protected toggleSideView(view: IdeWidget): void {
    this.sideView = this.sideView?.id === view.id
      ? undefined
      : view;
  }

  protected toggleBottomView(view: IdeWidget): void {
    this.bottomView = this.bottomView?.id === view.id
      ? undefined
      : view;
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
        160,
        Math.min(600, startWidth + event.clientX - startX)
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
        100,
        Math.min(600, startHeight + startY - event.clientY)
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
}
