using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Validators
{
    public class TitleValidator : ValidationAttribute
    {
        readonly int _maxLenght;
        readonly int _minLenght;
        public TitleValidator(int minLenght = 3, int maxLenght = 50)
        {
            _maxLenght = maxLenght;
            _minLenght = minLenght;
        }

        //validation Context, it is what in constructor
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var title = value.ToString();
            if (string.IsNullOrEmpty(title))
            {
                return new ValidationResult("Не может быть пустым");
            }
            if (title.Length < _minLenght)
            {
                return new ValidationResult($"Не может быть меньше {_minLenght} символов");
            }
            if (!Regex.IsMatch(title, @"^[a-zA-Z]+$"))
            {
                return new ValidationResult("Не может содержать в себе другие символы, кроме букв");
            }
            if (title.Length > _maxLenght)
            {
                return new ValidationResult($"Не может быть больше {_maxLenght} символов");
            }
            return ValidationResult.Success;
        }
    }
}
