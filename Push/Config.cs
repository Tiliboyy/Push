using Exiled.API.Interfaces;
using System.ComponentModel;

namespace Push
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }

        [Description("The force of the push. Accepts float values")]
        public float PushForce { get; set; } = 3f;
        [Description("The distance at which a player can push. Accepts float values")]
        public float PushLength { get; set; } = 0.9f;
        [Description("How frequently can one push people. Accepts float values")]
        public float PushCooldownSec { get; set; } = 5.5f;
        [Description("Hint to show to the player that got pushed. %player% - Nickname of the player that pushed.")]
        public string PushedHint { get; set; } = "<color=red>%player%</color> pushed you! Asshole.";
        [Description("Higher value - more resource usage, but somother")]
        public int Iterations { get; set; } = 16;
        [Description("Do not enable this unless instructed otherwise")]
        public bool DebugMode { get; set; } = false;
    }
}
