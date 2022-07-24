using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using MEC;

namespace Push
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class Push : ParentCommand
    {
        public static Dictionary<Player, DateTime> Cooldowns = new Dictionary<Player, DateTime>();
        public Push() => LoadGeneratedCommands();
        public override string Command => ".push";

        public override string[] Aliases => new string[] {"push", ".p"};

        public override string Description => "pushes someone in front of you.";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
            if (Main.Instance.Config.DebugMode)
            {
                Timing.RunCoroutine(PushPlayer(player, player)); // for testing without 2nd player
                response = "debug";
                return true;
            }
            if (Cooldowns.TryGetValue(player, out DateTime value))
            {
                var difference = DateTime.Now - value;
                if (difference.TotalSeconds < Main.Instance.Config.PushCooldownSec)
                {
                    response = "Pushing too much, too fast.";
                    return false;
                }
            }
            var ray = new Ray(player.CameraTransform.position, player.CameraTransform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, Main.Instance.Config.PushLength))
            {
                response = "No one to push.";
                return false;
            }
            var hit_player = Player.Get(hit.transform.gameObject);
            if (hit_player == null || hit_player == player)
            {
                response = "Can't push that.";
                return false;
            }
            Timing.RunCoroutine(PushPlayer(hit_player, player));
            if (!Cooldowns.TryGetValue(player, out _))
            {
                Cooldowns.Add(player, DateTime.Now);
            }
            else
            {
                Cooldowns[player] = DateTime.Now;
            }
            hit_player.ShowHint(Regex.Replace(Main.Instance.Config.PushedHint, "%player%", player.Nickname), 5);
            response = $"{player.Nickname} has been pushed.";
            return true;

        }

        private IEnumerator<float> PushPlayer(Player hit_player, Player pusher)
        {
            Vector3 pushed = pusher.CameraTransform.forward * Main.Instance.Config.PushForce;
            Vector3 endPosition = hit_player.Position + new Vector3(pushed.x, 0, pushed.z);

            Log.Debug(endPosition, Main.Instance.Config.DebugMode);

            for (int i = 1; i < Main.Instance.Config.Iterations; i++)
            {
                Vector3 newPos = Vector3.MoveTowards(hit_player.Position, endPosition, Main.Instance.Config.PushForce/Main.Instance.Config.Iterations);
                
                Log.Debug(newPos, Main.Instance.Config.DebugMode);

                if (Physics.Linecast(hit_player.Position, newPos, hit_player.ReferenceHub.playerMovementSync.CollidableSurfaces))
                {
                    yield break;
                }
                hit_player.Position = newPos;
                yield return Timing.WaitForOneFrame;
            }

        }
    }
}
