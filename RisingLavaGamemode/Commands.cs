using System;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace RisingLavaGamemode
{
	internal class Commands : ICommandHandler
	{
		private readonly RisingLavaGamemode plugin;
		public Commands(RisingLavaGamemode plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0) return new[] { GetUsage() };

			switch (args[0].ToLower())
			{
				case "enable":
					plugin.Functions.EnableGamemode();

					return new string[] { "RisingLavaGamemode will be Enabled for the next round!" };
				case "disable":
					plugin.Functions.DisableGamemode();

					return new string[] { "RisingLavaGamemode now disabled." };
				case "lcz":
					Timing.RunCoroutine(plugin.Functions.LczLockdown(0f));
					return new[] { "Lcz locked down." };
				case "hcz":
					Timing.RunCoroutine(plugin.Functions.HczLockdown(0f));
					return new[] { "Hcz locked down." };
				case "ent":
					Timing.RunCoroutine(plugin.Functions.EntLockdown(0f));
					return new[] { "Ent locked down." };
				default:
					return new[] { "Ya dun goofed, kid." };
			}
		}

		public string GetUsage() => "Debug Commands only.";

		public string GetCommandDescription() => "";
	}
}