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
    public class ThoughtWorker_FamiliarScent : ThoughtWorker
	{
		public override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
		{
			if (p == null || otherPawn == null) { return false; }
			if (p.genes == null || otherPawn.genes == null) { return false; }
			if (!GeneUtil.SameXenotype(p, otherPawn) || !RelationsUtility.PawnsKnowEachOther(p, otherPawn))
			{
				return false;
			}
			if (otherPawn.genes?.HasGene(OutlandGenesDefOf.Outland_FamiliarScent) ?? false)
			{
				return ThoughtState.ActiveAtStage(1);
			}
			return false;
		}
	}
}
