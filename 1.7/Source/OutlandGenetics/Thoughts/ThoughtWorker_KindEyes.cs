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
    public class ThoughtWorker_KindEyes : ThoughtWorker
	{
		public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
		{
			if(otherPawn == null) { return false; }
			if(otherPawn.genes == null) { return false; }
			if (!RelationsUtility.PawnsKnowEachOther(p, otherPawn))
			{
				return false;
			}
			if (otherPawn?.genes?.HasGene(OutlandGenesDefOf.Outland_KindEyes) ?? false)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return false;
		}
	}
}
