using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enterprise.Lib.DataAccess
{
    public static class DbUtility
    {
        public static string GetWhereMatchAnyNotNull(object obj)
        {
            var wherClauseStr = new StringBuilder();
            var props = obj?.GetType().GetProperties();

            if (props != null && props.Count() > 0)
            {
                foreach (var prop in props)
                {
                    if (wherClauseStr.ToString().Trim() != string.Empty)
                        wherClauseStr.Append($" AND ");

                    var str = $"(@{prop.Name} IS NULL OR {prop.Name} = @{prop.Name})";

                    wherClauseStr.Append(str);
                }
            }

            return wherClauseStr.ToString();
        }
    }
}
