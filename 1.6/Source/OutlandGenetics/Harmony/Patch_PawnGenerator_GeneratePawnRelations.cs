using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace OutlandGenes
{
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawnRelations")]
    public static class Patch_PawnGenerator_GeneratePawnRelations
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn)
        {
            if(pawn.GeneActive(OutlandGenesDefOf.Outland_AllMale) || pawn.GeneActive(OutlandGenesDefOf.Outland_AllFemale))
            {
                return false;
            }
            return true;
        }
    }
}
