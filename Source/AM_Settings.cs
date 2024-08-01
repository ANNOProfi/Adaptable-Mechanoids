using Verse;
using UnityEngine;

namespace AdaptableMechanoids
{
    public class AM_Settings : ModSettings
    {
        public bool hardMode = false;

        public float maxValue = 2f;

        public bool useHeat = false;

        public bool useMax = false;

        public int adaptationTime = 5000;

        public float adaptationStep = 0.001f;

        public bool adaptColonyMech = false;

        public bool adaptAIMech = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hardMode, "hardMode", defaultValue: false);
            Scribe_Values.Look(ref maxValue, "maxValue", defaultValue: 2f);
            Scribe_Values.Look(ref useHeat, "useHeat", defaultValue: false);
            Scribe_Values.Look(ref adaptationTime, "adaptationTime", defaultValue: 5000);
            Scribe_Values.Look(ref adaptationStep, "adaptationStep", defaultValue: 0.001f);
            Scribe_Values.Look(ref useMax, "useMax", defaultValue: false);
            Scribe_Values.Look(ref adaptAIMech, "adaptAIMech", defaultValue: true);
            Scribe_Values.Look(ref adaptColonyMech, "adaptColonyMech", defaultValue: false);
        }
    }
}