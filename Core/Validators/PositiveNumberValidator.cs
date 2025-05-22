using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Validators
{
    public class PositiveNumberValidator : ValidationAttribute
    {
        readonly int _max;
        readonly int _min;
        public PositiveNumberValidator(int min = 0, int max = -1)
        {
            _max = max;
            _min = min;
        }

        //validation Context, it is what in constructor
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var myObject = int.Parse(value.ToString());
            if (string.IsNullOrEmpty(myObject.ToString()))
            {
                return new ValidationResult("NotEmpty");
            }
            if (myObject < _min)
            {
                return new ValidationResult("InValidParams");
            }
            if (myObject > _max && _max != -1)
            {
                return new ValidationResult("InValidParams");
            }
            return ValidationResult.Success;
        }
    }
}
