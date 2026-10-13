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
    [StaticConstructorOnStartup]
	public static class Patch_PregnancyUtility_TryGetInheritedXenotype
	{
        static Patch_PregnancyUtility_TryGetInheritedXenotype()
        {
            MethodBase tryGetInheritedXenotypeInfo = AccessTools.Method(typeof(PregnancyUtility), "TryGetInheritedXenotype");
            HarmonyMethod postfixInfo = AccessTools.Method(typeof(Patch_PregnancyUtility_TryGetInheritedXenotype), "Postfix");
            OutlandGenesMod.harmonyInstance.Patch(tryGetInheritedXenotypeInfo, null, postfixInfo, null, null);
        }

		public static void Postfix(Pawn mother, Pawn father, ref XenotypeDef xenotype, ref bool __result)
		{
            LogUtil.LogMessage("Attempting Inherited Xenotype Patch");
            if (mother != null && mother.genes != null)
            {
                if (mother.genes.GenesListForReading.Any(g => g.def.defName.Contains("Outland_XenoReproduction")))
                {
                    List<XenotypeDef> xenotypes = new List<XenotypeDef>();
                    foreach (Gene gene in mother.genes.GenesListForReading.Where(g => g.def.defName.Contains("Outland_XenoReproduction")))
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
                        LogUtil.LogMessage("Successful Inherited Xenotype Patch");
                    }
                }
            }
        }
	}
}
