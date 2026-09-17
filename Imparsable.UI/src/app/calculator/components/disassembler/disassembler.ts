import {Component, inject} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {MatIcon} from "@angular/material/icon";
import {MatIconButton} from "@angular/material/button";
import {ToolWindow} from "@shared/components/tool-window/tool-window";
import {Context} from '@calculator/services/context';

@Component({
  selector: 'app-disassembler',
    imports: [
        AsyncPipe,
        MatIcon,
        MatIconButton,
        ToolWindow
    ],
  templateUrl: './disassembler.html',
  styleUrl: './disassembler.scss',
})
export class Disassembler {
  protected readonly context: Context = inject(Context);
}
