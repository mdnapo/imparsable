import {Component} from '@angular/core';
import {IdeWidget} from '../../app.models';
import {Ide2} from '../ide2/ide2';

@Component({
  selector: 'app-calculator-ide',
  imports: [
    Ide2
  ],
  templateUrl: './calculator-ide.html',
  styleUrl: './calculator-ide.scss',
})
export class CalculatorIde {
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
      var r = Math.random() * 16 | 0,
        v = c == 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }
}
