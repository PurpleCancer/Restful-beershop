using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeerShop.Models
{
    public class Locks
    {
        public static object orderLock = new object();
    }
}
