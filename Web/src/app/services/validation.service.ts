import {Injectable} from '@angular/core';
import {FormGroup, FormControl, ValidationErrors} from '@angular/forms';

@Injectable()
export class ValidationService {

  constructor() 
  {


  }

      isFieldValid(field: string, form: FormGroup): boolean {

        return !form.get(field).valid && form.get(field).touched;
      }
    
      GetErrors(field: string,form: FormGroup): Array<string> {
    
         var nums = [];
        
          const controlErrors: ValidationErrors = form.get(field).errors;
    
          if (controlErrors != null) {
             Object.keys(controlErrors).forEach(keyError => {
              nums.push(controlErrors[keyError]);
            });
          }
    
        return nums;
      }
      
      displayFieldCss(field: string, form: FormGroup) {
        return {
          'has-error': this.isFieldValid(field,form),
          'has-feedback': this.isFieldValid(field,form)
        };
      }
    
      validateAllFormFields(formGroup: FormGroup) {         //{1}
        Object.keys(formGroup.controls).forEach(field => {  //{2}
          const control = formGroup.get(field);             //{3}
          if (control instanceof FormControl) {             //{4}
            control.markAsTouched({ onlySelf: true });
          } 
          else if (control instanceof FormGroup) {        //{5}
            this.validateAllFormFields(control);            //{6}
          }
        });
      }   
}