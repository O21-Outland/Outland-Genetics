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
	[HarmonyPatch(typeof(HumanEmbryo), "CanImplantReport")]
	public static class Patch_HumanEmbryo_CanImplantReport
	{
		[HarmonyPostfix]
		public static void Postfix(Pawn pawn, ref AcceptanceReport __result)
		{
			if (pawn.genes?.HasGene(OutlandGenesDefOf.Outland_EggLayer) ?? false)
			{
				__result = "Outland.CannotEggLayer".Translate();
			}
		}
	}
}
