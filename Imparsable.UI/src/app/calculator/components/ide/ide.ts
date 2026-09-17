import {AfterViewInit, Component, inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {Workbench} from "@shared/components/workbench/workbench";
import {WorkbenchEditor} from "@shared/components/workbench-editor/workbench-editor";
import {Subscription} from 'rxjs';
import type {editor} from 'monaco-editor';
import {WorkbenchView} from '@shared/models/workbench-view';
import {Problems} from '@calculator/components/problems/problems';
import {Disassembler} from '@calculator/components/disassembler/disassembler';
import {Runner} from '@calculator/components/runner/runner';
import {Memory} from '@calculator/components/memory/memory';
import {Grammar} from '@calculator/components/grammar/grammar';
import {Explorer} from '@calculator/components/explorer/explorer';
import {FileService} from '@calculator/services/file-service';
import {LanguageServer} from '@shared/services/language-server';
import {LanguageId} from '@config/monaco';
import {MemoryService} from '@calculator/services/memory-service';
import {ProblemService} from '@calculator/services/problem-service';
import {WorkbenchService} from '@calculator/services/workbench-service';
import {RunnerService} from '@calculator/services/runner-service';
import {DisassemblerService} from '@calculator/services/disassembler-service';
import {Calculator} from 'imp-wasm';
import {Callback} from '@shared/utils/types';

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
  @ViewChild(Workbench)
  private workbench!: Workbench;
  protected readonly context: FileService = inject(FileService);
  protected readonly workbenchService: WorkbenchService = inject(WorkbenchService);
  protected readonly runner: RunnerService = inject(RunnerService);
  protected readonly disassembler: DisassemblerService = inject(DisassemblerService);
  protected readonly memory: MemoryService = inject(MemoryService);
  protected readonly problems: ProblemService = inject(ProblemService);
  private readonly languageServer: LanguageServer = inject(LanguageServer);
  private readonly subscriptions: Subscription[] = [];

  private readonly failedCallback: Callback<void> =
    (): void => this.workbench.setBottomView(this.bottomViews[2]);

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
      badge: () => this.problems.errors()
    }
  ];

  async init(editor: editor.IStandaloneCodeEditor): Promise<void> {

    this.workbenchService.setEditor(editor);

    await this.languageServer.connect(LanguageId.Calculator, '/lsp/clc');
  }

  ngOnInit(): void {
    Calculator.onFailure.subscribe(this.failedCallback);

    this.subscriptions.push(
      this.workbenchService.editorInitialized.subscribe({
        next: async () => await this.languageServer.connect(LanguageId.Calculator, '/lsp/clc')
      })
    );

    this.subscriptions.push(
      this.workbenchService.setLeftView.subscribe(value => this.workbench.setLeftView(this.leftViews[value])),
      this.workbenchService.setRightView.subscribe(value => this.workbench.setRightView(this.rightViews[value])),
      this.workbenchService.setBottomView.subscribe(value => this.workbench.setBottomView(this.bottomViews[value])),
    );

    this.workbenchService.registerAction(() => {
      return {
        id: 'open-explorer',
        label: 'Open Explorer',
        keybindings: [
          window.monaco.KeyMod.CtrlCmd |
          window.monaco.KeyMod.Alt |
          window.monaco.KeyCode.KeyF
        ],
        run: () => this.workbench.setLeftView(this.leftViews[0], true)
      };
    });

    this.workbenchService.registerAction(() => {
      return {
        id: 'open-grammar',
        label: 'Open Grammar',
        keybindings: [
          window.monaco.KeyMod.CtrlCmd |
          window.monaco.KeyMod.Alt |
          window.monaco.KeyCode.KeyG
        ],
        run: () => this.workbench.setLeftView(this.leftViews[1], true)
      };
    });

    this.workbenchService.registerAction(() => {
      return {
        id: 'open-memory',
        label: 'Open Memory',
        keybindings: [
          window.monaco.KeyMod.CtrlCmd |
          window.monaco.KeyMod.Alt |
          window.monaco.KeyCode.KeyM
        ],
        run: () => this.workbench.setRightView(this.rightViews[0], true)
      };
    });

    this.workbenchService.registerAction(() => {
      return {
        id: 'execute-file',
        label: 'Execute File',
        keybindings: [
          window.monaco.KeyMod.CtrlCmd |
          window.monaco.KeyMod.Shift |
          window.monaco.KeyCode.KeyX
        ],
        run: () => this.runner.execute()
      };
    });

    this.workbenchService.registerAction(() => {
      return {
        id: 'open-problems',
        label: 'Open Problems',
        keybindings: [
          window.monaco.KeyMod.CtrlCmd |
          window.monaco.KeyMod.Shift |
          window.monaco.KeyCode.KeyQ
        ],
        run: () => this.workbench.setBottomView(this.bottomViews[2])
      };
    });

    this.workbenchService.registerAction(() => {
      return {
        id: 'disassemble-file',
        label: 'Disassemble File',
        keybindings: [
          window.monaco.KeyMod.CtrlCmd |
          window.monaco.KeyMod.Shift |
          window.monaco.KeyCode.KeyD
        ],
        run: () => this.disassembler.execute()
      };
    });

    this.workbenchService.registerAction(() => {
      return {
        id: 'save-file',
        label: 'Save File',
        keybindings: [
          window.monaco.KeyMod.CtrlCmd |
          window.monaco.KeyCode.KeyS
        ],
        run: async () => await this.context.file.value?.save()
      };
    });
  }

  ngAfterViewInit(): void {
    this.workbench.setLeftView(this.leftViews[0]);
    this.workbench.setRightView(this.rightViews[0]);
    this.workbench.setBottomView(this.bottomViews[0]);
  }

  ngOnDestroy(): void {
    Calculator.onFailure.unsubscribe(this.failedCallback);
    this.subscriptions.forEach(subscription => subscription.unsubscribe());
  }
}
