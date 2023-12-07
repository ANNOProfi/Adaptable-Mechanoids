

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_MechArmorStats
    {
        /*public float Armor_Blunt{get => CalculateArmor(armorValues[AM_DefOf.Blunt], AM_DefOf.Blunt);}

        public float Armor_Sharp{get => CalculateArmor(armorValues[DamageArmorCategoryDefOf.Sharp], DamageArmorCategoryDefOf.Sharp);}

        public float Armor_Heat{get => CalculateArmor(armorValues[AM_DefOf.Heat], AM_DefOf.Heat);}*/

        private float armor_total
        {
            get
            {
                return armorValues[AM_DefOf.Blunt] + armorValues[DamageArmorCategoryDefOf.Sharp] + armorValues[AM_DefOf.Heat];
            }
        }

        private float damageAmountsTotal
        {
            get
            {
                return damageAmounts[AM_DefOf.Blunt] + damageAmounts[DamageArmorCategoryDefOf.Sharp] + damageAmounts[AM_DefOf.Heat]; 
            }
        }

        private float armor_totalOriginal = 0.0f;

        public Dictionary<DamageArmorCategoryDef, float> armorValues = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> armorOffsets = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> damageAmounts = new Dictionary<DamageArmorCategoryDef, float>();

        private string defName = null;

        public string DefName{get => defName; set => defName = value;}

        public AM_MechArmorStats(float blunt, float sharp, float heat, string name)
        {
            armorValues.Add(AM_DefOf.Blunt, blunt);
            armorValues.Add(DamageArmorCategoryDefOf.Sharp, sharp);
            armorValues.Add(AM_DefOf.Heat, heat);

            armorOffsets.Add(AM_DefOf.Blunt, 0f);
            armorOffsets.Add(DamageArmorCategoryDefOf.Sharp, 0f);
            armorOffsets.Add(AM_DefOf.Heat, 0f);

            armor_totalOriginal = armorValues[AM_DefOf.Blunt] + armorValues[DamageArmorCategoryDefOf.Sharp] + armorValues[AM_DefOf.Heat];

            DefName = name;

            damageAmounts.Add(AM_DefOf.Blunt, 0f);
            damageAmounts.Add(DamageArmorCategoryDefOf.Sharp, 0f);
            damageAmounts.Add(AM_DefOf.Heat, 0f);
        }

        public void CalculateArmor(/*float armorValue, DamageArmorCategoryDef armorType*/)
        {
            /*Log.Message("Calculating new ArmorValue from "+armorValue +" and "+damageAmounts[armorType] +" damage for "+DefName);
            if(damageAmounts[armorType] == 0)
            {
                Log.Message("Setting ArmorOffset to 0");
                return 0;
            }

            float armorNewValue = (armor_total*(damageAmounts[armorType]/(damageAmountsTotal))) - armorValues[armorType];

            Log.Message("Setting " + armorType.defName + " to " + armorNewValue);
            return armorNewValue;*/

            Log.Message("Calculating new ArmorOffset for "+DefName);

            float damagePerArmorBlunt = damageAmounts[AM_DefOf.Blunt]/(armorValues[AM_DefOf.Blunt]*100);

            float damagePerArmorSharp = damageAmounts[DamageArmorCategoryDefOf.Sharp]/(armorValues[DamageArmorCategoryDefOf.Sharp]*100);

            float damagePerArmorHeat = damageAmounts[AM_DefOf.Heat]/(armorValues[AM_DefOf.Heat]*100);

            Dictionary<DamageArmorCategoryDef, float> damageSort = new Dictionary<DamageArmorCategoryDef, float>();

            damageSort.Add(AM_DefOf.Blunt, damagePerArmorBlunt);
            damageSort.Add(DamageArmorCategoryDefOf.Sharp, damagePerArmorSharp);
            damageSort.Add(AM_DefOf.Heat, damagePerArmorHeat);

            if(damageSort[AM_DefOf.Blunt] < damageSort[DamageArmorCategoryDefOf.Sharp])
            {
                armorOffsets[DamageArmorCategoryDefOf.Sharp] += (damageSort[DamageArmorCategoryDefOf.Sharp] - damageSort[AM_DefOf.Blunt])/100;
                armorOffsets[AM_DefOf.Blunt] -= (damageSort[DamageArmorCategoryDefOf.Sharp] - damageSort[AM_DefOf.Blunt])/100;
                if(armorOffsets[AM_DefOf.Blunt] < -armorValues[AM_DefOf.Blunt])
                {
                    armorOffsets[AM_DefOf.Blunt] = -armorValues[AM_DefOf.Blunt];
                }
            }
            else if(damageSort[AM_DefOf.Blunt] > damageSort[DamageArmorCategoryDefOf.Sharp])
            {
                armorOffsets[AM_DefOf.Blunt] += (damageSort[AM_DefOf.Blunt] - damageSort[DamageArmorCategoryDefOf.Sharp])/100;
                armorOffsets[DamageArmorCategoryDefOf.Sharp] -= (damageSort[AM_DefOf.Blunt] - damageSort[DamageArmorCategoryDefOf.Sharp])/100;
                if(armorOffsets[DamageArmorCategoryDefOf.Sharp] < -armorValues[DamageArmorCategoryDefOf.Sharp])
                {
                    armorOffsets[DamageArmorCategoryDefOf.Sharp] = -armorValues[DamageArmorCategoryDefOf.Sharp];
                }
            }

            if(damageSort[AM_DefOf.Blunt] < damageSort[AM_DefOf.Heat])
            {
                armorOffsets[AM_DefOf.Heat] += (damageSort[AM_DefOf.Heat] - damageSort[AM_DefOf.Blunt])/100;
                armorOffsets[AM_DefOf.Blunt] -= (damageSort[AM_DefOf.Heat] - damageSort[AM_DefOf.Blunt])/100;
                if(armorOffsets[AM_DefOf.Blunt] < -armorValues[AM_DefOf.Blunt])
                {
                    armorOffsets[AM_DefOf.Blunt] = -armorValues[AM_DefOf.Blunt];
                }
            }
            else if(damageSort[AM_DefOf.Blunt] > damageSort[AM_DefOf.Heat])
            {
                armorOffsets[AM_DefOf.Blunt] += (damageSort[AM_DefOf.Blunt] - damageSort[AM_DefOf.Heat])/100;
                armorOffsets[AM_DefOf.Heat] -= (damageSort[AM_DefOf.Blunt] - damageSort[AM_DefOf.Heat])/100;
                if(armorOffsets[AM_DefOf.Heat] < -armorValues[AM_DefOf.Heat])
                {
                    armorOffsets[AM_DefOf.Heat] = -armorValues[AM_DefOf.Heat];
                }
            }

            if(damageSort[AM_DefOf.Heat] < damageSort[DamageArmorCategoryDefOf.Sharp])
            {
                armorOffsets[DamageArmorCategoryDefOf.Sharp] += (damageSort[DamageArmorCategoryDefOf.Sharp] - damageSort[AM_DefOf.Heat])/100;
                armorOffsets[AM_DefOf.Heat] -= (damageSort[DamageArmorCategoryDefOf.Sharp] - damageSort[AM_DefOf.Heat])/100;
                if(armorOffsets[AM_DefOf.Heat] < -armorValues[AM_DefOf.Heat])
                {
                    armorOffsets[AM_DefOf.Heat] = -armorValues[AM_DefOf.Heat];
                }
            }
            else if(damageSort[AM_DefOf.Heat] > damageSort[DamageArmorCategoryDefOf.Sharp])
            {
                armorOffsets[AM_DefOf.Heat] += (damageSort[AM_DefOf.Heat] - damageSort[DamageArmorCategoryDefOf.Sharp])/100;
                armorOffsets[DamageArmorCategoryDefOf.Sharp] -= (damageSort[AM_DefOf.Heat] - damageSort[DamageArmorCategoryDefOf.Sharp])/100;
                if(armorOffsets[DamageArmorCategoryDefOf.Sharp] < -armorValues[DamageArmorCategoryDefOf.Sharp])
                {
                    armorOffsets[DamageArmorCategoryDefOf.Sharp] = -armorValues[DamageArmorCategoryDefOf.Sharp];
                }
            }
        }
    }
}