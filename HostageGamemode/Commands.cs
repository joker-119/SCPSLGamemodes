using Smod2.Commands;

namespace HostageGamemode
{
	internal class Commands : ICommandHandler
	{
		private readonly HostageGamemode plugin;
		public Commands(HostageGamemode plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] agrs)
		{
			return new[] { "" };
		}

		public string GetUsage() => "";

		public string GetCommandDescription() => "";
	}
}