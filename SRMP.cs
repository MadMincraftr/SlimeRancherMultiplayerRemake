using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP
{
    public class SRMP
    {
        public static void Log(string message)
        {
            SRML.Console.Console.Instance.Log(message);
        }
    }
}
