import {ApplicationConfig, provideBrowserGlobalErrorListeners} from '@angular/core';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {provideMonacoEditor} from 'ngx-monaco-editor-v2';
import {MonacoOptions} from './app.config.monaco';
import {provideHttpClient} from '@angular/common/http';
import {provideDefaultClient} from '../api/imparsable';
import {provideCharts, withDefaultRegisterables} from 'ng2-charts';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(),
    provideDefaultClient({basePath: location.origin}),
    provideMonacoEditor(MonacoOptions),
    provideCharts(withDefaultRegisterables()),
  ],
};
