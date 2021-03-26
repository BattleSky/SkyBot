using System;
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
            return rnd.Next(16000) * 2;
        }
        private static int CalculateHeal()
        {
            var rnd = new Random();
            return rnd.Next(16000) * 2;
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
            return embed;
        }
        
        private static string MessageTankDamage(int damageToTank)
        {
            var resultMessage = damageToTank switch
            {
                _ when damageToTank <= 0 => "Босс промахнулся. Куда он вообще смотрит?",
                _ when damageToTank < 3000 => "Танк получил небольшой урон! Похильте, ему грустно!",
                _ when damageToTank < 7000 => "Танк хотел увернуться, но не вышло.",
                _ when damageToTank < 9000 => "Танк забыл прожаться! Слишком много кнопок!",
                _ when damageToTank < 12000 => "Танку выбили зуб, но он держится. И вы держитесь.",
                _ when true => "Босс использовал против танка незаконный приём! Ух, тяжело."
            };
            return resultMessage;
        }

        private static string MessageHealingDone(int healingDone, CommandContext ctx)
        {
            var resultMessage = healingDone switch
            {
                _ when healingDone <= 0 => ctx.User.Mention +
                                           " исцелил себя вместо танка. Уверен, что хил - это для тебя?",
                _ when healingDone < 3000 => ctx.User.Mention + " задел танка АОЕ исцелением.",
                _ when healingDone < 7000 => ctx.User.Mention + " прилепил на танка пластырь.",
                _ when healingDone < 9000 => ctx.User.Mention + " похилил потому что может.",
                _ when healingDone < 12000 => ctx.User.Mention + " прожал мощную исцеляшку! А он неплох!",
                _ when true => ctx.User.Mention + ", красава!",
            };
            return resultMessage;
        }
    }
}
