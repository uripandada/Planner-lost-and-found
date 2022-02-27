import { FormArray, FormControl, FormGroup } from '@angular/forms';

// custom validator to check that two fields match
export function atLeastOneSelected() {
    return (formArray: FormArray) => {
      let selecteds = formArray.getRawValue().filter(isSelected => isSelected);
      return selecteds.length > 0 ? null : { atLeastOneSelected: 'You must select at least one' }
    }
}
