import {Component, inject, OnDestroy, ViewChild} from '@angular/core';
import {IdeWidget} from '../../app.models';
import {Ide} from '../ide/ide';
import {LanguageId} from '../../app.config.monaco';
import {lsp, editor} from 'monaco-editor';
import {CalculatorRunner} from '../calculator-runner/calculator-runner';
import {CalculatorContext} from '../../services/calculator-context';
import {CalculatorProblems} from '../calculator-problems/calculator-problems';
import {CalculatorDisassembler} from '../calculator-disassembler/calculator-disassembler';
import {Subscription} from 'rxjs';

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
  imports: [Ide],
  templateUrl: './calculator-ide.html',
  styleUrl: './calculator-ide.scss',
})
export class CalculatorIde implements OnDestroy {
  @ViewChild(Ide)
  private ide!: Ide;
  private readonly context: CalculatorContext = inject(CalculatorContext);
  private editor?: editor.IStandaloneCodeEditor;
  private transport?: lsp.WebSocketTransport;
  private client?: lsp.MonacoLspClient;
  private subscription: Subscription = new Subscription();
  protected model?: editor.ITextModel;

  side: IdeWidget[] = [
    {id: 'explorer', icon: 'folder', view: Explorer},
  ];

  bottom: IdeWidget[] = [
    {id: 'runner', icon: 'play_arrow', view: CalculatorRunner},
    {id: 'disassembler', icon: 'data_array', view: CalculatorDisassembler},
    {id: 'problems', icon: 'error', view: CalculatorProblems, badge: () => this.context.errors()},
  ];

  async init(editor: editor.IStandaloneCodeEditor): Promise<void> {
    this.editor = editor;
    this.transport = await window.monaco.lsp.WebSocketTransport.connectTo({address: "wss://localhost:5001/lsp/clc"});
    this.client = new window.monaco.lsp.MonacoLspClient(this.transport);

    this.context.model.set(
      window.monaco.editor.createModel(
        code,
        LanguageId.Calculator,
        window.monaco.Uri.parse('file://workspace/test.clc')
      )
    );

    this.editor.setModel(this.context.model()!);

    this.subscription.add(
      this.context.failure.subscribe(failed => {
        if (failed) {
          this.ide.bottomView = this.bottom[2];
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.transport?.close();
    this.model?.dispose();
    this.editor?.dispose();
    this.subscription.unsubscribe();

    this.client = undefined;
    this.transport = undefined;
  }
}

@Component({
  selector: 'app-explorer',
  imports: [],
  template: `
    <h3>Explorer</h3>
    <div>&nbsp;&nbsp;&nbsp;&nbsp;main.clc</div>`,
})
class Explorer {
}
