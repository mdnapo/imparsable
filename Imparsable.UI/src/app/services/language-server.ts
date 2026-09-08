import {OnDestroy, Service} from '@angular/core';
import type {lsp} from 'monaco-editor';

interface Connection {
  transport: lsp.WebSocketTransport;
  client: lsp.MonacoLspClient;
}

function getWebSocketUrl(path: string): string {
  const protocol: string = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
  return `${protocol}//${window.location.host}${path}`;
}

@Service()
export class LanguageServer implements OnDestroy {
  private readonly connections: Map<string, Connection> = new Map<string, Connection>();

  async connect(languageId: string, path: string): Promise<void> {
    if (this.connections.has(languageId))
      return;

    const address = getWebSocketUrl(path);
    const transport = await window.monaco.lsp.WebSocketTransport.connectTo({address});
    const client = new window.monaco.lsp.MonacoLspClient(transport);

    this.connections.set(languageId, {transport, client});
  }

  ngOnDestroy(): void {
    this.connections.forEach((connection: Connection): void => {
      connection.transport.dispose();
    });
  }
}
