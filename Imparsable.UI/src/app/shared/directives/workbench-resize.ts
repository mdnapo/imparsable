import {Directive, ElementRef, EventEmitter, Input, Output} from '@angular/core';

export type WorkbenchResizeAxis = 'x' | 'y';

@Directive({
  selector: '[workbenchResize]',
  host: {
    '(pointerdown)': 'start($event)'
  }
})
export class WorkbenchResize {
  @Input({required: true}) workbenchResize!: HTMLElement;
  @Input({required: true}) resizeProperty!: string;
  @Input({required: true}) resizeValue!: number;
  @Input() resizeAxis: WorkbenchResizeAxis = 'x';
  @Input() resizeDirection = 1;
  @Input() resizeMin = 0;
  @Input() resizeMax = Number.MAX_SAFE_INTEGER;

  @Output() resizeValueChange: EventEmitter<number> = new EventEmitter<number>();

  constructor(private readonly element: ElementRef<HTMLElement>) {
  }

  protected start(event: PointerEvent): void {
    const target = this.element.nativeElement;
    target.setPointerCapture(event.pointerId);

    const start = this.position(event);
    const startValue = this.resizeValue;

    let nextValue = startValue;
    let frame = 0;

    const move = (event: PointerEvent): void => {
      nextValue = Math.max(
        this.resizeMin,
        Math.min(
          this.resizeMax,
          startValue + (this.position(event) - start) * this.resizeDirection
        )
      );

      if (frame !== 0)
        return;

      frame = requestAnimationFrame(() => {
        this.workbenchResize.style.setProperty(
          this.resizeProperty,
          `${nextValue}px`
        );

        frame = 0;
      });
    };

    const stop = (): void => {
      if (frame !== 0) {
        cancelAnimationFrame(frame);

        this.workbenchResize.style.setProperty(
          this.resizeProperty,
          `${nextValue}px`
        );
      }

      this.resizeValueChange.emit(nextValue);

      target.releasePointerCapture(event.pointerId);
      target.removeEventListener('pointermove', move);
      target.removeEventListener('pointerup', stop);
      target.removeEventListener('pointercancel', stop);
    };

    target.addEventListener('pointermove', move);
    target.addEventListener('pointerup', stop);
    target.addEventListener('pointercancel', stop);
  }

  private position(event: PointerEvent): number {
    return this.resizeAxis === 'x'
      ? event.clientX
      : event.clientY;
  }
}
