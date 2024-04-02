using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechProg5_8
{
    public class func
    {
        public double Result { get; private set; }
        public bool IsValid { get; private set; }

        public func(double x, double y)
        {
            if (Math.Exp(y) != 0) //в данном случае знаменатель никогда не будет равен 0
            {
                Result = x / Math.Exp(y);
                IsValid = true;
            }
            else
            {
                Result = 0;
                IsValid = false;
            }
        }
    }
}
