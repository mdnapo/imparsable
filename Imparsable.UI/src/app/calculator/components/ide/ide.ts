import {AfterViewInit, Component, inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {Workbench} from "@shared/components/workbench/workbench";
import {WorkbenchEditor} from "@shared/components/workbench-editor/workbench-editor";
import {Subscription} from 'rxjs';
import type {editor, IDisposable} from 'monaco-editor';
import {WorkbenchView} from '@shared/models/workbench-view';
import {Problems} from '@calculator/components/problems/problems';
import {Disassembler} from '@calculator/components/disassembler/disassembler';
import {Runner} from '@calculator/components/runner/runner';
import {Memory} from '@calculator/components/memory/memory';
import {Grammar} from '@calculator/components/grammar/grammar';
import {Explorer} from '@calculator/components/explorer/explorer';
import {Context} from '@calculator/services/context';
import {LanguageServer} from '@shared/services/language-server';
import {LanguageId} from '@config/monaco';

@Component({
  selector: 'app-ide',
  imports: [
    AsyncPipe,
    Workbench,
    WorkbenchEditor
  ],
  templateUrl: './ide.html',
  styleUrl: './ide.scss',
})
export class Ide implements OnInit, AfterViewInit, OnDestroy {
  protected readonly context: Context = inject(Context);
  private readonly languageServer: LanguageServer = inject(LanguageServer);

  private editor?: editor.IStandaloneCodeEditor;
  private readonly subscriptions: Subscription = new Subscription();
  private explorerSubscription?: IDisposable;
  private grammarSubscription?: IDisposable;
  private memorySubscription?: IDisposable;
  private executeSubscription?: IDisposable;
  private disassembleSubscription?: IDisposable;
  private problemsSubscription?: IDisposable;
  private saveSubscription?: IDisposable;

  @ViewChild(Workbench)
  private workbench!: Workbench;

  protected readonly leftViews: WorkbenchView[] = [
    {
      id: 'explorer',
      title: 'Explorer (ctrl + alt + f)',
      icon: 'folder',
      view: Explorer
    },
    {
      id: 'grammar',
      title: 'Grammar (ctrl + alt + g)',
      icon: 'regular_expression',
      view: Grammar
    }
  ];

  protected readonly rightViews: WorkbenchView[] = [
    {
      id: 'memory',
      title: 'Memory (ctrl + alt + m)',
      icon: 'monitor_heart',
      view: Memory
    }
  ];

  protected readonly bottomViews: WorkbenchView[] = [
    {
      id: 'runner',
      title: 'Execute (ctrl + shift + x)',
      icon: 'terminal_2',
      view: Runner
    },
    {
      id: 'disassembler',
      title: 'Disassemble (ctrl + shift + d)',
      icon: 'data_array',
      view: Disassembler
    },
    {
      id: 'problems',
      title: 'Problems (ctrl + shift + q)',
      icon: 'error',
      view: Problems,
      badge: () => this.context.errors()
    }
  ];

  async init(editor: editor.IStandaloneCodeEditor): Promise<void> {
    this.editor = editor;

    this.updateModel();

    await this.languageServer.connect(LanguageId.Calculator, '/lsp/clc');

    this.explorerSubscription = this.editor.addAction({
      id: 'open-explorer',
      label: 'Open Explorer',
      keybindings: [
        window.monaco.KeyMod.CtrlCmd |
        window.monaco.KeyMod.Alt |
        window.monaco.KeyCode.KeyF
      ],
      run: () => this.workbench.setLeftView(this.leftViews[0])
    });

    this.grammarSubscription = this.editor.addAction({
      id: 'open-grammar',
      label: 'Open Grammar',
      keybindings: [
        window.monaco.KeyMod.CtrlCmd |
        window.monaco.KeyMod.Alt |
        window.monaco.KeyCode.KeyG
      ],
      run: () => this.workbench.setLeftView(this.leftViews[1])
    });

    this.memorySubscription = this.editor.addAction({
      id: 'open-memory',
      label: 'Open Memory',
      keybindings: [
        window.monaco.KeyMod.CtrlCmd |
        window.monaco.KeyMod.Alt |
        window.monaco.KeyCode.KeyM
      ],
      run: () => this.workbench.setRightView(this.rightViews[0])
    });

    this.executeSubscription = this.editor.addAction({
      id: 'execute-file',
      label: 'Execute File',
      keybindings: [
        window.monaco.KeyMod.CtrlCmd |
        window.monaco.KeyMod.Shift |
        window.monaco.KeyCode.KeyX
      ],
      run: () => this.context.execute()
    });

    this.problemsSubscription = this.editor.addAction({
      id: 'open-problems',
      label: 'Open Problems',
      keybindings: [
        window.monaco.KeyMod.CtrlCmd |
        window.monaco.KeyMod.Shift |
        window.monaco.KeyCode.KeyQ
      ],
      run: () => this.workbench.setBottomView(this.bottomViews[2])
    });

    this.disassembleSubscription = this.editor.addAction({
      id: 'disassemble-file',
      label: 'Disassemble File',
      keybindings: [
        window.monaco.KeyMod.CtrlCmd |
        window.monaco.KeyMod.Shift |
        window.monaco.KeyCode.KeyD
      ],
      run: () => this.context.disassemble()
    });

    this.saveSubscription = this.editor.addAction({
      id: 'save-file',
      label: 'Save File',
      keybindings: [
        window.monaco.KeyMod.CtrlCmd |
        window.monaco.KeyCode.KeyS
      ],
      run: async () => await this.context.file.value?.save()
    });

    this.subscriptions.add(
      this.context.executed.subscribe(() =>
        this.workbench.setBottomView(this.bottomViews[0])
      )
    );

    this.subscriptions.add(
      this.context.disassembled.subscribe(() =>
        this.workbench.setBottomView(this.bottomViews[1])
      )
    );

    this.subscriptions.add(
      this.context.failure.subscribe(() =>
        this.workbench.setBottomView(this.bottomViews[2])
      )
    );
  }

  ngOnInit(): void {
    this.subscriptions.add(
      this.context.file.subscribe(() => this.updateModel())
    );
  }

  ngAfterViewInit(): void {
    this.workbench.setLeftView(this.leftViews[0]);
    this.workbench.setRightView(this.rightViews[0]);
    this.workbench.setBottomView(this.bottomViews[0]);
  }

  ngOnDestroy(): void {
    this.editor?.dispose();
    this.explorerSubscription?.dispose();
    this.grammarSubscription?.dispose();
    this.memorySubscription?.dispose();
    this.executeSubscription?.dispose();
    this.disassembleSubscription?.dispose();
    this.problemsSubscription?.dispose();
    this.saveSubscription?.dispose();
    this.subscriptions.unsubscribe();
  }

  private updateModel(): void {
    const file = this.context.file.value;

    if (!file || !this.editor)
      return;

    this.editor.setModel(file.getModel());
  }
}

