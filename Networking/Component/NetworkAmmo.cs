using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP.Networking.Component
{
    public class NetworkAmmo : Ammo
    {
        internal static Dictionary<string, Ammo> all = new Dictionary<string, Ammo>();
        public string ammoId;
        public NetworkAmmo(string id, HashSet<Identifiable.Id> potentialAmmo, int numSlots, int usableSlots, Predicate<Identifiable.Id>[] slotPreds, Func<Identifiable.Id, int, int> slotMaxCountFunction) : base(potentialAmmo, numSlots, usableSlots, slotPreds, slotMaxCountFunction)
        {
            ammoId = id;
            all.Add(ammoId, this);
        }
    }
}
