import {Type} from '@angular/core';

export interface WorkbenchView {
  id: string;
  title: string;
  icon: string;
  view: Type<any>;
  badge?: () => number;
}
