import {Component, inject, OnDestroy} from '@angular/core';
import {IdeWidget} from '../../app.models';
import {Ide} from '../ide/ide';
import {LanguageId} from '../../app.config.monaco';
import {lsp, editor} from 'monaco-editor';
import {CalculatorRunner} from '../calculator-runner/calculator-runner';
import {CalculatorContext} from '../../services/calculator-context';

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
  private readonly context: CalculatorContext = inject(CalculatorContext);
  private editor?: editor.IStandaloneCodeEditor;
  private transport?: lsp.WebSocketTransport;
  private client?: lsp.MonacoLspClient;
  protected model?: editor.ITextModel;

  side: IdeWidget[] = [
    {id: 'explorer', label: 'Explorer', icon: 'folder', view: Explorer},
    // {id: 'search', label: 'Search', icon: 'search', view: Stub},
    // {id: 'outline', label: 'Outline', icon: 'account_tree', view: Stub},
  ];
  bottom: IdeWidget[] = [
    {id: 'run', label: 'Run', icon: 'play_arrow', view: CalculatorRunner},
    // {id: 'problems', label: 'Problems', icon: 'play_arrow', view: Stub},
    // {id: 'output', label: 'Output', icon: 'output', view: Stub},
    // {id: 'terminal', label: 'Terminal', icon: 'terminal', view: Stub},
  ];

  async init(editor: editor.IStandaloneCodeEditor): Promise<void> {
    this.editor = editor;
    this.transport = await window.monaco.lsp.WebSocketTransport.connectTo({address: "wss://localhost:5001/lsp/clc"});
    this.client = new window.monaco.lsp.MonacoLspClient(this.transport);

    // this.model = window.monaco.editor.createModel(
    //   code,
    //   LanguageId.Calculator,
    //   window.monaco.Uri.parse('file://workspace/test.clc')
    // );
    // editor.setModel(this.model);


    this.context.model.set(
      window.monaco.editor.createModel(
        code,
        LanguageId.Calculator,
        window.monaco.Uri.parse('file://workspace/test.clc')
      )
    );
    this.editor.setModel(this.context.model()!)
    // window.monaco.editor.setModelLanguage(this.model, LanguageId.Calculator);
  }

  ngOnDestroy(): void {
    this.transport?.close();
    this.model?.dispose();
    this.editor?.dispose();

    this.client = undefined;
    this.transport = undefined;
  }
}

@Component({
  selector: 'app-explorer',
  imports: [],
  template: `
    <div>main.clc</div>`,
})
class Explorer {
}
