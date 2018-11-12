using Newtonsoft.Json;
using Enterprise.Lib.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Enterprise.Lib.Core.Extension
{
    public static class ObjectExtention
    {
        public static bool IsAllPropertiesNull(this object myObject)
        {
            return myObject.GetType().GetProperties()
            .Where(pi => pi.GetValue(myObject) is string)
            .Select(pi => pi.GetValue(myObject))
            .All(value => value == null);
        }

        public static bool IsAllPropertiesNullOrEmpty(this object myObject)
        {
            return myObject.GetType().GetProperties()
            .Where(pi => pi.GetValue(myObject) is string)
            .Select(pi => (string) pi.GetValue(myObject))
            .All(value => String.IsNullOrEmpty(value));
        }

        public static bool IsAnyPropertyNull(this object myObject)
        {
            return myObject.GetType().GetProperties()
            .Where(pi => pi.GetValue(myObject) is string)
            .Select(pi => pi.GetValue(myObject))
            .Any(value => value == null);
        }

        public static bool IsAnyPropertyNullOrEmpty(this object myObject)
        {
            return myObject.GetType().GetProperties()
            .Where(pi => pi.GetValue(myObject) is string)
            .Select(pi => (string)pi.GetValue(myObject))
            .Any(value => String.IsNullOrEmpty(value));
        }

        public static bool IsAnyPropertyNotNullOrEmpty(this object myObject)
        {
            return myObject.GetType().GetProperties()
            .Where(pi => pi.GetValue(myObject) is string)
            .Select(pi => (string)pi.GetValue(myObject))
            .Any(value => !String.IsNullOrEmpty(value));
        }

        public static object GetPropertyValue(this object myObject, string propertyName)
        {
            if (IsPropertyExist(myObject, propertyName))
                return myObject.GetType().GetProperties()
                        .Where(p => p.Name.EqualsIgnoreCase(propertyName))
                        .Select(pi => pi.GetValue(myObject)).FirstOrDefault();

            return null;
        }

        public static T ConvertToType<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static bool IsPropertyExist(this object obj, string property)
        {
            return ((Type)obj.GetType()).GetProperties().Where(p => p.Name.EqualsIgnoreCase(property)).Any();
        }

        /// <summary>
        /// This function will check if any [DataMember] is not null besides DataMember(IsRequired = true)
        /// DataMember(IsRequired = true) are used only for identification and does not qualify as interested field
        /// You cannot have just one member as DataMember(IsRequired = true) and expect this to return true
        /// you should have atleast one other member marked as [DataMember] besides DataMember(IsRequired = true)
        /// All members should be Nullable e.g. int? bool? DateTime?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool CheckDataContractRequiredFieldswithAtleastOneNotEmptyOtherField<T>(this T obj)
        {
            if (!Attribute.IsDefined(typeof(T), typeof(DataContractAttribute)))
            {
                throw new Exception($"{typeof(T).ToString()} does not have attribute DataContract");
            }

            var shouldProcess = false;

            var props = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(DataMemberAttribute)));

            if (props == null || props.Count() <=0)
            {
                throw new Exception($"To process data, atleast one property should have [DataMember] attribute");
            }

            foreach (var prop in props)
            {
                bool isRequired = prop.GetAttributValue((DataMemberAttribute a) => a.IsRequired);
                var val = prop.GetValue(obj);

                //if (!(val.GetType().IsNullable()))
                //    throw new Exception($"All attributes should be nullable. {prop.Name} is not Nullable.");

                val = (val == null) || (val is string && string.IsNullOrEmpty(val.ToString())) || (val.GetType() == typeof(DateTime) && default(DateTime) == (DateTime) val) ? null : val;

                //if field is required and is null then break, no need to process
                if (isRequired && val == null)
                {
                    return false;
                    //shouldProcess = false;
                    //break;
                }
                //if field is not required and is not null then we should proess the record as long as isRequired condition above keep isRequired true
                // and any property has not null value
                else if (!isRequired && val != null)
                {
                    shouldProcess = true;
                }
            }

            return shouldProcess;
        }

        public static TValue GetAttributValue<TAttribute, TValue>(this PropertyInfo prop, Func<TAttribute, TValue> value) where TAttribute : Attribute
        {
            var att = prop.GetCustomAttributes(
                typeof(TAttribute), true
                ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return value(att);
            }
            return default(TValue);
        }

        public static string ToOrgContentResultFormat(this object obj, HttpStatusCode httpStatusCode= HttpStatusCode.OK)
        {
            if (obj == null)
                obj = new { content = string.Empty };

            var contentString = string.Empty;

            if (httpStatusCode != HttpStatusCode.OK)
            {
                contentString = JsonConvert
                    .SerializeObject(
                        new
                        {
                            content = string.Empty,
                            status = new
                            {
                                message = obj,
                                code = httpStatusCode,
                                clientip = HttpHelper.GetRequestIp(),
                                clienthostname = HttpHelper.GetHostname()
                            }
                        }
                    );
            }
            else
            {
                contentString = JsonConvert
                    .SerializeObject(
                        new
                        {
                            content = obj,
                            status = new
                            {
                                message = "success",
                                code = httpStatusCode,
                                clientip = HttpHelper.GetRequestIp(),
                                clienthostname = HttpHelper.GetHostname()
                            }
                        }
                    );
            }

            return contentString;

        }

        public static bool Compare<T>(this T Object1, T object2)
        {
            //Get the type of the object
            Type type = typeof(T);

            //return false if any of the object is false
            if (Object1 == null || object2 == null)
                return false;

            //Loop through each properties inside class and get values for the property from both the objects and compare
            foreach (System.Reflection.PropertyInfo property in type.GetProperties())
            {
                if (property.Name != "ExtensionData")
                {
                    string Object1Value = string.Empty;
                    string Object2Value = string.Empty;
                    if (type.GetProperty(property.Name).GetValue(Object1, null) != null)
                        Object1Value = type.GetProperty(property.Name).GetValue(Object1, null).ToString();
                    if (type.GetProperty(property.Name).GetValue(object2, null) != null)
                        Object2Value = type.GetProperty(property.Name).GetValue(object2, null).ToString();
                    if (Object1Value.Trim() != Object2Value.Trim())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsNotMapped(this PropertyInfo propertyInfo)
        {
            try
            {
                var attrs = propertyInfo.GetCustomAttributes(typeof(NotMappedAttribute), true);
                if (attrs.Any())
                    //result = ((DisplayAttribute)attrs[0]).Name;
                    return true;
            }
            catch (Exception)
            {
                //eat the exception
            }
            return false;
        }

        public static bool IsValidPassword(this string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@"^(?s)(.){8,15}$",RegexOptions.Singleline);
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one lower case letter";
                return false;
            }
            else if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one upper case letter";
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage = "Password should not be less than 8 characters or greater than 15 characters";
                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one numeric value";
                return false;
            }

            else if (!hasSymbols.IsMatch(input))
            {
                ErrorMessage = "Password should contain At least one special case characters";
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsValidPhoneNumer(this string str)
        {
            if (str != null && str.Trim().ToLower() == "null")
                return true;

            //string pattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            string pattern = @"^\W*([0-9][0-9][0-9])\W*([0-9][0-9]{2})\W*([0-9]{4})(\se?x?t?(\W*\d*\W*))?";

            return Regex.IsMatch(str, pattern, RegexOptions.IgnorePatternWhitespace);

        }

        public static bool IsValidEmail(this string str)
        {
            if (str != null && str.Trim().ToLower() == "null")
                return true;

            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

           return Regex.IsMatch(str, pattern, RegexOptions.IgnorePatternWhitespace);
        }

        public static bool IsValidSSN(this string str)
        {
            if (str != null && str.Trim().ToLower() == "null")
                return true;

            string pattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{2})[-. ]?([0-9]{4})$";

            return Regex.IsMatch(str, pattern, RegexOptions.IgnorePatternWhitespace);
        }

        public static bool RemoveSpecialCharacters(this string str)
        {
            string pattern = @"[^\w\d]";

            return Regex.IsMatch(str, pattern, RegexOptions.IgnorePatternWhitespace);

        }

        public static bool IsValidDateTime(this string str, out DateTime dt)
        {
            return  DateTime.TryParse(str, out dt);

            //return DateTime.TryParse(str, out dt);
        }

        public static bool IsValidDate(this string str, out DateTime dt)
        {

            //if (str != null && str.Trim().ToLower() == "null")
            //{
            //    dt = DateTime.MinValue;
            //    return true;
            //}
            //var dateTimeFormats = DateTimeFormatInfo.CurrentInfo.GetAllDateTimePatterns('d');

            string[] dateTimeFormats = {
                    @"MMddyyyy",@"MM/dd/yyyy",@"MM-dd-yyyy",
                    @"Mddyyyy",@"M/dd/yyyy",@"M-dd-yyyy",
                    @"MMdyyyy",@"MM/d/yyyy",@"MM-d-yyyy",
                    @"Mdyyyy",@"M/d/yyyy",@"M-d-yyyy"
            };

            return DateTime.TryParseExact(str,
                                      dateTimeFormats,
                                      System.Globalization.CultureInfo.InvariantCulture,
                                      System.Globalization.DateTimeStyles.None,
                                      out dt);

            //return DateTime.TryParse(str, out dt);
        }

        public static string GetValOrNull(this string str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? null : str;
        }


        public static string GetNotNullVal(this string str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) ? string.Empty : str;
        }

        public static bool EqualsIgnoreCase(this string str, string strToComparWith)
        {
            str = str.GetNotNullVal().Replace("\"", string.Empty).ToLower().Trim();
            strToComparWith = strToComparWith.GetNotNullVal().Replace("\"", string.Empty).ToLower().Trim();

            return str == strToComparWith; 
        }

        public static string GetValueOrAlternate(this string str, string alternateValue)
        {
            if (string.IsNullOrEmpty(str))
                return alternateValue;

            return str;
        }

        public static bool EqualsToEmptyOrValueIgnoreCase(this string str, string value)
        {
            str = str.GetNotNullVal().Replace("\"", string.Empty).ToLower().Trim();
            value = value.GetNotNullVal().Replace("\"", string.Empty).ToLower().Trim();

            return str == string.Empty || str == value;
        }

        public static object GetPropertyValue(this ExpandoObject dObj, string propertyName)
        {
            if (dObj.IsPropertyExist(propertyName))
                return dObj.GetPropertyValue(propertyName);

            return null;
        }

        public static DateTime ToDateTime(this string str)
        {
            str = str.GetNotNullVal().Replace("\"", string.Empty).ToLower().Trim();

            if (str.IsValidDateTime(out DateTime dt))
                return dt;
            return DateTime.MinValue;
        }

        public static DateTime ToDate(this string str)
        {
            str = str.GetNotNullVal().Replace("\"", string.Empty).ToLower().Trim();

            if (str.IsValidDate(out DateTime dt))
                return dt;
            return DateTime.MinValue;
        }

        public static DateTime? ToDateTimeNullable(this string str)
        {
            str = str.GetNotNullVal().Replace("\"", string.Empty).ToLower().Trim();

            var dt = str.ToDate();
            if (dt == DateTime.MinValue)
                return null;

            return dt;
        }

        public static bool MinValOrEqualsTo(this DateTime dt, DateTime dtToComparWith)
        {
            return dt == DateTime.MinValue || dt == dtToComparWith;
        }

        public static bool MinValOrEqualsTo(this DateTime dt, DateTime? dtToComparWith)
        {
            return dt == DateTime.MinValue || dt == dtToComparWith;
        }

        public static bool NullOrEqualsTo(this DateTime? dt, DateTime? dtToComparWith)
        {
            return dt == null || dt == DateTime.MinValue || dt == dtToComparWith;
        }

        public static int GetIntVal(this int? intVal)
        {
            return intVal == null ? -1 : intVal ?? default(int);
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsValueType) return Activator.CreateInstance(type) == null;

            return true;
        }

    }
}
