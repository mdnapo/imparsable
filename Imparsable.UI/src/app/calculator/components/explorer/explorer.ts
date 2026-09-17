import {AfterViewInit, Component, inject, ViewChild} from '@angular/core';
import {MatTree, MatTreeNode, MatTreeNodeDef, MatTreeNodePadding, MatTreeNodeToggle} from '@angular/material/tree';
import {MatIcon} from '@angular/material/icon';
import {MatIconButton} from '@angular/material/button';
import {ToolWindow} from '@shared/components/tool-window/tool-window';
import {Context} from '@calculator/services/context';
import {IdeDirectory, IdeFile, IdeNode} from '@shared/models/filesystem';

@Component({
  selector: 'app-explorer',
  imports: [
    MatIcon,
    MatIconButton,
    MatTree,
    MatTreeNode,
    MatTreeNodeDef,
    MatTreeNodePadding,
    MatTreeNodeToggle,
    ToolWindow
  ],
  templateUrl: './explorer.html',
  styleUrl: './explorer.scss',
})
export class Explorer  implements AfterViewInit {
  protected readonly context: Context = inject(Context);

  @ViewChild(MatTree)
  protected tree!: MatTree<IdeNode>;

  protected readonly childrenAccessor =
    (node: IdeNode): IdeNode[] => node instanceof IdeDirectory ? node.children : [];

  protected readonly hasChild =
    (_: number, node: IdeNode): boolean => node instanceof IdeDirectory;

  ngAfterViewInit(): void {
    const files = this.context.files();

    if (files.value.length === 0)
      return;

    for (const node of files.value) {
      if (node instanceof IdeDirectory) {
        node.traverse(directory => {
          if (directory.expanded())
            this.tree.expand(directory);
        });
      }
    }
  }

  protected openFile(event: MouseEvent, node: IdeFile): void {
    event.stopPropagation();
    this.context.openFile(node);
  }

  protected onExpandedChange(node: IdeDirectory, event: boolean): void {
    node.setExpanded(event);
  }
}
