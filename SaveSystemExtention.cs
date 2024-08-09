using SRML.SR.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMP
{
    public static class SaveSystemExtention
    {
        public static CompoundDataPiece GetOrCreateEmpty(this CompoundDataPiece data, string key)
        {
            CompoundDataPiece value;
            if (data.HasPiece(key))
            {
                value = data.GetCompoundPiece(key);
                value.DataList.Clear();
            }
            else
                data.AddPiece(value = new CompoundDataPiece(key));
            return value;
        }
    }
}
