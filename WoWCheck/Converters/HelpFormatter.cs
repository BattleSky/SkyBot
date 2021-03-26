using System;
using System.Collections.Generic;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;

namespace WoWCheck.Converters
{
    class HelpFormatter : BaseHelpFormatter
    {
        protected DiscordEmbedBuilder Embed;
        protected StringBuilder StringBuilder;

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            Embed = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Команды",
                Description = "Команды, используемые ботом SkyBot",
                Timestamp = DateTime.UtcNow,
            };
            StringBuilder = new StringBuilder();
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            Embed.AddField(command.Name, command.Description);
            StringBuilder.AppendLine($"{command.Name} - {command.Description}");
            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            foreach (var cmd in subcommands)
            {
                Embed.AddField("-" + cmd.Name, cmd.Description);            
                StringBuilder.AppendLine($"{cmd.Name} - {cmd.Description}");
            }
            return this;
        }

        public override CommandHelpMessage Build()
        {
             return new CommandHelpMessage(embed: Embed);
        }
    }
}
