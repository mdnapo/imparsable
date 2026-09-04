import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {MatIcon} from "@angular/material/icon";
import {MatIconButton} from "@angular/material/button";
import {CalculatorContext} from '../../services/calculator-context';
import {IdeDirectory, IdeNode} from '../../app.filesystem';
import {MatTree, MatTreeNode, MatTreeNodeDef, MatTreeNodePadding, MatTreeNodeToggle} from '@angular/material/tree';

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
export class CalculatorExplorer {
  protected readonly context: CalculatorContext = inject(CalculatorContext);

  protected readonly childrenAccessor =
    (node: IdeNode): IdeNode[] => node instanceof IdeDirectory ? node.children : [];

  protected readonly hasChild =
    (_: number, node: IdeNode): boolean => node instanceof IdeDirectory;
}
