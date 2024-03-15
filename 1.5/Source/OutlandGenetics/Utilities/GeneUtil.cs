using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using VanillaGenesExpanded;

namespace OutlandGenes
{
    [StaticConstructorOnStartup]
    public static class GeneUtil
	{
		public static bool GeneActive(this Pawn pawn, GeneDef geneDef)
		{
			if (pawn.genes != null)
			{
				return pawn.genes.GetGene(geneDef)?.Active ?? false;
			}
			return false;
		}

		public static float GetPregnancySpeed(Pawn pawn)
        {
			float pregnancySpeed = 1f;

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene.Active)
                {
					GeneExtension ext = gene.def.GetModExtension<GeneExtension>();
					if(ext != null && ext.pregnancySpeedFactor != 1f)
                    {
						pregnancySpeed *= ext.pregnancySpeedFactor;
                    }
                }
            }

			return pregnancySpeed;
        }

		public static void ReimplantXenogerm(Pawn caster, Pawn recipient)
		{
			if (!ModLister.CheckBiotech("xenogerm reimplantation"))
			{
				return;
			}
			QuestUtility.SendQuestTargetSignals(caster.questTags, "XenogermReimplanted", caster.Named("SUBJECT"));
			recipient.genes.SetXenotype(caster.genes.Xenotype);
			recipient.genes.xenotypeName = caster.genes.xenotypeName;
			recipient.genes.xenotypeName = caster.genes.xenotypeName;
			recipient.genes.iconDef = caster.genes.iconDef;
			recipient.genes.ClearXenogenes();
			foreach (Gene gene in caster.genes.GenesListForReading)
			{
				recipient.genes.AddGene(gene.def, xenogene: true);
			}
			if (!caster.genes.Xenotype.soundDefOnImplant.NullOrUndefined())
			{
				caster.genes.Xenotype.soundDefOnImplant.PlayOneShot(SoundInfo.InMap(recipient));
			}
			recipient.health.AddHediff(OutlandGenesDefOf.Outland_EndogermReplicating);
			ExtractGermline(caster);
			UpdateGermlineReplication(recipient);
		}

		public static void ExtractGermline(Pawn pawn, int overrideDurationTicks = -1)
		{
			if (ModLister.CheckBiotech("xenogerm extraction"))
			{
				pawn.health.AddHediff(OutlandGenesDefOf.Outland_EndogermLossShock);
				if (PawnWouldDieFromReimplanting(pawn))
				{
					pawn.genes.SetXenotype(XenotypeDefOf.Baseliner);
				}
				Hediff hediff = HediffMaker.MakeHediff(OutlandGenesDefOf.Outland_EndogermReplicating, pawn);
				if (overrideDurationTicks > 0)
				{
					hediff.TryGetComp<HediffComp_Disappears>().ticksToDisappear = overrideDurationTicks;
				}
				pawn.health.AddHediff(hediff);
			}
		}

		public static bool PawnWouldDieFromReimplanting(Pawn pawn)
		{
			if (!ModsConfig.BiotechActive)
			{
				return false;
			}
			return pawn.health.hediffSet.HasHediff(OutlandGenesDefOf.Outland_EndogermReplicating);
		}

		public static void UpdateGermlineReplication(Pawn pawn)
		{
			if (ModsConfig.BiotechActive)
			{
				Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(OutlandGenesDefOf.Outland_EndogermReplicating);
				if (firstHediffOfDef != null)
				{
					pawn.health.RemoveHediff(firstHediffOfDef);
				}
				pawn.health.AddHediff(OutlandGenesDefOf.Outland_EndogermReplicating);
			}
		}
		public static bool SameXenotype(Pawn pawn, Pawn other)
		{
            try
			{
				if ((pawn?.genes?.GenesListForReading.NullOrEmpty() ?? false) || (other?.genes?.GenesListForReading.NullOrEmpty() ?? false))
				{
					return false;
				}
				if (pawn?.genes?.Xenotype == other?.genes?.Xenotype)
				{
					return true;
				}
				if (pawn?.genes?.GenesListForReading?.Union(other?.genes?.GenesListForReading).EnumerableNullOrEmpty() ?? false)
				{
					return true;
				}
			}
			catch
            {
				return false;
            }
			return false;
		}
	}
}
