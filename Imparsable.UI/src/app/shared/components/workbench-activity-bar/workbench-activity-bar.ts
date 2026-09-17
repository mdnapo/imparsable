import {Component, EventEmitter, Input, Output} from '@angular/core';
import {MatBadge} from '@angular/material/badge';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {WorkbenchView} from '@shared/models/workbench-view';

@Component({
  selector: 'app-workbench-activity-bar',
  imports: [
    MatBadge,
    MatIcon,
    MatIconButton
  ],
  templateUrl: './workbench-activity-bar.html',
  styleUrl: './workbench-activity-bar.scss'
})
export class WorkbenchActivityBar {
  @Input() primary: WorkbenchView[] = [];
  @Input() secondary: WorkbenchView[] = [];
  @Input() activePrimary?: WorkbenchView;
  @Input() activeSecondary?: WorkbenchView;

  @Output() primarySelected: EventEmitter<WorkbenchView> = new EventEmitter<WorkbenchView>();
  @Output() secondarySelected: EventEmitter<WorkbenchView> = new EventEmitter<WorkbenchView>();
}
