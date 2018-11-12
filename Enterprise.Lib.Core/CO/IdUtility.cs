using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Enterprise.Lib.Core.CO
{
    public static class IdUtility
    {
       public static string GetUniqueId()
        {
            //return Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==",string.Empty);

            return String.Format("{0:X}", Guid.NewGuid().GetHashCode());
        }
        public static string NewWebId(string firstName, string lastName, int maxLength = 16, int number = 0)
        {
            var prefix = firstName.Substring(0, 1);
            var postfix = lastName;
            var numberStr = number > 0 ? number.ToString() : string.Empty;

            var webId = $"{prefix.ToLower()}{postfix.ToLower()}{numberStr}";

            string regExp = @"[^\w\d]";

            webId = Regex.Replace(webId.Trim(), regExp, "");
            return webId;
        }

        public static string NewWebIdNext(string webId, IEnumerable<string> listOfExistingWebIds)
        {
            var listOfAllWebIdsWithDBID = new List<string>();
            listOfAllWebIdsWithDBID = listOfExistingWebIds.ToList();
            listOfAllWebIdsWithDBID.Add(webId);

            var checkIfAllWebIdsInTheListStartWithWebId = listOfAllWebIdsWithDBID.Select(x => x.ToLower().StartsWith(webId.ToLower())).Any();

            if (checkIfAllWebIdsInTheListStartWithWebId)
            {
                var numList = listOfAllWebIdsWithDBID
                            .Select(x => Regex.Match(x, @"\d+$", RegexOptions.RightToLeft).Value)
                            .Where(x => !string.IsNullOrEmpty(x));

                var webIdNum = numList != null && numList.Count() > 0 ? numList.Select(x => int.Parse(x)).Max() : 0;
                string regExp = @"\d+$";
                webId = Regex.Replace(webId.Trim(), regExp, "");
                webId = $"{webId.Replace(webIdNum.ToString(), string.Empty)}{webIdNum + 1}";
            }

            return webId;
        }

        public static string MaxWebID(string webId, IEnumerable<string> listOfExistingWebIds)
        {
            var checkIfAllWebIdsInTheListStartWithWebId = listOfExistingWebIds.Select(x => x.ToLower().StartsWith(webId.ToLower())).Any();

            if (checkIfAllWebIdsInTheListStartWithWebId)
            {
                var numList = listOfExistingWebIds
                            .Select(x => Regex.Match(x, @"\d+$", RegexOptions.RightToLeft).Value)
                            .Where(x => !string.IsNullOrEmpty(x));

                var webIdNum = numList != null && numList.Count() > 0 ? numList.Select(x => int.Parse(x)).Max() : 0;
                webId = $"{webId.Replace(webIdNum.ToString(), string.Empty)}{webIdNum}";
            }

            return webId;
        }

        public static string NewAutoWebId(string prefix = "", string postfix = "", int length = 7, bool auto = true)
        {
            var autoString = auto ? Guid.NewGuid().ToString("d").Substring(1, length) : string.Empty;
            return $"{prefix.ToLower()}{autoString}{postfix.ToLower()}";
        }

        public static string NewDefaultPassword(string givenName, string ssn=null, string state=null, string lastName=null)
        {
            var lastFour = string.IsNullOrEmpty(ssn)? null: Regex.Match(ssn, @"\d{4}$", RegexOptions.RightToLeft).Value;

            var firstFour = string.IsNullOrEmpty(lastName) ? null : Regex.Match(lastName, @"\d{4}$").Value;
            var firstCharacterOfGivenName = givenName.Substring(0, 1).ToLower();

            if (lastFour!= null && lastFour=="0000" && state!=null)
                return $"{firstCharacterOfGivenName}{lastFour}{state.ToUpper()}$";
            if (firstFour != null)
                return $"{firstCharacterOfGivenName}{firstFour}#$";

            return $"{firstCharacterOfGivenName}1234Qwer$";
        }
    }
}