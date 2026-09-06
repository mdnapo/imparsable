import {AfterViewInit, Component, inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {IdeWidget} from '../../app.models';
import {Ide} from '../ide/ide';
import {lsp, editor, IDisposable} from 'monaco-editor';
import {CalculatorRunner} from '../calculator-runner/calculator-runner';
import {CalculatorContext} from '../../services/calculator-context';
import {CalculatorProblems} from '../calculator-problems/calculator-problems';
import {CalculatorDisassembler} from '../calculator-disassembler/calculator-disassembler';
import {Subscription} from 'rxjs';
import {CalculatorExplorer} from '../calculator-explorer/calculator-explorer';
import {AsyncPipe} from '@angular/common';

function getWebSocketUrl(path: string): string {
  const protocol: string = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
  return `${protocol}//${window.location.host}${path}`;
}

@Component({
  selector: 'app-calculator-ide',
  imports: [Ide, AsyncPipe],
  templateUrl: './calculator-ide.html',
  styleUrl: './calculator-ide.scss'
})
export class CalculatorIde implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild(Ide)
  private ide!: Ide;
  protected readonly context: CalculatorContext = inject(CalculatorContext);
  private editor?: editor.IStandaloneCodeEditor;
  private transport?: lsp.WebSocketTransport;
  private client?: lsp.MonacoLspClient;
  private subscriptions: Subscription = new Subscription();
  private executeSubscription?: IDisposable;
  private disassembleSubscription?: IDisposable;
  private problemsSubscription?: IDisposable;
  private saveSubscription?: IDisposable;

  protected side: IdeWidget[] = [
    {id: 'explorer', icon: 'folder', view: CalculatorExplorer},
  ];

  protected bottom: IdeWidget[] = [
    {id: 'runner', alt: 'Execute', icon: 'terminal_2', view: CalculatorRunner},
    {id: 'disassembler', icon: 'data_array', view: CalculatorDisassembler},
    {id: 'problems', icon: 'error', view: CalculatorProblems, badge: () => this.context.errors()},
  ];

  async init(editor: editor.IStandaloneCodeEditor): Promise<void> {
    this.editor = editor;
    this.transport = await window.monaco.lsp.WebSocketTransport.connectTo({address: getWebSocketUrl('/lsp/clc')});
    this.client = new window.monaco.lsp.MonacoLspClient(this.transport);

    this.executeSubscription = this.editor.addAction({
      id: 'execute-file',
      label: 'Execute File',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyMod.Shift | window.monaco.KeyCode.KeyX],
      run: () => this.context.execute()
    });

    this.problemsSubscription = this.editor.addAction({
      id: 'open-problems',
      label: 'Open Problems',
      keybindings: [window.monaco.KeyMod.CtrlCmd | window.monaco.KeyMod.Shift | window.monaco.KeyCode.KeyP],
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

  ngOnInit(): void {
    this.subscriptions.add(
      this.context.file.subscribe(file => {
        if (file === null) return;
        this.editor?.setModel(file.getModel());
      })
    );
  }

  ngAfterViewInit(): void {
    this.ide.setSideView(this.side[0]);
    this.ide.setBottomView(this.bottom[0]);
  }

  ngOnDestroy(): void {
    this.transport?.close();
    this.editor?.dispose();
    this.saveSubscription?.dispose();
    this.executeSubscription?.dispose();
    this.disassembleSubscription?.dispose();
    this.problemsSubscription?.dispose();
    this.subscriptions.unsubscribe();

    this.client = undefined;
    this.transport = undefined;
  }
}

