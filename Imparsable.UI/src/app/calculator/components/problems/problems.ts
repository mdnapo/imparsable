import {Component, inject} from '@angular/core';
import {AsyncPipe} from "@angular/common";
import {MatIcon} from "@angular/material/icon";
import {ToolWindow} from "@shared/components/tool-window/tool-window";
import {Diagnostic, DiagnosticSeverity} from '@shared/models/language';
import {ProblemService} from '@calculator/services/problem-service';

@Component({
  selector: 'app-problems',
  imports: [
    AsyncPipe,
    MatIcon,
    ToolWindow
  ],
  templateUrl: './problems.html',
  styleUrl: './problems.scss',
})
export class Problems {
  protected readonly service: ProblemService = inject(ProblemService);

  protected severityIcon(diagnostic: Diagnostic): string {
    switch (diagnostic.severity) {
      case DiagnosticSeverity.ERROR:
        return 'error';
      case DiagnosticSeverity.WARNING:
        return 'warning';
      default:
        return 'info';
    }
  }

  protected severityClass(diagnostic: Diagnostic): string {
    return `severity-${DiagnosticSeverity[diagnostic.severity].toLowerCase()}`;
  }
}

