using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OutlandGenes
{
    public static class LogUtil
    {
        public static readonly Color msgColor = new Color(0.266f, 0.580f, 0.890f);

        public static readonly Color wrnColor = new Color(0.796f, 0.325f, 0.878f);

        public static readonly Color errColor = new Color(0.901f, 0.192f, 0.152f);

        public static readonly string msgPrefix = $":: {OutlandGenesMod.mod.Content.Name} ::";

        public static void LogMessage(string msg)
        {
            Log.Message(msg);
            UnityEngine.Debug.Log(msg);
        }

        public static void LogWarning(string msg)
        {
            Log.Warning(msg);
            UnityEngine.Debug.Log(msg);
        }

        public static void LogError(string msg)
        {
            Log.Error(msg);
            UnityEngine.Debug.Log(msg);
        }
    }
}
