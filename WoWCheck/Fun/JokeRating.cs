using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace WoWCheck.Fun
{
    public class JokeRating
    {
        private DiscordEmoji EmojiToCheck { get; }
        private DiscordChannel Channel { get; }
        /// <summary>
        /// minute * seconds * milliseconds
        /// </summary>
        private const int TimeToSleepMs = 1 * 60 * 1000; 
        private const string EmojiString = "";
        private const string TableName = "";

        public JokeRating(CommandContext ctx)
        {
            EmojiToCheck = DiscordEmoji.FromName(Connections.Connections.Discord, EmojiString);
            Channel = ctx.Channel;
        }

        public async Task<List<string>> CollectMessagesWithJokesAsync()
        {
            while (true)
            {
                var messages = await Channel.GetMessagesAsync(200);
                var ratingByMessage = await GetRatingByMessage(messages);
                Thread.Sleep(TimeToSleepMs);
            }
        }
        
        private async Task<Dictionary<ulong, int>> GetRatingByMessage(IEnumerable<DiscordMessage> messages)
        {
            var resultDict = new Dictionary<ulong, int>();
            foreach (var msg in messages)
            {
                var users = await msg.GetReactionsAsync(EmojiToCheck, 30);
                var rating = users.Count;
                
                foreach (var usr in users)
                    if (usr.Id == msg.Author.Id)
                        rating--;
                
                resultDict.Add(msg.Id, rating);
            }
            return resultDict;
        }

        private void WriteToDb(Dictionary<ulong, int> msgRating)
        {
            var queryString = new StringBuilder();
            throw new NotImplementedException();
        }

        private void UpdateDb()
        {
            throw new NotImplementedException();
        }
    }
}