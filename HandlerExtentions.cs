using SRMP.Networking.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMP
{
    internal static class HandlerExtentions
    {
        public static void BeginHandle(this GameObject obj) => obj.AddComponent<HandledDummy>();

        public static void EndHandle(this GameObject obj) => obj.RemoveComponent<HandledDummy>();

        public static bool IsHandling(this GameObject obj) => obj.GetComponent<HandledDummy>() != null;
        public static bool IsHandling(this Component obj) => obj.GetComponent<HandledDummy>() != null;
    }
}
