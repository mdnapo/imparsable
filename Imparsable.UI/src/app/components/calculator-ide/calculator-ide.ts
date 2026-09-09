import {AfterViewInit, Component, inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {IdeWidget} from '../../app.models';
import {Ide} from '../ide/ide';
import type {editor, IDisposable} from 'monaco-editor';
import {CalculatorRunner} from '../calculator-runner/calculator-runner';
import {CalculatorContext} from '../../services/calculator-context';
import {CalculatorProblems} from '../calculator-problems/calculator-problems';
import {CalculatorDisassembler} from '../calculator-disassembler/calculator-disassembler';
import {Subscription} from 'rxjs';
import {CalculatorExplorer} from '../calculator-explorer/calculator-explorer';
import {AsyncPipe} from '@angular/common';
import {LanguageServer} from '../../services/language-server';
import {LanguageId} from '../../app.config.monaco';
import {CalculatorGrammar} from '../calculator-grammar/calculator-grammar';

@Component({
  selector: 'app-calculator-ide',
  imports: [Ide, AsyncPipe],
  templateUrl: './calculator-ide.html',
  styleUrl: './calculator-ide.scss'
})
export class CalculatorIde implements OnInit, AfterViewInit, OnDestroy {
  protected readonly context: CalculatorContext = inject(CalculatorContext);
  private readonly languageServer: LanguageServer = inject(LanguageServer);

  private editor?: editor.IStandaloneCodeEditor;
  private subscriptions: Subscription = new Subscription();
  private explorerSubscription?: IDisposable;
  private grammarSubscription?: IDisposable;
  private executeSubscription?: IDisposable;
  private disassembleSubscription?: IDisposable;
  private problemsSubscription?: IDisposable;
  private saveSubscription?: IDisposable;

  @ViewChild(Ide)
  private ide!: Ide;

  protected side: IdeWidget[] = [
    {
      id: 'explorer',
      alt: 'Explorer (ctrl + alt + f)',
      icon: 'folder',
      view: CalculatorExplorer
    },
    {
      id: 'grammar',
      alt: 'Grammar (ctrl + alt + g)',
      icon: 'regular_expression',
      view: CalculatorGrammar
    },
  ];

  protected bottom: IdeWidget[] = [
    {
      id: 'runner',
      alt: 'Execute (ctrl + shift + x)',
      icon: 'terminal_2',
      view: CalculatorRunner
    },
    {
      id: 'disassembler',
      alt: 'Disassemble (ctrl + shift + d)',
      icon: 'data_array',
      view: CalculatorDisassembler
    },
    {
      id: 'problems',
      alt: 'Problems (ctrl + shift + q)',
      icon: 'error',
      view: CalculatorProblems,
      badge: () => this.context.errors()
    },
  ];

  async init(editor: editor.IStandaloneCodeEditor): Promise<void> {
    this.editor = editor;

    this.updateModel();

    await this.languageServer.connect(LanguageId.Calculator, '/lsp/clc');

    this.explorerSubscription = this.editor.addAction({
      id: 'open-explorer',
      label: 'Open Explorer',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyMod.Alt | window.monaco.KeyCode.KeyF],
      run: () => this.ide.setSideView(this.side[0])
    });

    this.grammarSubscription = this.editor.addAction({
      id: 'open-grammar',
      label: 'Open Grammar',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyMod.Alt | window.monaco.KeyCode.KeyG],
      run: () => this.ide.setSideView(this.side[1])
    });

    this.executeSubscription = this.editor.addAction({
      id: 'execute-file',
      label: 'Execute File',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyMod.Shift | window.monaco.KeyCode.KeyX],
      run: () => this.context.execute()
    });

    this.problemsSubscription = this.editor.addAction({
      id: 'open-problems',
      label: 'Open Problems',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyMod.Shift | window.monaco.KeyCode.KeyQ],
      run: () => this.ide.setBottomView(this.bottom[2])
    });

    this.disassembleSubscription = this.editor.addAction({
      id: 'disassemble-file',
      label: 'Disassemble File',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyMod.Shift | window.monaco.KeyCode.KeyD],
      run: () => this.context.disassemble()
    });

    this.saveSubscription = this.editor.addAction({
      id: 'save-file',
      label: 'Save File',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyCode.KeyS],
      run: async () => await this.context.file.value?.save()
    });

    this.subscriptions.add(this.context.executed.subscribe(() => this.ide.setBottomView(this.bottom[0])));
    this.subscriptions.add(this.context.disassembled.subscribe(() => this.ide.setBottomView(this.bottom[1])));
    this.subscriptions.add(this.context.failure.subscribe(() => this.ide.setBottomView(this.bottom[2])));
  }

  private updateModel(): void {
    const file = this.context.file.value;

    if (!file || !this.editor)
      return;

    this.editor.setModel(file.getModel());
  }

  ngOnInit(): void {
    this.subscriptions.add(this.context.file.subscribe(file => this.updateModel()));
  }

  ngAfterViewInit(): void {
    this.ide.setSideView(this.side[0]);
    this.ide.setBottomView(this.bottom[0]);
  }

  ngOnDestroy(): void {
    this.editor?.dispose();
    this.explorerSubscription?.dispose();
    this.grammarSubscription?.dispose();
    this.saveSubscription?.dispose();
    this.executeSubscription?.dispose();
    this.disassembleSubscription?.dispose();
    this.problemsSubscription?.dispose();
    this.subscriptions.unsubscribe();
  }
}

