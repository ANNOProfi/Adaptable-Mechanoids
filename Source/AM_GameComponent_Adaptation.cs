using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_GameComponent_Adaptation : GameComponent
    {
        public Dictionary<string, AM_MechArmorStats> mechArmorList = new Dictionary<string, AM_MechArmorStats>();

        public HashSet<string> mechList = new HashSet<string>();

        private int ticksToUpdate = AM_Utilities.Settings.adaptationTime;

        private bool useHeatInitialised = false;

        private bool useMaxInitialised = false;

        public AM_GameComponent_Adaptation(Game game)
        {
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if(ticksToUpdate != 0)
            {
                ticksToUpdate--;
                return;
            }
            else
            {
                //Resetting if heat armor is enabled mid-game
                if(!useHeatInitialised && AM_Utilities.Settings.useHeat)
                {
                    useHeatInitialised = true;

                    foreach(string name in mechList)
                    {
                        mechArmorList[name].ResetArmor(false);
                    }
                }

                if(!useMaxInitialised && AM_Utilities.Settings.useMax)
                {
                    useMaxInitialised = true;

                    foreach(string name in mechList)
                    {
                        mechArmorList[name].ResetMax();
                    }
                }

                foreach(string name in mechList)
                {
                    mechArmorList[name].CalculateArmor(AM_Utilities.Settings.hardMode);
                }

                if(useHeatInitialised && !AM_Utilities.Settings.useHeat)
                {
                    useHeatInitialised = false;
                }

                if(useMaxInitialised && !AM_Utilities.Settings.useMax)
                {
                    useMaxInitialised = false;

                    foreach(string name in mechList)
                    {
                        mechArmorList[name].ResetMax();
                    }
                }

                ticksToUpdate = AM_Utilities.Settings.adaptationTime;
            }
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            if(AM_Utilities.Settings.useHeat)
            {
                useHeatInitialised = true;
            }

            if(AM_Utilities.Settings.useMax)
            {
                useMaxInitialised = true;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref mechList, false, "mechList", LookMode.Deep);
            Scribe_Collections.Look(ref mechArmorList, "mechArmorList", LookMode.Deep, LookMode.Deep);
        }
    }
}