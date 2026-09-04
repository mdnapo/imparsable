import {ChangeDetectionStrategy, Component, inject, OnInit} from '@angular/core';
import {MatIcon} from "@angular/material/icon";
import {MatIconButton} from "@angular/material/button";
import {CalculatorContext} from '../../services/calculator-context';
import {IdeDirectory, IdeNode, IdeTree} from '../../app.filesystem';
import {MatTree, MatTreeNode, MatTreeNodeDef, MatTreeNodePadding, MatTreeNodeToggle} from '@angular/material/tree';
import {readCalculatorWorkspace} from '../../app.config.filesystem';

@Component({
  selector: 'app-calculator-explorer',
  imports: [
    MatIcon,
    MatIconButton,
    MatTree,
    MatTreeNode,
    MatTreeNodeDef,
    MatTreeNodeToggle,
    MatTreeNodePadding
  ],
  templateUrl: './calculator-explorer.html',
  styleUrl: './calculator-explorer.scss',
  changeDetection: ChangeDetectionStrategy.Eager
})
export class CalculatorExplorer implements OnInit {
  protected readonly context: CalculatorContext = inject(CalculatorContext);
  protected dataSource!: IdeTree;

  protected readonly childrenAccessor =
    (node: IdeNode): IdeNode[] => node instanceof IdeDirectory
      ? node.children
      : [];

  protected readonly hasChild =
    (_: number, node: IdeNode): boolean => node instanceof IdeDirectory;

  async ngOnInit(): Promise<void> {
    this.dataSource = new IdeTree(await readCalculatorWorkspace(), '/workspace');
    console.log(this.dataSource);
  }
}
