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
    public class Comp_GermlineReimplanter : CompAbilityEffect
    {
        public new CompProperties_GermlineReimplanter Props => (CompProperties_GermlineReimplanter)props;

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
            return base.CanApplyOn(target, dest);
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            Hediff_Implanted hediff = (Hediff_Implanted)target.Pawn.health.AddHediff(Props.hediff);
        }

        public override void PostApplied(List<LocalTargetInfo> targets, Map map)
        {
            base.PostApplied(targets, map);
            Hediff_Implanted hediff = (Hediff_Implanted)targets.Where(p => p.Pawn != null).First().Pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff);
            hediff.SetParents(parent.pawn, null, PreggoUtil.GetInheritedGeneSet(parent.pawn));
            hediff.progressSpeed = GeneUtil.GetPregnancySpeed(parent.pawn);
        }
    }
}
