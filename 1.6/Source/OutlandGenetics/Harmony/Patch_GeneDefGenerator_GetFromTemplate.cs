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
    [HarmonyPatch(typeof(GeneDefGenerator), "GetFromTemplate")]
    public static class Patch_GeneDefGenerator_GetFromTemplate
    {
        [HarmonyPostfix]
        public static void Postfix(AscensionGeneTemplateDef template, Def def, int displayOrderBase, GeneDef __result)
        {
            // Simple fix for modExtensions not being copied through templates.
            if(!template.modExtensions.NullOrEmpty() && __result.modExtensions.NullOrEmpty())
            {
                __result.modExtensions = template.modExtensions;
            }
        }
    }
}
