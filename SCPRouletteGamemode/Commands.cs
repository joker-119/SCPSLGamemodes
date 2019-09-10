using System;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace SCPRouletteGamemode
{
	internal class Commands : ICommandHandler
	{
		private readonly ScpRouletteGamemode plugin;
		public Commands(ScpRouletteGamemode plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0) return new[] { GetUsage() };

			switch (args[0].ToLower())
			{
				case "enable":
					plugin.Functions.EnableGamemode();

					return new string[] { "SCP Roulette will be Enabled for the next round!" };
				case "disable":
					plugin.Functions.DisableGamemode();

					return new string[] { "SCP Roulette now disabled." };
				default:
					return new[] { "Ya dun goofed, kid." };
			}
		}

		public string GetUsage() => "Debug Commands only.";

		public string GetCommandDescription() => "";
	}
}