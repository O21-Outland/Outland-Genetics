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
    public class CompProperties_GermlineReimplanter : CompProperties_AbilityEffect
    {
        public CompProperties_GermlineReimplanter()
        {
            compClass = typeof(Comp_GermlineReimplanter);
        }

        public HediffDef hediff;
    }
}
