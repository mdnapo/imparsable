import {NgxMonacoEditorConfig} from 'ngx-monaco-editor-v2';

export const LanguageId = {
  Calculator: 'clc'
}

export const DefaultOptions: NgxMonacoEditorConfig = {
  defaultOptions: {
    theme: 'vscode',
    automaticLayout: true,
    wordBasedSuggestions: 'off',
    suggest: {showWords: false},
    minimap: {enabled: true},
    scrollBeyondLastLine: false,
    fontSize: 14,
  },
  onMonacoLoad: () => {
    window.monaco.languages.register({id: LanguageId.Calculator, extensions: [`.${LanguageId.Calculator}`]});
    window.monaco.languages.setMonarchTokensProvider(LanguageId.Calculator, {
      keywords: ['const', 'var', 'print', 'for', 'while', 'break', 'continue', 'true', 'false', 'if', 'else'],
      operators: ['%', '||', '&&', '+', '+=', '-', '-=', '*', '*=', '/', '/=', '!', '<', '>', '!=', '==', '<=', '=>'],
      tokenizer: {
        root: [
          // Keywords
          [/\b(?:const|var|print|for|while|break|continue|true|false|if|else)\b/, 'keyword'],

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
}
