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
            if(hediff.HasAnyEarnedAscensionGene)
            {
                float curExp = hediff.currentExp;
                if (curExp < 1f)
                {
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
                }
                else
                {
                    string readyToAscend = "OutlandGenes.ReadyToAscend".Translate();
                    Vector2 vec = Text.CalcSize(readyToAscend);
                    Rect infoContainer = new Rect(0f, curY, rect.width, textHeight);
                    Rect levelInfoRect = new Rect((infoContainer.width / 2) - (vec.x / 2f), curY + 2f, vec.x, textHeight);
                    curY += infoContainer.height + 2f;
                    GUI.color = new ColorInt(138, 229, 226).ToColor;
                    Widgets.Label(levelInfoRect, readyToAscend);
                    GUI.color = Color.white;
                }
            }

            if(hediff.PotentialXenotypes.Count > 1)
            {
                string ascensionSelection = "OutlandGenes.SelectXenotype".Translate();
                if(hediff.targetXenotype != null)
                {
                    ascensionSelection = "OutlandGenes.SelectedXenotype".Translate(hediff.targetXenotype.LabelCap);
                }
                Rect rect2 = new Rect(2f, curY, rect.width - 4f, textHeight);
                if(Widgets.ButtonText(rect2, ascensionSelection, true, false, true))
                {
                    Find.WindowStack.Add(new FloatMenu(hediff.GetXenotypePotentialTargets().ToList()));
                }
                curY += rect2.height;
            }
            if (Prefs.DevMode && DebugSettings.godMode && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                if (hediff.CanAscend)
                {
                    string ascend = "DEV: Ascend Now";
                    Rect rect3 = new Rect(2f, curY, rect.width - 4f, textHeight);
                    if (Widgets.ButtonText(rect3, ascend, true, false, true))
                    {
                        hediff.Ascend();
                    }
                    curY += rect3.height;
                }
                else
                {
                    string ascend = "DEV: Max Experience";
                    Rect rect4 = new Rect(2f, curY, rect.width - 4f, textHeight);
                    if (Widgets.ButtonText(rect4, ascend, true, false, true))
                    {
                        hediff.GiveExp(1);
                    }
                    curY += rect4.height;
                }
            }


        }
    }
}
