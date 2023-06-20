using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using InventorySystem.Items.Firearms.Modules;
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
                if (difference.TotalSeconds < Main.Instance.Config.PushCooldown)
                {
                    response = "Pushing too much, too fast.";
                    return false;
                }
            }
            var cast = Physics.Raycast(player.Position, player.Transform.forward, out var hit, Main.Instance.Config.PushRange);
            if (!cast)
            {
                response = "No one to push.";
                return false;
            }
            var hitPlayer = Player.Get(hit.transform.GetComponentInParent<ReferenceHub>());
            if (hitPlayer == null || hitPlayer == player)
            {
                response = "Can't push that.";
                return false;
            }
            Timing.RunCoroutine(PushPlayer(hitPlayer, player));
            if (!Cooldowns.TryGetValue(player, out _))
            {
                Cooldowns.Add(player, DateTime.Now);
            }
            else
            {
                Cooldowns[player] = DateTime.Now;
            }
            hitPlayer.ShowHint(Regex.Replace(Main.Instance.Config.PushedHint, "%player%", player.Nickname), 5);
            response = $"{player.Nickname} has been pushed.";
            return true;

        }

        private IEnumerator<float> PushPlayer(Player hit_player, Player pusher)
        {
            Vector3 pushed = pusher.CameraTransform.forward * Main.Instance.Config.PushForce;
            Vector3 endPosition = hit_player.Position + new Vector3(pushed.x, 0, pushed.z);

            Log.Debug(endPosition);

            for (int i = 1; i < Main.Instance.Config.Iterations; i++)
            {
                Vector3 newPos = Vector3.MoveTowards(hit_player.Position, endPosition, Main.Instance.Config.PushForce/Main.Instance.Config.Iterations);
                
                Log.Debug(newPos);

                if (Physics.Linecast(hit_player.Position, newPos))
                {
                    yield break;
                }
                hit_player.Position = newPos;
                yield return Timing.WaitForOneFrame;
            }

        }
    }
}
