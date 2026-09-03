// import * as monaco from '@codingame/monaco-vscode-editor-api';
//
import {NgxMonacoEditorConfig} from 'ngx-monaco-editor-v2';

export const LanguageId = {
  Calculator: 'clc'
}

export const DefaultOptions: NgxMonacoEditorConfig = {
  defaultOptions: {
    theme: 'vs-dark',
    automaticLayout: true,
    minimap: {
      enabled: true,
    },
    scrollBeyondLastLine: false,
    fontSize: 14,
  }
}
//
// export function registerCalculatorLanguage(): void {
//   monaco.languages.register({id: LanguageId.Calculator, extensions: [`.${LanguageId.Calculator}`]});
//   monaco.languages.setMonarchTokensProvider(LanguageId.Calculator, {
//     keywords: ['const', 'var', 'print', 'for', 'while', 'break', 'continue', 'true', 'false', 'if', 'else'],
//     operators: ['%', '||', '&&', '+', '+=', '-', '-=', '*', '*=', '/', '/=', '!', '<', '>', '!=', '==', '<=', '=>'],
//     tokenizer: {
//       root: [
//         // Keywords
//         [/\b(?:const|var|print|for|while|break|continue|true|false|if|else)\b/, 'keyword'],
//
//         // Identifiers (quoted strings)
//         [/".*?"|'.*?'/, 'string'],
//
//         // Symbols: :, ;
//         [/;/, 'delimiter'],
//
//         // numbers
//         [/\d*\.\d+/, 'number.float'],
//         [/\d+/, 'number'],
//       ]
//     }
//   });
// }
