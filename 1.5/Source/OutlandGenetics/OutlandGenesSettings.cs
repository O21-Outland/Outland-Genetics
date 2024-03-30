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
    public class OutlandGenesSettings : ModSettings
    {
        public bool verboseLogging = false;

        public bool restoredContent = false;
        public bool rc_passionGenes = false;
        public bool rc_addictionGenes = false;
        public bool rc_resurrectGene = false;
        public bool rc_sanguophageRessurect = false;
        public bool extendImpidGenes = false;
        public bool alternateYttakin = false;
        public bool alternateYttakinNoBeards = false;

        public float earnedAscensionExperienceFactor = 1.0f;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref restoredContent, "restoredContent", false);
            Scribe_Values.Look(ref rc_passionGenes, "rc_passionGenes", false);
            Scribe_Values.Look(ref rc_addictionGenes, "rc_addictionGenes", false);
            Scribe_Values.Look(ref rc_resurrectGene, "rc_resurrectGene", false);
            Scribe_Values.Look(ref rc_sanguophageRessurect, "rc_sanguophageRessurect", false);
            Scribe_Values.Look(ref extendImpidGenes, "extendImpidGenes", false);
            Scribe_Values.Look(ref alternateYttakin, "alternateYttakin", false);
            Scribe_Values.Look(ref alternateYttakinNoBeards, "alternateYttakinNoBeards", false);

            Scribe_Values.Look(ref earnedAscensionExperienceFactor, "earnedAscensionExperienceFactor", 1.0f);
        }

        public bool IsValidSetting(string input)
        {
            if (GetType().GetFields().Where(p => p.FieldType == typeof(bool)).Any(i => i.Name == input))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }
        }
    }
}
