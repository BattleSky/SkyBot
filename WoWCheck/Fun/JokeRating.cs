using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Org.BouncyCastle.Crypto.Tls;

namespace WoWCheck.Fun
{
    public class JokeRating
    {
        /*
         * Должно быть две таблицы:
         * 1. guildID, channelID - каналы, в которых собирать рейтинг
         * 2. messageID, messageDate, rating, userId - рейтинг на каждое сообщение (отсюда собирать топ)
         */
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
            while (true) // todo: exit property
            {
                var messages = await Channel.GetMessagesAsync(200);
                var ratingByMessage = await GetRatingByMessageAsync(messages);
                Thread.Sleep(TimeToSleepMs);
            }
        }

        public async Task UpdateRatingAsync()
        {
            var dateNow = DateTimeOffset.Now;
            throw new NotImplementedException();
        }
        
        private async Task<List<JokeRatingDbModel>> GetRatingByMessageAsync(IEnumerable<DiscordMessage> messages)
        {
            var result = new List<JokeRatingDbModel>();
            foreach (var msg in messages)
            {
                var users = await msg.GetReactionsAsync(EmojiToCheck, 30);
                var rating = users.Count;
                
                foreach (var usr in users)
                    if (usr.Id == msg.Author.Id)
                        rating--;

                var jokeRatingDbModel = new JokeRatingDbModel(msg.Id, msg.Timestamp, msg.Author.Id, rating);
                result.Add(jokeRatingDbModel);
            }
            return result;
        }

        private void WriteToDb(Dictionary<ulong, int> msgRating)
        {
            var queryString = new StringBuilder();
            throw new NotImplementedException();
        }

        private void UpdateDb()
        {
            var queryString = new StringBuilder();
            throw new NotImplementedException();
        }
    }

    // TODO: Move classes to separate file
    public class JokeRatingDbModel 
    {
        private ulong MessageID { get; set; }
        private DateTimeOffset MessageDate { get; set; }
        private ulong UserID { get; set; }
        private int Rating { get; set; }

        public JokeRatingDbModel(ulong messageId, DateTimeOffset messageDate, ulong userId, int rating)
        {
            MessageID = messageId;
            MessageDate = messageDate;
            UserID = userId;
            Rating = rating;
        }

        public void ChangeRating(int rating)
        {
            throw new NotImplementedException();
        }
    }
}