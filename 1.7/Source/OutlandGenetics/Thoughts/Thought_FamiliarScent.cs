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
    public class Thought_FamiliarScent : Thought_SituationalSocial
	{
		public override float OpinionOffset()
		{
			return GeneUtil.SameXenotype(pawn, OtherPawn()) ? (OtherPawn().genes.HasGene(OutlandGenesDefOf.Outland_FamiliarScent) ? 20 : 0) : 0;
		}
	}
}
