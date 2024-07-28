using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using LudeonTK;

namespace AdaptableMechanoids
{
    public class AM_MechArmorStats
    {
        private float Armor_total
        {
            get
            {
                if(AM_Utilities.Settings.useHeat)
                {
                    return armorValues[AM_DefOf.Blunt] + armorValues[DamageArmorCategoryDefOf.Sharp] + armorValues[AM_DefOf.Heat];
                }

                return armorValues[AM_DefOf.Blunt] + armorValues[DamageArmorCategoryDefOf.Sharp];
            }
        }

        private float DamageAmountsTotal
        {
            get
            {
                if(AM_Utilities.Settings.useHeat)
                {
                    return damageAmounts[AM_DefOf.Blunt] + damageAmounts[DamageArmorCategoryDefOf.Sharp] + damageAmounts[AM_DefOf.Heat];
                }

                return damageAmounts[AM_DefOf.Blunt] + damageAmounts[DamageArmorCategoryDefOf.Sharp]; 
            }
        }

        private List<DamageArmorCategoryDef> armorTypes = new List<DamageArmorCategoryDef>();

        //private float armor_totalOriginal = 0.0f;

        private float unspentPoints = 0f;

        private float adaptationStep;

        public Dictionary<DamageArmorCategoryDef, float> armorValues = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> armorOffsets = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> damageAmounts = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> armorNewValues = new Dictionary<DamageArmorCategoryDef, float>();

        private string defName = null;

        public string DefName{get => defName; set => defName = value;}

        private float maxValue = 2;

        private bool usingHeat = false;

        public AM_MechArmorStats(float blunt, float sharp, float heat, string name)
        {
            armorTypes.Add(AM_DefOf.Blunt);
            armorTypes.Add(DamageArmorCategoryDefOf.Sharp);

            armorValues.Add(AM_DefOf.Blunt, blunt);
            armorValues.Add(DamageArmorCategoryDefOf.Sharp, sharp);
            armorValues.Add(AM_DefOf.Heat, heat);
            //

            armorOffsets.Add(AM_DefOf.Blunt, 0f);
            armorOffsets.Add(DamageArmorCategoryDefOf.Sharp, 0f);
            armorOffsets.Add(AM_DefOf.Heat, 0f);
            //

            armorNewValues.Add(AM_DefOf.Blunt, 0f);
            armorNewValues.Add(DamageArmorCategoryDefOf.Sharp, 0f);
            armorNewValues.Add(AM_DefOf.Heat, heat);

            //armor_totalOriginal = armorValues[AM_DefOf.Blunt] + armorValues[DamageArmorCategoryDefOf.Sharp];

            DefName = name;

            damageAmounts.Add(AM_DefOf.Blunt, 0f);
            damageAmounts.Add(DamageArmorCategoryDefOf.Sharp, 0f);

            if(AM_Utilities.Settings.useHeat)
            {
                armorTypes.Add(AM_DefOf.Heat);
                //armor_totalOriginal += armorValues[AM_DefOf.Heat];

                damageAmounts.Add(AM_DefOf.Heat, 0f);

                usingHeat = true;
            }

            maxValue = AM_Utilities.Settings.maxValue;
            adaptationStep = AM_Utilities.Settings.adaptationStep;
        }

        public void ResetArmor()
        {
            bool useHeat = AM_Utilities.Settings.useHeat;
            bool hardMode = AM_Utilities.Settings.hardMode;

            maxValue = AM_Utilities.Settings.maxValue;

            if(useHeat)
            {
                armorTypes.Add(AM_DefOf.Heat);
                armorOffsets.Add(AM_DefOf.Heat, 0f);
                //armor_totalOriginal += armorValues[AM_DefOf.Heat];

                damageAmounts.Add(AM_DefOf.Heat, 0f);
            }
            else if(usingHeat && !useHeat)
            {
                damageAmounts.Remove(AM_DefOf.Heat);

                if(!hardMode)
                {
                    unspentPoints += armorOffsets[AM_DefOf.Heat];
                }
                
                armorOffsets[AM_DefOf.Heat] = 0f;
                armorTypes.Remove(AM_DefOf.Heat);

                usingHeat = false;
            }

            if(!hardMode)
            {
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = Armor_total*(damageAmounts[armor]/(DamageAmountsTotal));
                }

                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] < armorValues[armor])
                    {
                        while(armorOffsets[armor] - adaptationStep >= -armorValues[armor])
                        {
                            armorOffsets[armor] -= adaptationStep;
                            unspentPoints += adaptationStep;
                        }
                    }
                }
            }
        }

        public void CalculateArmor(bool hardMode)
        {

            if(!hardMode)
            {
                if(usingHeat && !AM_Utilities.Settings.useHeat)
                {
                    damageAmounts.Remove(AM_DefOf.Heat);
                    unspentPoints += armorOffsets[AM_DefOf.Heat];
                    armorOffsets[AM_DefOf.Heat] = 0f;
                    armorTypes.Remove(AM_DefOf.Heat);
                }

                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = Armor_total*(damageAmounts[armor]/(DamageAmountsTotal));
                }

                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] < armorValues[armor])
                    {
                        if(armorOffsets[armor] - adaptationStep >= -armorValues[armor])
                        {
                            armorOffsets[armor] -= adaptationStep;
                            unspentPoints += adaptationStep;
                        }
                    }
                }

                while(unspentPoints > 0f)
                {
                    foreach(DamageArmorCategoryDef armor in armorTypes)
                    {
                        if(armorNewValues[armor] > armorValues[armor] && armorOffsets[armor] + armorValues[armor] < maxValue)
                        {
                            armorOffsets[armor] += adaptationStep;
                            unspentPoints -= adaptationStep;
                        }
                    }
                }
            }
            else
            {
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = armorValues[armor] + (damageAmounts[armor] * 0.01f);
                }
            }

            if(unspentPoints > 0f || hardMode)
            {
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] > armorValues[armor] && armorOffsets[armor] + armorValues[armor] < maxValue)
                    {
                        if((unspentPoints - adaptationStep) >= 0f && !hardMode)
                        {
                            armorOffsets[armor] += adaptationStep;
                            unspentPoints -= adaptationStep;
                        }
                        else if(hardMode)
                        {
                            armorOffsets[armor] += adaptationStep * 0.1f;
                        }
                    }
                }
            }
        }
    }
}