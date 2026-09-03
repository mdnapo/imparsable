import {ApplicationConfig, provideBrowserGlobalErrorListeners} from '@angular/core';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {provideMonacoEditor} from 'ngx-monaco-editor-v2';
import {DefaultOptions} from './app.config.monaco';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideMonacoEditor(DefaultOptions)
  ]
};
