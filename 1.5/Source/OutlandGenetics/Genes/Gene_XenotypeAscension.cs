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
    public class Gene_XenotypeAscension : Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            if (!pawn.health.hediffSet.HasHediff(OutlandGenesDefOf.Outland_XenotypeAscender))
            {
                Hediff_XenotypeAscender hediff = (Hediff_XenotypeAscender)HediffMaker.MakeHediff(OutlandGenesDefOf.Outland_XenotypeAscender, pawn, null);
                hediff.targetXenotype = DefDatabase<XenotypeDef>.GetNamed(def.defName.Replace("Outland_XenotypeAscension_", ""));

                pawn.health.AddHediff(hediff);
            }
        }

        public override void PostRemove()
        {
            base.PostRemove();
            if (pawn.health.hediffSet.HasHediff(OutlandGenesDefOf.Outland_XenotypeAscender))
            {
                pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(OutlandGenesDefOf.Outland_XenotypeAscender));
            }
        }
    }
}
