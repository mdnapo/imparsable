import {Component, inject} from '@angular/core';
import {AsyncPipe} from '@angular/common';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {ToolWindow} from '@shared/components/tool-window/tool-window';
import {Context} from '@calculator/services/context';

@Component({
  selector: 'app-runner',
  imports: [
    AsyncPipe,
    MatIcon,
    MatIconButton,
    ToolWindow
  ],
  templateUrl: './runner.html',
  styleUrl: './runner.scss',
})
export class Runner {
  protected readonly context: Context = inject(Context);

}
