using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Currency.UserClasses
{
    static class StringExtension
    {
        public static bool IsCorrectForRequest(this string value)
        {
            string pattern = @"(^0[1-9]|[12][0-9]|3[01])\/(0[1-9]|1[0-2])\/(19[0-9][0-9]|20[01][0-9]|202[0-3])";
            
            return Regex.IsMatch(value, pattern);
        }

        public static bool IsCorrectXML(this string value)
        {
            string no_data = "Error in parameters";
            string valute = "Valute";
            
            return (!(value.Contains(no_data)) && (value.Contains(valute)));
        }
    }
}
