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
	[HarmonyPatch(typeof(Hediff_Pregnant), "Tick")]
	public static class Patch_Hediff_Pregnant_Tick
	{
		[HarmonyPrefix]
		public static bool Prefix(Hediff_Pregnant __instance)
		{
			if ((Find.TickManager.TicksAbs % 200 == 0) && (__instance?.pawn?.genes?.HasGene(OutlandGenesDefOf.Outland_EggLayer) ?? false))
			{
				try
				{
                    if (!__instance.pawn.genes.GetGene(OutlandGenesDefOf.Outland_EggLayer).Active)
                    {
						return true;
                    }
					float progressSpeed = GeneUtil.GetPregnancySpeed(__instance.pawn);
					// Lay the egg.
					Thing egg = LayHumanEgg(__instance.pawn, __instance.father, __instance.geneSet);
					GenSpawn.Spawn(egg, __instance.pawn.Position, __instance.pawn.Map);
					Comp_HumanoidHatcher comp = egg.TryGetComp<Comp_HumanoidHatcher>();
					if(comp != null)
                    {
						if(__instance.pawn.genes.GenesListForReading.Any(g => g.def.defName.Contains("Outland_XenoReproduction")))
                        {
							List<XenotypeDef> xenotypes = new List<XenotypeDef>();
							foreach(Gene gene in __instance.pawn.genes.GenesListForReading)
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
								comp.potentialXenotypes = xenotypes;
							}
                        }
						comp.progressSpeed = progressSpeed;
                    }
					// Make temporarily sterile.
					Hediff hediff = HediffMaker.MakeHediff(OutlandGenesDefOf.Outland_EggFatigue, __instance.pawn);
					__instance.pawn.health.AddHediff(hediff);
					// Tell the player.
					Find.LetterStack.ReceiveLetter("Outland.EggLaidLabel".Translate(__instance.pawn.NameShortColored), "Outland.EggLaidDesc".Translate(__instance.pawn.NameShortColored), LetterDefOf.PositiveEvent, (TargetInfo)__instance.pawn);
					__instance.pawn.health.RemoveHediff(__instance);
				}
				catch (Exception ex)
				{
					Log.Message("Error Suppressed: " + ex.ToString());
				}
				return false;
			}
			return true;
		}

		public static Thing LayHumanEgg(Pawn mother, Pawn father, GeneSet geneSet)
		{
			Thing thing;
			thing = ThingMaker.MakeThing(OutlandGenesDefOf.Outland_HumanoidEgg);
			Comp_HumanoidHatcher comp = thing.TryGetComp<Comp_HumanoidHatcher>();
			comp.mother = mother;
			if (mother?.genes?.xenotype == father?.genes?.xenotype)
			{
				comp.xenotype = mother.genes.xenotype;
			}
			else
			{
				comp.xenotype = XenotypeDefOf.Baseliner;
			}
			if (father != null)
			{
				comp.father = father;
			}
			comp.geneSet = geneSet;

			return thing;
		}
	}
}
