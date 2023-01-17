using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;
using System;

namespace Library.Models
{
    public class IsISBN : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string pattern = @"^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0-9X]{13}$|97[89][0-9]{10}$|(?=(?:[0-9]+[- ]){4})[- 0-9]{17}$)(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]$";
            if (value != null)
            {
                if (value is string)
                {
                    string ISBN = (string)value;
                    if (Regex.IsMatch(ISBN, pattern))
                    {
                        var chars = Regex.Replace(ISBN, @"[- ]|^ISBN(?:-1[03])?:?", "").ToList();

                        var last = (int)char.GetNumericValue(chars[chars.Count - 1]);
                        chars.RemoveAt(chars.Count - 1);

                        var sum = 0;
                        int check;

                        if (chars.Count == 9)
                        {
                            chars.Reverse();
                            for (int i = 0; i < chars.Count; i++)
                            {
                                sum += (i + 2) * ((int)char.GetNumericValue(chars[i]));
                            }
                            check = (11 - (sum % 11));
                            if (check == 10)
                            {
                                check = -1;
                            }
                            else if (check == 11)
                            {
                                check = 0;
                            }
                        }
                        else {
                            for (int i = 0; i < chars.Count; i++)
                            {
                                sum += (((i % 2) * 2) + 1) * ((int)char.GetNumericValue(chars[i]));
                            }
                            check = (10 - (sum % 10));
                            if (check == 10)
                            {
                                check = 0;
                            }
                        }
                        if (check == last)
                        {
                            return ValidationResult.Success;
                        }
                        else {
                            return new ValidationResult("Invalid ISBN Check digit");
                        }
                    }
                    else {
                        return new ValidationResult("Invalid ISBN");
                    }
                }
            }
            return new ValidationResult("Invalid ISBN");

        }
    }
}