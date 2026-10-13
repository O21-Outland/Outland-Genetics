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
	[HarmonyPatch(typeof(Recipe_ExtractOvum), "CompletableEver")]
	public static class Patch_Recipe_ExtractOvum_CompletableEver
	{
		[HarmonyPostfix]
		public static void Postfix(Recipe_ImplantEmbryo __instance, Pawn surgeryTarget, ref bool __result)
		{
			if (__result && (surgeryTarget.genes?.HasGene(OutlandGenesDefOf.Outland_EggLayer) ?? false))
			{
				__result = false;
				return;
			}
		}
	}
}
