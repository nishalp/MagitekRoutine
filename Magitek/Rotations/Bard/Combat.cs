using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Logic;
using Magitek.Logic.Bard;
using Magitek.Logic.Roles;
using Magitek.Models.Bard;
using Magitek.Utilities;

namespace Magitek.Rotations.Bard
{
    internal static class Combat
    {
        public static async Task<bool> Execute()
        {
            Group.UpdateAllies();

            if (Core.Me.IsCasting)
                return true;

            if (await Casting.TrackSpellCast())
                return true;

            await Casting.CheckForSuccessfulCast();

            Globals.InParty = PartyManager.IsInParty;
            Globals.PartyInCombat = Globals.InParty && Utilities.Combat.Enemies.Any(r => r.TaggerType == 2) || Core.Me.InCombat;
            Utilities.Routines.Bard.RefreshVars();

            if (!Core.Me.HasTarget || !Core.Me.CurrentTarget.ThoroughCanAttack())
                return false;

            if (await CustomOpenerLogic.Opener()) return true;

            if (BotManager.Current.IsAutonomous)
            {
                if (Core.Me.HasTarget)
                {
                    Movement.NavigateToUnitLos(Core.Me.CurrentTarget, 20);
                }
            }

            if ( Utilities.Routines.Bard.CheckLastSpellsForWeaveing() < 2 && Spells.HeavyShot.Cooldown.TotalMilliseconds > 700 + BardSettings.Instance.UserLatencyOffset )
            {
                if (await Songs.LetMeSingYouTheSongOfMyPeople()) return true;
                if (await Cooldowns.BattleVoice()) return true;
                if (await Cooldowns.RagingStrikes()) return true;
                if (await Cooldowns.RefulgentBarrage()) return true;
                if (await SingleTarget.PitchPerfect()) return true;
                if (await SingleTarget.BloodletterInMagesBallard()) return true;
                if (await SingleTarget.EmpyrealArrow()) return true;
                if (await SingleTarget.Sidewinder()) return true;
                if (await SingleTarget.Bloodletter()) return true;
            }

            if (await Dot.HandleDots()) return true;
            if (await Dot.HandleMultiDotting()) return true;
            if (await Aoe.ApexArrow()) return true;
            if (await SingleTarget.StraightShot()) return true;
            return (await SingleTarget.HeavyShot());

        }
    }
}