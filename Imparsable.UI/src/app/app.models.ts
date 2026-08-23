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

export interface SourceFile {
  name: string;
  content: string;
  languageId: string;
}

export type StdOutput = { id: number; text: string };
