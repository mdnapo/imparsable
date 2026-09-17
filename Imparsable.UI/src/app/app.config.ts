import {ApplicationConfig, provideBrowserGlobalErrorListeners} from '@angular/core';
import {provideRouter} from '@angular/router';
import {routes} from './app.routes';
import {provideMonacoEditor} from 'ngx-monaco-editor-v2';
import {provideHttpClient} from '@angular/common/http';
import {provideCharts, withDefaultRegisterables} from 'ng2-charts';
import {provideDefaultClient} from '@api/imparsable';
import {MonacoOptions} from '@config/monaco';

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
