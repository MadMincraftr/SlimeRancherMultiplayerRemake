using MonomiPark.SlimeRancher.Persist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking.SaveModels
{
    public class NetworkV01 : Persistable
    {
        public bool sharedMoney;

        public bool sharedKeys;
        
        public bool sharedUpgrades;

        public void Load(Stream stream)
        {
            throw new NotImplementedException();
        }

        public long Write(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
