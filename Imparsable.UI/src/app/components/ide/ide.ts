import {Component} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatTabsModule} from '@angular/material/tabs';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MonacoEditorModule} from 'ngx-monaco-editor-v2';

@Component({
  selector: 'app-ide',
  imports: [
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatToolbarModule,
    MonacoEditorModule,
  ],
  templateUrl: './ide.html',
  styleUrl: './ide.scss',
})
export class Ide {
  public code = [
    'let answer = 40 + 2;',
    '',
    'print(answer);',
  ].join('\n');

  public editorOptions = {
    automaticLayout: true,
    minimap: {
      enabled: true,
    },
    scrollBeyondLastLine: false,
    fontSize: 14,
  };

  public panelHeight = 200;

  public startPanelResize(
    event: PointerEvent,
    element: HTMLElement
  ): void {
    const target = event.currentTarget as HTMLElement;

    target.setPointerCapture(event.pointerId);

    const startY = event.clientY;
    const startHeight = this.panelHeight;

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
        element.style.setProperty('--panel-height', `${nextHeight}px`);
        frame = 0;
      });
    };

    const stop = (): void => {
      if (frame !== 0)
        cancelAnimationFrame(frame);

      this.panelHeight = nextHeight;

      target.releasePointerCapture(event.pointerId);

      target.removeEventListener('pointermove', move);
      target.removeEventListener('pointerup', stop);
    };

    target.addEventListener('pointermove', move);
    target.addEventListener('pointerup', stop);
  }
}
