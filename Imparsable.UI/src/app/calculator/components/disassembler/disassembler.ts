import {Component, inject} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {MatIcon} from "@angular/material/icon";
import {MatIconButton} from "@angular/material/button";
import {ToolWindow} from "@shared/components/tool-window/tool-window";
import {FileService} from '@calculator/services/file-service';
import {DisassemblerService} from '@calculator/services/disassembler-service';

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
  protected readonly context: FileService = inject(FileService);
  protected readonly service: DisassemblerService = inject(DisassemblerService);
}
