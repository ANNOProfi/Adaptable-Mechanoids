using System.Collections.Generic;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_GameComponent_Adaptation : GameComponent
    {
        private float armor_Blunt;

        private float armor_Sharp;

        private float armor_Heat;

        public Dictionary<string, AM_MechArmorStats> mechList;

        public AM_GameComponent_Adaptation(Game game)
        {
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            
        }
    }
}