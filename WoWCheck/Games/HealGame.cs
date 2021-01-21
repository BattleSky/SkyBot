using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace WoWCheck.Games
{

    static class HealGame
    {
        private static int MinTankHp => 27000;
        private static int MaxTankHp => 50000;
        private static int CurrentTankHp { get; set; }
        public static bool IsGameActive => (CurrentTankHp > 0 && CurrentTankHp < MaxTankHp);
        //      public static List<string> Logging;

        public static DiscordEmbedBuilder InitializeHealGame()
        {
            
            var rnd = new Random();
            var color = "какого-то";
            CurrentTankHp = rnd.Next(MinTankHp, MaxTankHp-5000);
            if (CurrentTankHp < 32000)
                color = "зеленого";
            else if(CurrentTankHp >= 32000 && CurrentTankHp <= 39000)
                color = "синего";
            else if (CurrentTankHp > 39000)
                color = "эпического";
//            Logging = new List<string>() {"Создан танк с " + CurrentTankHp + " здоровья."};
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Исцеляй-игра",
                Description = "Создал Вам " + color + " танка и отправил в рейд.",
                Timestamp = DateTime.UtcNow
            };
            return embed;
        }
        
        public static DiscordEmbedBuilder ResultOfOneRound(CommandContext ctx)
        {

            var heal = CalculateHeal();
            var messageHealingDone = MessageHealingDone(heal, ctx);
            var damageToTank = CalculateTankDamage();
            var messageTankDamage = MessageTankDamage(damageToTank);
            
            CurrentTankHp = CurrentTankHp + heal - damageToTank;

            var result = "**Танк исцелен на " + heal + "**\n *" + messageHealingDone + "*\n**Танк огреб " +
                         damageToTank + "**\n *" + messageTankDamage + "*\n**Здоровье танка: " + CurrentTankHp + "**";

            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#3AE6DB"),
                Title = "Результат боя",
                //Description = "Жизнь тлен. Особенно танка. И не жизнь вовсе.",
                Timestamp = DateTime.UtcNow
            };
            if (CurrentTankHp >= MaxTankHp)
                embed = TankIsAliveEmbed();
            else if (CurrentTankHp <= 0)
                embed = TankIsDeadEmbed();
            else
                embed.AddField("Всё ещё в бою", result);
                
            return embed;
        }
        private static int CalculateTankDamage()
        {
            var rnd = new Random();
            return rnd.Next(16000);
        }
        private static int CalculateHeal()
        {
            var rnd = new Random();
            return rnd.Next(16000);
        }

        private static DiscordEmbedBuilder TankIsDeadEmbed()
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#F90012"),
                Title = "Танк погиб!",
                Description = "Жизнь тлен. Особенно танка. И не жизнь вовсе.",
                Timestamp = DateTime.UtcNow
            };
            //var resultBuilder = new StringBuilder();
            //foreach (var log in Logging)
            //{
            //    resultBuilder.Append(log + "\n");
            //}
            //embed.AddField("Лог", resultBuilder.ToString());


            return embed;
        }

        private static DiscordEmbedBuilder TankIsAliveEmbed()
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#48DD00"),
                Title = "Танк исцелен! Вы спасены!",
                Description = "Ура!",
                Timestamp = DateTime.UtcNow
            };
            //var resultBuilder = new StringBuilder();
            //foreach (var log in Logging)
            //{
            //    resultBuilder.Append(log + "\n");
            //}
            //embed.AddField("Лог", resultBuilder.ToString());
            return embed;
        }
        
        private static string MessageTankDamage(int damageToTank)
        {
            var resultMessage = "";
            if (damageToTank == 0)
                resultMessage = "Босс промахнулся. Куда он вообще смотрит?";
            else if (damageToTank > 0 && damageToTank < 3000)
                resultMessage = "Танк получил небольшой урон! Похильте, ему грустно!";
            else if (damageToTank >= 3000 && damageToTank < 7000)
                resultMessage = "Танк хотел увернуться, но не вышло.";
            else if (damageToTank >= 7000 && damageToTank < 9000)
                resultMessage = "Танк забыл прожаться! Слишком много кнопок!";
            else if (damageToTank >= 9000 && damageToTank < 12000)
                resultMessage = "Танку выбили зуб, но он держится. И вы держитесь.";
            else if (damageToTank >= 12000)
                resultMessage = "Босс использовал против танка незаконный приём! Ух, тяжело.";

            return resultMessage;
        }

        private static string MessageHealingDone(int healingDone, CommandContext ctx)
        {
            var resultMessage = "";
            if (healingDone == 0)
                resultMessage = ctx.User.Mention + " исцелил себя вместо танка. Уверен, что хил - это для тебя?";
            else if (healingDone > 0 && healingDone < 3000)
                resultMessage = ctx.User.Mention + " задел танка АОЕ исцелением.";
            else if (healingDone >= 3000 && healingDone < 7000)
                resultMessage = ctx.User.Mention + " прилепил на танка пластырь.";
            else if (healingDone >= 7000 && healingDone < 9000)
                resultMessage = ctx.User.Mention + " похилил потому что может.";
            else if (healingDone >= 9000 && healingDone < 12000)
                resultMessage = ctx.User.Mention + " прожал мощную исцеляшку! А ты неплох!";
            else if (healingDone >= 12000)
                resultMessage = ctx.User.Mention + ", красава!";

            return resultMessage;
        }




    }
}
