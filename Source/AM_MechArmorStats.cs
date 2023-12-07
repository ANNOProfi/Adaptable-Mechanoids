

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_MechArmorStats
    {
        private float armor_Blunt = 0f;

        public float Armor_Blunt{get => armor_Blunt; set => armor_Blunt = value;}

        private float armor_Sharp = 0f;

        public float Armor_Sharp{get => armor_Sharp; set => armor_Sharp = value;}

        private float armor_Heat = 0f;

        public float Armor_Heat{get => armor_Heat; set => armor_Heat = value;}

        private bool isRegistered = false;

        public bool IsRegistered{get => isRegistered; set => isRegistered = value;}

        private float amor_total
        {
            get
            {
                return Armor_Blunt + Armor_Sharp + Armor_Heat;
            }
        }

        public Dictionary<DamageArmorCategoryDef, int> damageInstances;

        public Dictionary<DamageArmorCategoryDef, float> damageAmounts;

        private string defName = null;

        public string DefName{get => defName; set => defName = value;}

        public AM_MechArmorStats(float blunt, float sharp, float heat, string name)
        {
            Armor_Blunt = blunt;

            Armor_Sharp = sharp;

            Armor_Heat = heat;

            DefName = name;

            damageInstances.Add(AM_DefOf.Blunt, 0);
            damageInstances.Add(DamageArmorCategoryDefOf.Sharp, 0);
            damageInstances.Add(AM_DefOf.Heat, 0);

            damageAmounts.Add(AM_DefOf.Blunt, 0);
            damageAmounts.Add(DamageArmorCategoryDefOf.Sharp, 0);
            damageAmounts.Add(AM_DefOf.Heat, 0);
        }
    }
}