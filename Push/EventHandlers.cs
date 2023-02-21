using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Server;

namespace Push
{
    internal class EventHandlers
    {
        public void OnRoundEnd(RoundEndedEventArgs _)
        {
            Push.Cooldowns.Clear();
        }
    }
}
