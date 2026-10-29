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
    public class Comp_AscendTarget : CompAbilityEffect
    {
        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
        {
            if (target == null || target.Pawn == null)
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
            if (!target.Pawn.health?.hediffSet?.HasHediff(OutlandGenesDefOf.Outland_XenotypeAscender) ?? false)
            {
                return false;
            }
            Hediff_XenotypeAscender hediff = (Hediff_XenotypeAscender)target.Pawn.health.hediffSet.GetFirstHediffOfDef(OutlandGenesDefOf.Outland_XenotypeAscender);
            if (!hediff.CanAscend)
            {
                return false;
            }
            return base.CanApplyOn(target, dest);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            Hediff_XenotypeAscender hediff = (Hediff_XenotypeAscender)target.Pawn.health.hediffSet.GetFirstHediffOfDef(OutlandGenesDefOf.Outland_XenotypeAscender);

            if (hediff.CanAscend)
            {
                hediff.Ascend();
            }
            else
            {
                Messages.Message("Cannot Ascend yet, not enough experience.", MessageTypeDefOf.RejectInput);
            }
        }
    }
}
