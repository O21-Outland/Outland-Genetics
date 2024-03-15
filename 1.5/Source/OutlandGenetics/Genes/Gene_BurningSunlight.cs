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
    public class Gene_BurningSunlight : Gene
    {
        public bool HasApparelCoverage => pawn.apparel.BodyPartGroupIsCovered(BodyPartGroupDefOf.Torso) && ((pawn.apparel?.BodyPartGroupIsCovered(BodyPartGroupDefOf.UpperHead) ?? false) || (pawn.apparel?.BodyPartGroupIsCovered(BodyPartGroupDefOf.FullHead) ?? false));

        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksAbs % 600 == 0 && pawn.Spawned && pawn.Position.InSunlight(pawn.Map) && pawn.CanEverAttachFire() && !HasApparelCoverage)
            {
                pawn.TryAttachFire(0.3f, null);
            }
        }


    }
}
