using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace OutlandGenes
{
    public class Hediff_EggPouch : Hediff
	{
		public ThingDef curEggDef = null;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref curEggDef, "curEggDef");
        }
    }
}
