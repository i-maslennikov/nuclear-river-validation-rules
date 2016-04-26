using System;

namespace NuClear.Replication.Core
{
    public static class TypeExtensions
    {
        public static string GetFriendlyName(this Type type)
        {
            var friendlyName = type.Name;
            if (type.IsGenericType)
            {
                var backtickIndex = friendlyName.IndexOf('`');
                if (backtickIndex > 0)
                {
                    friendlyName = friendlyName.Remove(backtickIndex);
                }

                friendlyName += "[";

                var typeParameters = type.GetGenericArguments();
                for (var i = 0; i < typeParameters.Length; ++i)
                {
                    var typeParamName = typeParameters[i].FullName;
                    var commaIndex = typeParamName.IndexOf(',');
                    if (commaIndex > 0)
                    {
                        typeParamName = typeParamName.Remove(commaIndex);
                    }

                    friendlyName += i == 0 ? typeParamName : "," + typeParamName;
                }

                friendlyName += "]";
            }

            return friendlyName;
        }
    }
}