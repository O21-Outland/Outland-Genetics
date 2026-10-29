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
    [HarmonyPatch(typeof(HealthCardUtility), "DrawHediffRow")]
    public class Patch_HealthCardUtility_HediffDrawRow
    {
        [HarmonyPostfix]
        public static void Postfix(Rect rect, Pawn pawn, IEnumerable<Hediff> diffs, ref float curY)
        {
            Hediff_XenotypeAscender hediff = (Hediff_XenotypeAscender)diffs.ToList().Find(h => h is Hediff_XenotypeAscender);

            if (hediff != null)
            {
                Rect card = new Rect(0f, curY, rect.width, 0f);

                float textHeight = 20f;
                card.height = 8f + (hediff.HasAnyEarnedAscensionGene ? textHeight + 18f : 0f) + (hediff.PotentialXenotypes.Count > 1 ? textHeight : 0f) + ((Prefs.DevMode && DebugSettings.godMode) ? textHeight : 0f);

                Widgets.DrawMenuSection(card);

                GUI.BeginGroup(card);
                AscenderUtil.DrawHediffCard(card, pawn, hediff, textHeight);
                GUI.EndGroup();
                curY += card.height;
            }
        }
    }
}
