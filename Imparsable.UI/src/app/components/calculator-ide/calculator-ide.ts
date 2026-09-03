import {Component, OnDestroy} from '@angular/core';
import {IdeWidget} from '../../app.models';
import {Ide} from '../ide/ide';
import {LanguageId} from '../../app.config.monaco';
import * as monaco from 'monaco-editor';

declare const window: { monaco: typeof monaco };

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
  private transport?: monaco.lsp.WebSocketTransport;
  private client?: monaco.lsp.MonacoLspClient;
  private editor?: monaco.editor.IStandaloneCodeEditor;
  protected model?: monaco.editor.ITextModel;

  side: IdeWidget[] = [
    {id: 'explorer', label: 'Explorer', icon: 'folder', view: Explorer},
    {id: 'search', label: 'Search', icon: 'search', view: Stub},
    {id: 'outline', label: 'Outline', icon: 'account_tree', view: Stub},
  ];
  bottom: IdeWidget[] = [
    {id: 'problems', label: 'Problems', icon: 'error_outline', view: Stub},
    {id: 'output', label: 'Output', icon: 'output', view: Stub},
    {id: 'terminal', label: 'Terminal', icon: 'terminal', view: Stub},
  ];

  async init(editor: monaco.editor.IStandaloneCodeEditor): Promise<void> {
    this.transport = await window.monaco.lsp.WebSocketTransport.connectTo({address: "wss://localhost:5001/lsp/clc"});
    this.client = new window.monaco.lsp.MonacoLspClient(this.transport);
    this.editor = editor;

    this.model = window.monaco.editor.createModel(code, LanguageId.Calculator, window.monaco.Uri.parse('file://workspace/test.clc'));
    editor.setModel(this.model);
    window.monaco.editor.setModelLanguage(this.model, LanguageId.Calculator);
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

@Component({
  selector: 'app-stub',
  imports: [],
  template: `
    <div>{{ newGuid() }}</div>`,
})
class Stub {
  newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
      const r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
