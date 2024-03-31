using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OutlandGenes
{
    public class OutlandGenesMod : Mod
    {
        public static OutlandGenesMod mod;
        public static OutlandGenesSettings settings;
        public static Harmony harmonyInstance;

        public OutlandGenesSettingsPage currentPage = OutlandGenesSettingsPage.General;
        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        internal static string VersionDir => Path.Combine(mod.Content.ModMetaData.RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public OutlandGenesMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<OutlandGenesSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            File.WriteAllText(VersionDir, CurrentVersion);

            harmonyInstance = new Harmony("Neronix17.Outland.Genes");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Outland - Genetics";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            bool flag = optionsViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            DoOptionsCategoryContents(listing);
            // ======================================================================
            optionsViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {
            listing.Note("You will need to restart the game for most of these settings to take effect.", GameFont.Tiny);
            listing.GapLine();
            listing.CheckboxEnhanced("Enable Restored Content", "If enabled allows the following cut content that still remains in the code to be restored. You can tweak what exactly is restored with the options below. They all have clear reasons for being cut, generally lackluster and easily avoided or just overpowered, so they are disabled by default.", ref settings.restoredContent);
            listing.CheckboxEnhanced("- Passion Genes", "These genes allow manipulation of pawn passions, either removing the passion entirely or increasing it one level.", ref settings.rc_passionGenes);
            listing.CheckboxEnhanced("- Addiction Genes", "On top of the existing genes for addictions, these add a sensitivity variant that makes it easier for pawns with the gene to become addicts.", ref settings.rc_addictionGenes);
            listing.CheckboxEnhanced("- Ressurection Gene", "Adds a gene allowing Hemogenic pawns to resurrect others at a cost of hemogen.", ref settings.rc_resurrectGene);
            listing.CheckboxEnhanced("- Patch Sanguophage", "Patches the above Ressurection gene onto Sanguophages, seems that they were intended to have it.", ref settings.rc_sanguophageRessurect);
            listing.GapLine();
            listing.CheckboxEnhanced("Extended Impids", "Patches the Impid Xenotype to have additional horn variation as well as giving them a tail and chance of having wings.", ref settings.extendImpidGenes);
            listing.CheckboxEnhanced("Less Beards Yttakin", "Removes the gene from Yttakin that forces them to have a beard.", ref settings.alternateYttakin);
            listing.CheckboxEnhanced("No Beards Yttakin", "Adds the gene to Yttakin preventing them from ever having a beard.", ref settings.alternateYttakinNoBeards);
            listing.CheckboxEnhanced("Vanilla Skin Color Edits", "Makes some small changes to vanilla skin colors so they are organised among the added ones appropriately, and makes the yellows fit their name better.", ref settings.editVanillaSkins);
            listing.GapLine();
            listing.SliderLabeled("Earned Ascension Gain Multiplier: " + settings.earnedAscensionExperienceFactor.ToStringPercent(), settings.earnedAscensionExperienceFactor, 0.1f, 5.0f);
        }
    }
}
