import {Type} from '@angular/core';

export enum DiagnosticSeverity {
  WARNING = 1,
  ERROR = 2,
}

export interface Marker {
  line: number;
  column: number;
  offset: number;
  length: number;
}

export interface Diagnostic {
  marker: Marker;
  message: string;
  severity: DiagnosticSeverity;
}

export type StdOutput = { id: number; text: string };

export interface IdeWidget {
  id: string;
  icon: string;
  view: Type<any>;
  badge?: () => number;
}
