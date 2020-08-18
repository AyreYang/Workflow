using System;
using System.Collections.Generic;

namespace DataBase.common.extensions
{
    public static class Extensions
    {
        public static Type GetListItemType<T>(this IList<T> list)
        {
            return typeof(T);
        }
    }
}
