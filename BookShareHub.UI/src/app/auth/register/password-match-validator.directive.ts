import { Directive, forwardRef, Input, OnChanges } from '@angular/core';
import {
  AbstractControl,
  NG_VALIDATORS,
  ValidationErrors,
  Validator,
} from '@angular/forms';

@Directive({
  selector: '[appPasswordMatch]',
  standalone: true,
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => PasswordMatchValidatorDirective),
      multi: true,
    },
  ],
})
export class PasswordMatchValidatorDirective implements Validator, OnChanges {
  @Input() appPasswordMatch = '';

  private onValidatorChange = () => {};

  validate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    return control.value === this.appPasswordMatch ? null : { passwordMismatch: true };
  }

  registerOnValidatorChange(fn: () => void): void {
    this.onValidatorChange = fn;
  }

  ngOnChanges(): void {
    this.onValidatorChange();
  }
}
