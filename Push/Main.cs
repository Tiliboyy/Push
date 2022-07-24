using Exiled.API.Features;
using Server = Exiled.Events.Handlers.Server;

namespace Push
{
    public class Main : Plugin<Config>
    {
        EventHandlers handlers = new();

        public static Main Instance { get; set; }

        public override void OnEnabled()
        {
            Instance = this;
            handlers = new();
            Server.RoundEnded += handlers.OnRoundEnd;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            handlers = null;
            Server.RoundEnded -= handlers.OnRoundEnd;
            base.OnDisabled();
        }
    }
}