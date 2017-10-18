using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace rest_api.Filters
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class Gender : ValidationAttribute
    {
       public override bool IsValid(object value)
        {
            if (value == null) return true;
            if (value.ToString() == "Bay" || value.ToString() == "Bayan") return true;
            return false;
        }
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              ErrorMessageString, name);
        }
    }
}
