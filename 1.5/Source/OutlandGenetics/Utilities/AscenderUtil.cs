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
    [StaticConstructorOnStartup]
    public static class AscenderUtil
    {
        public static readonly Texture2D FillGrey = SolidColorMaterials.NewSolidColorTexture(new ColorInt(60, 60, 60).ToColor);
        public static readonly Texture2D FillBlue = SolidColorMaterials.NewSolidColorTexture(new ColorInt(105, 185, 200).ToColor);

        public static void DrawHediffCard(Rect rect, Pawn pawn, Hediff_XenotypeAscender hediff, float textHeight)
        {
            float curY = 2f;
            float curExp = hediff.currentExp;

            string levelInfo = pawn.genes.XenotypeLabelCap + " / Experience: " + curExp.ToStringPercent();
            Vector2 vec = Text.CalcSize(levelInfo);
            Rect infoContainer = new Rect(0f, curY, rect.width, textHeight);
            Rect levelInfoRect = new Rect((infoContainer.width / 2) - (vec.x / 2f), curY + 2f, vec.x, textHeight);
            curY += infoContainer.height + 2f;

            Rect fillRectTotal = new Rect(0f, curY, rect.width, 18f).ContractedBy(3f);
            Rect expBar = fillRectTotal;
            curY += fillRectTotal.ExpandedBy(3f).height;

            GUI.color = new ColorInt(138, 229, 226).ToColor;
            Widgets.Label(levelInfoRect, levelInfo);
            GUI.color = Color.white;
            Widgets.FillableBar(expBar, curExp, FillBlue, FillGrey, false);
            if (Prefs.DevMode && DebugSettings.godMode && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                if (hediff.CanAscend)
                {
                    string ascend = "DEV: Ascend to " + hediff.targetXenotype.LabelCap;
                    Vector2 vec2 = Text.CalcSize(ascend);
                    Rect rect2 = new Rect(2f, curY, rect.width - 4f, textHeight);
                    if (Widgets.ButtonText(rect2, ascend, true, false, true))
                    {
                        hediff.Ascend();
                    }
                    curY += rect2.height;
                }
                else
                {
                    string ascend = "DEV: Max Experience";
                    Vector2 vec2 = Text.CalcSize(ascend);
                    Rect rect2 = new Rect(2f, curY, rect.width - 4f, textHeight);
                    if (Widgets.ButtonText(rect2, ascend, true, false, true))
                    {
                        hediff.GiveExp(1);
                    }
                    curY += rect2.height;
                }
            }


        }
    }
}
