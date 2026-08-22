import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import bootsharp from "imp-wasm";

bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));

await bootsharp.boot();
