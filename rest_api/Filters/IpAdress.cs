using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;


namespace rest_api.Filters
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class IpAdress : ValidationAttribute
    {
       public override bool IsValid(object value)
        {
            IPAddress ip;
            return IPAddress.TryParse(value.ToString(), out ip);
        }
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture,
              ErrorMessageString, name);
        }
    }
}
