import {Directive, HostListener} from '@angular/core';

@Directive({
  selector: '[appPositiveInteger]',
})
export class PositiveIntegerDirective {

  constructor() { }

  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    if (e.key === '-' || e.key === 'e') e.preventDefault();
  }

  @HostListener('input', ['$event'])
  onInput(e: Event) {
    const input = e.target as HTMLInputElement;
    input.value = input.value.replace(/[^0-9]/g, '');
  }

}
