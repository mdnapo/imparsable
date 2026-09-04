import type {editor} from 'monaco-editor';
import type {Dirent} from '@zenfs/core';
import {DataSource} from '@angular/cdk/table';
import {CollectionViewer} from "@angular/cdk/collections";
import {BehaviorSubject, Observable, Observer, Subject, Subscription} from "rxjs";

export abstract class IdeNode {
  protected constructor(readonly name: string, readonly path: string) {
  }
}

export class IdeDirectory extends IdeNode {
  readonly children: IdeNode[] = [];

  constructor(name: string, path: string) {
    super(name, path);
  }
}

export class IdeFile extends IdeNode {
  model?: editor.ITextModel;

  constructor(name: string, path: string) {
    super(name, path);
  }
}

export class IdeTree extends BehaviorSubject<readonly IdeNode[]> implements DataSource<IdeNode> {
  private root!: IdeDirectory;

  constructor(entries: Dirent[] = [], rootPath: string = '') {
    super([]);
    this.load(entries, rootPath);
  }

  load(entries: Dirent[] = [], rootPath: string = '') {
    this.root = new IdeDirectory(this.getName(rootPath), rootPath);
    this.build(entries);
    this.next([this.root]);
  }

  connect(collectionViewer: CollectionViewer): Observable<readonly IdeNode[]> {
    return this;
  }

  disconnect(collectionViewer: CollectionViewer): void {
  }

  private build(entries: Dirent[]): void {
    const directories = new Map<string, IdeDirectory>();
    directories.set(this.root.path, this.root);

    for (const entry of entries) {
      const parentPath = this.getParentPath(entry);
      this.ensureDirectory(parentPath, directories);
    }

    for (const entry of entries) {
      const parentPath = this.getParentPath(entry);
      const parent = directories.get(parentPath);

      if (!parent)
        throw new Error(`Parent directory not found: ${parentPath}`);

      const path = this.join(parentPath, entry.name);

      if (entry.isDirectory()) {
        const directory = directories.get(path);

        if (directory && !parent.children.includes(directory))
          parent.children.push(directory);

        continue;
      }

      parent.children.push(new IdeFile(entry.name, path));
    }
  }

  private ensureDirectory(path: string, directories: Map<string, IdeDirectory>): IdeDirectory {
    const existing = directories.get(path);

    if (existing) {
      return existing;
    }

    const parentPath = this.getDirectoryParent(path);
    const parent = this.ensureDirectory(parentPath, directories);
    const directory = new IdeDirectory(this.getName(path), path);

    directories.set(path, directory);
    parent.children.push(directory);

    return directory;
  }

  private getParentPath(entry: Dirent): string {
    return (entry as Dirent & { _parentPath: string })._parentPath;
  }

  private getDirectoryParent(path: string): string {
    const index = path.lastIndexOf('/');

    if (index <= 0) {
      return '/';
    }

    return path.substring(0, index);
  }

  private getName(path: string): string {
    const index = path.lastIndexOf('/');
    return path.substring(index + 1);
  }

  private join(parent: string, name: string): string {
    return parent === '/'
      ? `/${name}`
      : `${parent}/${name}`;
  }
}
