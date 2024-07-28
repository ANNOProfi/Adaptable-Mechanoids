using Verse;

namespace AdaptableMechanoids
{
    public class AM_Settings : ModSettings
    {
        public bool hardMode = false;

        public float maxValue = 2f;

        public bool useHeat = false;

        public int adaptationTime = 5000;

        public float adaptationStep = 0.001f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hardMode, "hardMode", defaultValue: false);
            Scribe_Values.Look(ref maxValue, "maxValue", defaultValue: 2f);
            Scribe_Values.Look(ref useHeat, "useHeat", defaultValue: false);
            Scribe_Values.Look(ref adaptationTime, "adaptationTime", defaultValue: 5000);
            Scribe_Values.Look(ref adaptationStep, "adaptationStep", defaultValue: 0.001f);
        }
    }
}