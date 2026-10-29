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
    [HarmonyPatch(typeof(Pawn_SkillTracker), "Learn")]
    public static class Patch_Pawn_SkillTracker_Learn
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_SkillTracker __instance, SkillDef sDef, float xp)
        {
            if (xp > 0)
            {
                Hediff_XenotypeAscender hediff = (Hediff_XenotypeAscender)__instance.pawn.health.hediffSet.hediffs.Find(h => h is Hediff_XenotypeAscender);

                if (hediff != null && (__instance.GetSkill(sDef).passion.HasFlag(Passion.Minor) || __instance.GetSkill(sDef).passion.HasFlag(Passion.Major)))
                {
                    hediff.GiveExp(xp * 0.0001f);
                }
            }
        }
    }
}
