import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

// Leverage TypeScript type guards to check to see if we have a Character type selected
function isObject(value: any): value is object {
  return !!value // truthy
    && typeof value !== "string" // Not just string input in the autocomplete
    && typeof value === "object"; // Has some qualifying property of Character type
}

export const AutocompleteSingleSelectRequiredValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  return isObject(control?.value) ? null : { required: true };
}
export const SingleSelectWhereRequiredValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  let controlValue = control?.value;
  return controlValue && controlValue["referenceId"] ? null : { required: true };
}
