import {AfterViewInit, ChangeDetectionStrategy, Component, inject, ViewChild} from '@angular/core';
import {MatIcon} from "@angular/material/icon";
import {MatIconButton} from "@angular/material/button";
import {CalculatorContext} from '../../services/calculator-context';
import {IdeDirectory, IdeFile, IdeNode} from '../../app.filesystem';
import {MatTree, MatTreeNode, MatTreeNodeDef, MatTreeNodePadding, MatTreeNodeToggle} from '@angular/material/tree';
import {AsyncPipe} from '@angular/common';
import {MatToolbar} from '@angular/material/toolbar';

@Component({
  selector: 'app-calculator-explorer',
  imports: [
    MatIcon,
    MatIconButton,
    MatTree,
    MatTreeNode,
    MatTreeNodeDef,
    MatTreeNodeToggle,
    MatTreeNodePadding,
    AsyncPipe,
    MatToolbar,
  ],
  templateUrl: './calculator-explorer.html',
  styleUrl: './calculator-explorer.scss',
  changeDetection: ChangeDetectionStrategy.Eager
})
export class CalculatorExplorer implements AfterViewInit {
  protected readonly context: CalculatorContext = inject(CalculatorContext);
  @ViewChild(MatTree) protected tree!: MatTree<IdeNode>;

  protected readonly childrenAccessor =
    (node: IdeNode): IdeNode[] => node instanceof IdeDirectory ? node.children : [];

  protected readonly hasChild =
    (_: number, node: IdeNode): boolean => node instanceof IdeDirectory;

  ngAfterViewInit(): void {
    const files = this.context.files();

    if (files.value.length === 0) return;

    for (const node of files.value) {
      if (node instanceof IdeDirectory) {
        node.traverse(directory => {
          if (directory.expanded()) {
            this.tree.expand(directory);
          }
        });
      }
    }
  }

  protected openFile($event: MouseEvent, node: IdeFile): void {
    $event.stopPropagation();
    this.context.openFile(node);
  }

  protected onExpandedChange(node: IdeDirectory, $event: boolean): void {
    node.setExpanded($event);
  }
}
