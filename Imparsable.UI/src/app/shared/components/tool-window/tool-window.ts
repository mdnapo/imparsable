import {Component, Input} from '@angular/core';
import {MatToolbar} from '@angular/material/toolbar';

@Component({
  selector: 'app-tool-window',
  imports: [
    MatToolbar
  ],
  templateUrl: './tool-window.html',
  styleUrl: './tool-window.scss'
})
export class ToolWindow {
  @Input({required: true}) label!: string;
}
