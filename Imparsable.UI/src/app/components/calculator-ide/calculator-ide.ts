import {Component, inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {IdeWidget} from '../../app.models';
import {Ide} from '../ide/ide';
import {LanguageId} from '../../app.config.monaco';
import {lsp, editor} from 'monaco-editor';
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

const code: string = `const pi = 3.14;
const radius = 4 / 2;
var area = 2 * pi * radius;
print "Area" + ': ' + area;
print 1 + 2;

for (var x = 0; x < 3; x += 1)
    print x + 1;
`;

@Component({
  selector: 'app-calculator-ide',
  imports: [Ide, AsyncPipe],
  templateUrl: './calculator-ide.html',
  styleUrl: './calculator-ide.scss',
})
export class CalculatorIde implements OnInit, OnDestroy {
  @ViewChild(Ide)
  private ide!: Ide;
  protected readonly context: CalculatorContext = inject(CalculatorContext);
  private editor?: editor.IStandaloneCodeEditor;
  private transport?: lsp.WebSocketTransport;
  private client?: lsp.MonacoLspClient;
  private subscription: Subscription = new Subscription();

  side: IdeWidget[] = [
    {id: 'explorer', icon: 'folder', view: CalculatorExplorer},
  ];

  bottom: IdeWidget[] = [
    {id: 'runner', icon: 'terminal_2', view: CalculatorRunner},
    {id: 'disassembler', icon: 'data_array', view: CalculatorDisassembler},
    {id: 'problems', icon: 'error', view: CalculatorProblems, badge: () => this.context.errors()},
  ];

  async init(editor: editor.IStandaloneCodeEditor): Promise<void> {
    this.editor = editor;
    this.transport = await window.monaco.lsp.WebSocketTransport.connectTo({address: getWebSocketUrl('/lsp/clc')});
    this.client = new window.monaco.lsp.MonacoLspClient(this.transport);

    this.subscription.add(
      this.context.failure.subscribe(failed => {
        if (failed) {
          this.ide.bottomView = this.bottom[2];
        }
      })
    );
  }

  ngOnInit(): void {
    this.subscription.add(
      this.context.file.subscribe(file => {
        this.editor?.setModel(file);
      })
    );
  }

  ngOnDestroy(): void {
    this.transport?.close();
    this.editor?.dispose();
    this.subscription.unsubscribe();

    this.client = undefined;
    this.transport = undefined;
  }
}

