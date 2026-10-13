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
    public class Comp_XenotypeImplanter : CompAbilityEffect
    {
        public new CompProperties_XenotypeImplanter Props => (CompProperties_XenotypeImplanter)props;

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if(target == null || target.Pawn == null)
            {
                return false;
            }
            if (target.Pawn.RaceProps.IsMechanoid)
            {
                return false;
            }
            if (!target.Pawn.RaceProps.Humanlike)
            {
                return false;
            }
            if (!target.Pawn.DevelopmentalStage.HasFlag(DevelopmentalStage.Adult))
            {
                return false;
            }
            if(target.Pawn.genes.Xenotype != null && target.Pawn.genes.Xenotype == Props.xenotype)
            {
                return false;
            }
            return base.CanApplyOn(target, dest);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Pawn pawn = target.Pawn;
            if(pawn != null)
            {
				ImplantXenotype(pawn, Props.xenotype);
                FleckMaker.AttachedOverlay(pawn, FleckDefOf.FlashHollow, new Vector3(0f, 0f, 0.26f));
                if (PawnUtility.ShouldSendNotificationAbout(parent.pawn) || PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    int maxComa = OutlandGenesDefOf.Outland_EndogerminationComa.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
					int maxShock = OutlandGenesDefOf.Outland_EndogermLossShock.CompProps<HediffCompProperties_Disappears>().disappearsAfterTicks.max;
                    Find.LetterStack.ReceiveLetter("LetterLabelXenotypeImplanted".Translate(), "LetterTextGenesImplanted".Translate(parent.pawn.Named("CASTER"), pawn.Named("TARGET"), maxComa.ToStringTicksToPeriod().Named("COMADURATION"), maxShock.ToStringTicksToPeriod().Named("SHOCKDURATION")), LetterDefOf.NeutralEvent, new LookTargets(parent.pawn, pawn));
                }
            }
		}

		public void ImplantXenotype(Pawn pawn, XenotypeDef xenotype)
        {
			pawn.genes.SetXenotype(Props.xenotype);
			pawn.genes.xenotypeName = xenotype.label;
			pawn.genes.ClearXenogenes();
			foreach(GeneDef gene in xenotype.genes)
            {
				pawn.genes.AddGene(gene, xenotype.inheritable);
            }
			pawn.health.AddHediff(OutlandGenesDefOf.Outland_EndogerminationComa);
			parent.pawn.health.AddHediff(OutlandGenesDefOf.Outland_EndogermLossShock);
		}

		public override bool Valid(LocalTargetInfo target, bool throwMessages = false)
		{
			Pawn pawn = target.Pawn;
			if (pawn == null)
			{
				return base.Valid(target, throwMessages);
			}
			if (pawn.IsQuestLodger())
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotImplantInTempFactionMembers".Translate(), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (pawn.HostileTo(parent.pawn) && !pawn.Downed)
			{
				if (throwMessages)
				{
					Messages.Message("MessageCantUseOnResistingPerson".Translate(parent.def.Named("ABILITY")), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (pawn.genes.Xenotype != null && pawn.genes.Xenotype == Props.xenotype)
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotUseOnSameXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			if (!PawnIdeoCanAcceptReimplant(parent.pawn, pawn))
			{
				if (throwMessages)
				{
					Messages.Message("MessageCannotBecomeNonPreferredXenotype".Translate(pawn), pawn, MessageTypeDefOf.RejectInput, historical: false);
				}
				return false;
			}
			return base.Valid(target, throwMessages);
		}

		public bool PawnIdeoCanAcceptReimplant(Pawn implanter, Pawn implantee)
		{
			if (!ModsConfig.IdeologyActive)
			{
				return true;
			}
			if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.PropagateBloodfeederGene, implantee) && Props.xenotype.genes.Contains(GeneDefOf.Bloodfeeder))
			{
				return false;
			}
			if (!IdeoUtility.DoerWillingToDo(HistoryEventDefOf.BecomeNonPreferredXenotype, implantee) && !IsAPreferredXenotype(implantee.Ideo, Props.xenotype))
			{
				return false;
			}
			return true;
		}

		public bool IsAPreferredXenotype(Ideo ideo, XenotypeDef def)
		{
			if (!ideo.PreferredXenotypes.Any() && !ideo.PreferredCustomXenotypes.Any())
			{
				return false;
			}
			if (def.genes == null)
			{
				return false;
			}
			return ideo.PreferredXenotypes.Contains(def);
		}
	}
}
