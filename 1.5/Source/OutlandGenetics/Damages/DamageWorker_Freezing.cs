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
    public class DamageWorker_Freezing : DamageWorker_Cut
    {
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            if(thing is Pawn pawn && pawn != null && pawn.RaceProps.IsFlesh)
            {
                Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia);
                if(hediff != null)
                {
                    hediff.Severity += 0.15f;
                }
                else
                {
                    pawn.health.AddHediff(HediffDefOf.Hypothermia);
                    pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hypothermia).Severity = 0.15f;
                }
            }
            return base.Apply(dinfo, thing);
        }
    }
}
