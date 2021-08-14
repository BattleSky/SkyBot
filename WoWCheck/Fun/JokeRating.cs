using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json.Linq;

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
        private const string TableName = ""; // todo: fill
        private CancellationToken _cancellationToken;

        public JokeRating(CommandContext ctx)
        {
            EmojiToCheck = DiscordEmoji.FromName(Connections.Connections.Discord, EmojiString);
            Channel = ctx.Channel;
            _cancellationToken = new CancellationToken();
        }
        
        /*todo: 
         * сделать взаимодействие с guildId и channelId
         * временно можно сделать заглушку для конкретного канала
         */

        public async Task ExecuteWorkForGuild()
        {
            
            while (!_cancellationToken.IsCancellationRequested)
            {
                
            }
        }

        /// <summary>
        /// Первичый сбор сообщений с эмоциями
        /// </summary>
        public async Task CollectMessagesWithJokesAsync()
        {
            var lastMessage = Channel.LastMessageId;
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (Channel.LastMessageId == lastMessage) continue;
                lastMessage = Channel.LastMessageId;
                var messages = await Channel.GetMessagesAsync(100);
                var ratingByMessage = await GetJokesFromDiscordMessagesAsync(messages);
                await UpdateDb(ratingByMessage);
            }
        }
        /// <summary>
        /// Обновление рейтингов на основе выставленных реакций к сообщениям. ID сообщений собираются из БД.
        /// </summary>
        public async Task UpdateRatingAsync()
        {
            // todo: make this task trigger less frequently
            var dateBackWeekly = DateTimeOffset.Now.AddDays(-7);
            var messagesWithRating = await GetListOfJokesFromDB(dateBackWeekly, DateTimeOffset.Now);

            foreach (var jokeRatingMessage in messagesWithRating)
            {
                var discordMessage = await Channel.GetMessageAsync(jokeRatingMessage.MessageId);
                var reactCount = (await discordMessage.GetReactionsAsync(EmojiToCheck, 30)).Count;

                if (reactCount == jokeRatingMessage.Rating)
                {
                    messagesWithRating.Remove(jokeRatingMessage);
                    continue;
                }
                jokeRatingMessage.ChangeRating(CountRating(discordMessage).Result);
            }

            await UpdateDb(messagesWithRating);
        }
        
        /// <summary>
        /// Получает шутки из дискорд-сообщений
        /// </summary>
        /// <param name="messages">Дискорд-сообщения</param>
        /// <returns>Лист шуток</returns>
        private async Task<List<JokeRatingDbModel>> GetJokesFromDiscordMessagesAsync(IEnumerable<DiscordMessage> messages)
        {
            var result = new List<JokeRatingDbModel>();
            foreach (var msg in messages)
            {
                var rating = await CountRating(msg);
                var jokeRatingDbModel = new JokeRatingDbModel(msg.Id, msg.Timestamp, msg.Author.Id, rating);
                result.Add(jokeRatingDbModel);
            }
            return result;
        }

        /// <summary>
        /// Подсчет рейтинга шутки для дискорд-сообщения.
        /// </summary>
        /// <param name="msg">Дискорд-сообщение</param>
        /// <returns>Рейтинг</returns>
        private async Task<int> CountRating(DiscordMessage msg)
        {
            var users = await msg.GetReactionsAsync(EmojiToCheck, 30);
            var rating = users.Count;
                
            foreach (var usr in users)
                if (usr.Id == msg.Author.Id)
                    rating--;
            return rating;
        }

        private async Task WriteToDb(List<JokeRatingDbModel> jokesList)
        {
            var queryString = new StringBuilder();
            queryString.Append("INSERT INTO wowcheck.jokesRating");
            foreach (var j in jokesList)
            {
                //todo: end this
                queryString.Append($"('id' = {j.MessageId}, 'messageDate' = '{j.MessageDate}'),");
            }
            
            throw new NotImplementedException();
        }

        private async Task UpdateDb(List<JokeRatingDbModel> jokesList, DateTimeOffset from = default, DateTimeOffset to = default)
        {
            if (from == default || to == default)
                throw new NotImplementedException();
            

            var queryString = new StringBuilder();
            throw new NotImplementedException();   
        }

        private async Task<List<JokeRatingDbModel>> GetListOfJokesFromDB(DateTimeOffset from = default, DateTimeOffset to = default)
        {
            if (from == default || to == default)
                throw new NotImplementedException();


            throw new NotImplementedException();
        }
    }

    // TODO: Move classes to separate file
    public class JokeRatingDbModel 
    {
        public ulong MessageId { get; private set; }
        public DateTimeOffset MessageDate { get; private set; }
        public ulong UserId { get; private set; }
        public int Rating { get; private set; }

        public JokeRatingDbModel(ulong messageId, DateTimeOffset messageDate, ulong userId, int rating)
        {
            MessageId = messageId;
            MessageDate = messageDate;
            UserId = userId;
            Rating = rating;
        }

        public void ChangeRating(int rating)
        {
            Rating = rating;
        }
    }
}