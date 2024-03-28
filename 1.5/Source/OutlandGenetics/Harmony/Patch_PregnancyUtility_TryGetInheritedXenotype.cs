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
	[HarmonyPatch(typeof(PregnancyUtility), "TryGetInheritedXenotype")]
	public static class Patch_PregnancyUtility_TryGetInheritedXenotype
	{
		[HarmonyPostfix]
		public static void Postfix(ref bool __result, Pawn mother, Pawn father, ref XenotypeDef xenotype)
		{
            if (mother != null && mother.genes != null)
            {
                if (mother.genes.GenesListForReading.Any(g => g.def.defName.Contains("Outland_XenoReproduction")))
                {
                    List<XenotypeDef> xenotypes = new List<XenotypeDef>();
                    foreach (Gene gene in mother.genes.GenesListForReading)
                    {
                        DefModExt_Xenotype modExt = gene.def.GetModExtension<DefModExt_Xenotype>();
                        if (modExt != null)
                        {
                            if (!xenotypes.Contains(modExt.xenotypeDef))
                            {
                                xenotypes.Add(modExt.xenotypeDef);
                            }
                        }
                    }
                    if (!xenotypes.NullOrEmpty())
                    {
                        xenotype = (xenotypes.RandomElement());
                        __result = true;
                    }
                }
            }
        }
	}
}
