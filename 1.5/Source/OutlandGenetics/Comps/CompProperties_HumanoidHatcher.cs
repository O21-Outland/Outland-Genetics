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
    public class CompProperties_HumanoidHatcher : CompProperties
    {
        public CompProperties_HumanoidHatcher()
        {
            compClass = typeof(Comp_HumanoidHatcher);
        }

        public float daysToHatch = 14f;
    }
}
