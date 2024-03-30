using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;
using System.Reflection;

namespace OutlandGenes
{
    [HarmonyPatch(typeof(GeneUtility), "SameHeritableXenotype")]
	public static class Patch_GeneUtility_SameHeritableXenotype
    {
        [HarmonyPostfix]
		public static void Postfix(Pawn pawn1, Pawn pawn2, ref bool __result)
		{
            if (pawn1 != null && pawn1.genes != null)
            {
                if (pawn1.genes.GenesListForReading.Any(g => g.def.defName.Contains("Outland_XenoReproduction")))
                {
                    __result = true;
                }
            }
        }
	}
}
