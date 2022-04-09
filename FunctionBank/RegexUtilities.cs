using System.Globalization;
using System.Text.RegularExpressions;

namespace ZaitsevBankAPI.FunctionBank
{
    public class RegexUtilities
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static bool isValidPhone(string phone)
        {

            var phoneNumber = phone.Trim();
            return Regex.Match(phoneNumber, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$").Success;
        }

        public static string PhoneNumber(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            value = new System.Text.RegularExpressions.Regex(@"\D")
                .Replace(value, string.Empty);
            value = value.TrimStart('1');
            if (value.Length == 7)
                return Convert.ToInt64(value).ToString("(###)-##-##");
            if (value.Length == 10)
                return Convert.ToInt64(value).ToString("(###)-###-##-##");
            if (value.Length > 10)
                return Convert.ToInt64(value)
                    .ToString("(###)-###-##-##" + new String('#', (value.Length - 10)));
            return value;
        }
    }
}
