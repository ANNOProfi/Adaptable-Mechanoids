using System.Collections.Generic;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_HediffCompProperties_OffsetOtherHediff : HediffCompProperties
    {
        public AM_HediffCompProperties_OffsetOtherHediff()
        {
            this.compClass = typeof(AM_HediffComp_OffsetOtherHediff);
        }
        public List<HediffDef> affectedHediffs = new List<HediffDef>();

        public float offset = 0.0f;

        public bool affectsNewHediffs = false;

        public bool affectsExistingHediffs = true;
    }
}