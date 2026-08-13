// import {NgxMonacoEditorConfig} from 'ngx-monaco-editor-v2';
// import * as monaco from 'monaco-editor';
//
// declare const window: { monaco: typeof monaco };
//
// const monacoConfig: NgxMonacoEditorConfig = {
//   onMonacoLoad: () => {
//     window.monaco.languages.register({id: 'clc'});
//
//     window.monaco.languages.setMonarchTokensProvider('clc', {
//       keywords: ['const', 'var', 'print'],
//       operators: ['+', '-', '*', '/'],
//       tokenizer: {
//         root: [
//           // Keywords
//           [/\b(?:const|var|print)\b/, 'keyword'],
//
//           // Identifiers (quoted strings)
//           [/".*?"|'.*?'/, 'string'],
//
//           // Symbols: :, ;
//           [/;/, 'delimiter'],
//
//           // numbers
//           [/\d*\.\d+([eE][\-+]?\d+)?/, 'number.float'],
//           [/\d+/, 'number'],
//         ]
//       }
//     });
//   }
// }
//
// export default monacoConfig;
