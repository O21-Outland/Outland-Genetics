using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace OutlandGenes
{
    [HarmonyPatch(typeof(CompEggContainer), "Accepts")]
    public static class Patch_CompEggContainer_Accepts
    {
        [HarmonyPostfix]
        public static void Postfix(CompEggContainer __instance, ThingDef thingDef, ref bool __result)
        {
            if(thingDef.thingCategories != null && thingDef.thingCategories.Contains(OutlandGenesDefOf.Outland_HumanoidEggs))
            {
                if (!__instance.Empty)
                {
                    __result = false;
                    return;
                }
                __result = true;
                return;
            }
        }
    }
}
