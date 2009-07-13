#region Description
//-----------------------------------------------------------------------------
// File:        Group.cs
// Author:      Ligang Wang
// Email:       ligang@dingn.com
// Date:        01/31/2009
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Knoics.Math
{
    public class Group
    {
        public static void Cycle<TKey, TValue>(TKey[] cycle, Func<TKey, TValue> get, Func<TKey, TValue, bool> set)
        {
            if(cycle.Count() <2) return;
            int count = cycle.Count();
            TValue from = get(cycle[count - 1]);
            if (from == null) Debug.Assert(false);
            for (int i = 0; i < count; i++)
            {
                TValue save = get(cycle[i]);
                if (save == null) Debug.Assert(false);

                if (!set(cycle[i], from))
                    Debug.Assert(false);
                from = save;
            }
        }
    }
}
