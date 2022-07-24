using Exiled.Events.EventArgs;

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
