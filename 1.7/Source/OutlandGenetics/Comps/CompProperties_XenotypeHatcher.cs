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
    public class CompProperties_XenotypeHatcher : CompProperties
    {
        public CompProperties_XenotypeHatcher()
        {
            compClass = typeof(Comp_XenotypeHatcher);
        }

        public PawnKindDef hatcherPawn;

        public XenotypeDef hatcherXenotype;

        public float hatcherDaystoHatch = 1f;
    }
}
