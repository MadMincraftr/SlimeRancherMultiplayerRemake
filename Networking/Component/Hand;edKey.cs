using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP.Networking.Component
{
    public class HandledKey : MonoBehaviour
    {
        internal static bool collected = false;

        private static float timer;

        public static void StartTimer()
        {
            collected = true;
            timer = 0.075f;
        }

        void Update()
        {
            if (!collected) return;

            timer -= Time.deltaTime;

            if (timer < 0)
            {
                collected = false;
            }
        }
    }
}
