import {Service} from '@angular/core';
import {Subject} from 'rxjs';

@Service()
export class WorkbenchService {
  readonly setBottomView: Subject<number> = new Subject<number>();
}
