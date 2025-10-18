using Models.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public class GenericCompare<T> : IEqualityComparer<T> where T : BaseDBModel
    {
        public bool Equals(T x, T y)
        {
            if (x != null && y != null && x.Id == y.Id)
                return true;
            else
                return false;
        }
        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
