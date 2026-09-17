import {Component, Input, signal, WritableSignal} from '@angular/core';
import {NgComponentOutlet} from '@angular/common';
import {WorkbenchActivityBar} from '@shared/components/workbench-activity-bar/workbench-activity-bar';
import {WorkbenchResize} from '@shared/directives/workbench-resize';
import {WorkbenchView} from '@shared/models/workbench-view';

@Component({
  selector: 'app-workbench',
  imports: [
    NgComponentOutlet,
    WorkbenchActivityBar,
    WorkbenchResize,
    WorkbenchActivityBar
  ],
  templateUrl: './workbench.html',
  styleUrl: './workbench.scss'
})
export class Workbench {
  @Input() leftViews: WorkbenchView[] = [];
  @Input() rightViews: WorkbenchView[] = [];
  @Input() bottomViews: WorkbenchView[] = [];

  protected readonly leftView: WritableSignal<WorkbenchView | undefined> = signal(undefined);
  protected readonly rightView: WritableSignal<WorkbenchView | undefined> = signal(undefined);
  protected readonly bottomView: WritableSignal<WorkbenchView | undefined> = signal(undefined);

  protected leftViewWidth = 250;
  protected rightViewWidth = 500;
  protected bottomViewHeight = 300;

  protected readonly leftViewMinWidth = 160;
  protected readonly leftViewMaxWidth = 800;
  protected readonly rightViewMinWidth = 160;
  protected readonly rightViewMaxWidth = 800;
  protected readonly bottomViewMinHeight = 100;
  protected readonly bottomViewMaxHeight = 600;

  public setLeftView(view: WorkbenchView, toggle: boolean = false): void {
    if (this.leftView()?.id === view.id) {
      if (!toggle) return;
      this.toggleLeftView(view);
    } else {
      this.leftView.set(view);
    }
  }

  public setRightView(view: WorkbenchView, toggle: boolean = false): void {
    if (this.rightView()?.id === view.id) {
      if (!toggle) return;
      this.toggleRightView(view);
    } else {
      this.rightView.set(view);
    }
  }

  public setBottomView(view: WorkbenchView, toggle: boolean = false): void {
    if (this.bottomView()?.id === view.id) {
      if (!toggle) return;
      this.toggleBottomView(view);
    } else {
      this.bottomView.set(view);
    }
  }

  protected toggleLeftView(view: WorkbenchView): void {
    this.leftView.update(current =>
      current?.id === view.id ? undefined : view
    );
  }

  protected toggleRightView(view: WorkbenchView): void {
    this.rightView.update(current =>
      current?.id === view.id ? undefined : view
    );
  }

  protected toggleBottomView(view: WorkbenchView): void {
    this.bottomView.update(current =>
      current?.id === view.id ? undefined : view
    );
  }
}
