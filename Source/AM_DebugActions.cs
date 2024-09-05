using LudeonTK;
using Verse;

namespace AdaptableMechanoids
{
    public static class AM_DebugActions
    {
        [DebugAction("Adaptable Mechanoids", "ResetArmor", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ResetArmor()
        {
            AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

            component.ResetArmor(true);
        }
    }
}