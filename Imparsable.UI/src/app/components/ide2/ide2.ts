import {Component, Input} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatTabsModule} from '@angular/material/tabs';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MonacoEditorModule} from 'ngx-monaco-editor-v2';
import {IdeWidget} from '../../app.models';
import {NgComponentOutlet} from '@angular/common';

// type T1View = 'explorer' | 'search' | 'outline';
// type T2View = 'problems' | 'output' | 'terminal';


@Component({
  selector: 'app-ide2',
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatToolbarModule,
    MonacoEditorModule,
    NgComponentOutlet,
  ],
  templateUrl: './ide2.html',
  styleUrl: './ide2.scss',
})
export class Ide2 {
  public code = [
    'let answer = 40 + 2;',
    '',
    'print(answer);',
  ].join('\n');

  public editorOptions = {
    automaticLayout: true,
    scrollBeyondLastLine: false,
  };

  @Input()
  side: IdeWidget[] = [];
  // protected _side?: IdeWidget;

  @Input()
  bottom: IdeWidget[] = [];
  // protected _bottom?: IdeWidget;

  // public t1View?: string = 'explorer';
  // public t2View?: string = 'output';
  @Input()
  public t1View?: IdeWidget;

  @Input()
  public t2View?: IdeWidget;

  public t1Width = 240;
  public t2Height = 200;

  public toggleT1View(view: IdeWidget): void {
    this.t1View = this.t1View?.id === view.id
      ? undefined
      : view;
  }

  public toggleT2View(view: IdeWidget): void {
    this.t2View = this.t2View?.id === view.id
      ? undefined
      : view;
  }

  protected startT1Resize(event: PointerEvent, element: HTMLElement): void {
    const target = event.currentTarget as HTMLElement;
    target.setPointerCapture(event.pointerId);

    const startX = event.clientX;
    const startWidth = this.t1Width;

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
        element.style.setProperty('--t1-width', `${nextWidth}px`);
        frame = 0;
      });
    };

    const stop = (): void => {
      if (frame !== 0)
        cancelAnimationFrame(frame);

      this.t1Width = nextWidth;

      target.releasePointerCapture(event.pointerId);

      target.removeEventListener('pointermove', move);
      target.removeEventListener('pointerup', stop);
    };

    target.addEventListener('pointermove', move);
    target.addEventListener('pointerup', stop);
  }

  protected startT2Resize(event: PointerEvent, element: HTMLElement): void {
    const target = event.currentTarget as HTMLElement;
    target.setPointerCapture(event.pointerId);

    const startY = event.clientY;
    const startHeight = this.t2Height;

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
        element.style.setProperty('--t2-height', `${nextHeight}px`);
        frame = 0;
      });
    };

    const stop = (): void => {
      if (frame !== 0)
        cancelAnimationFrame(frame);

      this.t2Height = nextHeight;

      target.releasePointerCapture(event.pointerId);

      target.removeEventListener('pointermove', move);
      target.removeEventListener('pointerup', stop);
    };

    target.addEventListener('pointermove', move);
    target.addEventListener('pointerup', stop);
  }
}
