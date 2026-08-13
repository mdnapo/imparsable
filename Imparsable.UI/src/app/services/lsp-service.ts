import {OnDestroy, Service} from '@angular/core';

import {
  MonacoVscodeApiWrapper,
  type MonacoVscodeApiConfig
} from 'monaco-languageclient/vscodeApiWrapper';

import {
  LanguageClientWrapper,
  type LanguageClientConfig
} from 'monaco-languageclient/lcwrapper';

@Service()
export class LspService implements OnDestroy {
  private apiWrapper?: MonacoVscodeApiWrapper;
  private languageClient?: LanguageClientWrapper;
  private initialized = false;

  public async initialize(): Promise<void> {
    if (this.initialized) {
      return;
    }

    await this.initializeVscodeApi();
    await this.initializeLanguageClient();
    this.initialized = true;
  }

  private async initializeVscodeApi(): Promise<void> {
    const config: MonacoVscodeApiConfig = {
      $type: 'classic',
      viewsConfig: {
        $type: 'EditorService'
      }
    };

    this.apiWrapper = new MonacoVscodeApiWrapper(config);

    await this.apiWrapper.start();
  }

  private async initializeLanguageClient(): Promise<void> {
    const languageId = 'clc';

    const config: LanguageClientConfig = {
      languageId,
      connection: {
        options: {
          $type: 'WebSocketUrl',
          url: 'wss://localhost:5001/lsp/clc'
        }
      },

      clientOptions: {
        documentSelector: [languageId]
      }
    };

    this.languageClient = new LanguageClientWrapper(config);

    await this.languageClient.start();
  }

  ngOnDestroy(): void {
    console.log('Disposing languageClient...');
    this.languageClient?.dispose();
  }
}
