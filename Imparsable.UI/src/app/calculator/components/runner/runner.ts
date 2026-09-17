import {Component, inject} from '@angular/core';
import {AsyncPipe} from '@angular/common';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {ToolWindow} from '@shared/components/tool-window/tool-window';
import {FileService} from '@calculator/services/file-service';
import {RunnerService} from '@calculator/services/runner-service';

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
  protected readonly context: FileService = inject(FileService);
  protected readonly service: RunnerService = inject(RunnerService);
}
