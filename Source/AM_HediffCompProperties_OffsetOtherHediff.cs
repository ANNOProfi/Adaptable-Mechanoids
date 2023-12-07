using System.Collections.Generic;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_HediffCompProperties_OffsetOtherHediff : HediffCompProperties
    {
        public List<HediffDef> affectedHediffs = new List<HediffDef>();

        public float offset = 0.0f;

        public bool affectsNewHediffs = true;

        public bool affectsExistingHediffs = false;
    }
}