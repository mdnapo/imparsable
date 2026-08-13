import * as monaco from '@codingame/monaco-vscode-editor-api';

export const LanguageId = {
  Calculator: 'clc'
}

export function registerCalculatorLanguage(): void {
  monaco.languages.register({id: LanguageId.Calculator, extensions: [`.${LanguageId.Calculator}`]});
  monaco.languages.setMonarchTokensProvider(LanguageId.Calculator, {
    keywords: ['const', 'var', 'print'],
    operators: ['+', '-', '*', '/'],
    tokenizer: {
      root: [
        // Keywords
        [/\b(?:const|var|print)\b/, 'keyword'],

        // Identifiers (quoted strings)
        [/".*?"|'.*?'/, 'string'],

        // Symbols: :, ;
        [/;/, 'delimiter'],

        // numbers
        [/\d*\.\d+/, 'number.float'],
        [/\d+/, 'number'],
      ]
    }
  });
}
