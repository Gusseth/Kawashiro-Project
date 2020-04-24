using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Kawashiro_Project.data;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kawashiro_Project.commands.modules
{
    public class FlexmoneyCommand : ModuleBase<SocketCommandContext>
    {
        private SocketTextChannel channel;

        /// <summary>
        /// Makes the bot reply with the same message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Command("Flexmoney")]
        [Summary("Flex on the bot.")]
        public async Task Flexmoney([Remainder] string yourMoney = "0")
        {
            channel = (Context.Channel as SocketTextChannel);
            string yourMoneyParsed = yourMoney;

            yourMoneyParsed.Replace(" ", string.Empty);
            yourMoneyParsed.Replace(",", string.Empty);
            yourMoneyParsed.Replace(".", string.Empty);
            yourMoneyParsed = Regex.Replace(yourMoneyParsed, "[^0-9.-]", "");
            if (string.IsNullOrEmpty(yourMoneyParsed))
            {
                await ReplyAsync(string.Format(ResponseManager.GetLine("FlexMoneyNoNumber"), yourMoney, Context.User.Mention));
                return;
            }
            BigInteger money = BigInteger.Parse(yourMoneyParsed);
            if (money < 0)
            {
                await ReplyAsync(string.Format(ResponseManager.GetLine("FlexMoneyBrokeBitch"), money, Context.User.Mention));
                return;
            }
            uint addedAmount = (uint)Nitori.Random.Next(1, int.MaxValue);

            try
            {
                await ReplyAsync(string.Format(ResponseManager.GetLine("FlexMoneyResponse"), money + addedAmount, money, addedAmount, Context.User.Mention));
            }
            catch (ArgumentException)
            {
                int randomChance = Nitori.Random.Next(0, 1);
                if (randomChance == 0)
                {
                    await channel.SendFileAsync("data\\jpow.gif", "My name is Jerome Powell of the US Federal Reserve and money printer goes __**BRRRRRRR**__");
                }
                else
                {
                    await channel.SendFileAsync("data\\toomuchmoney.gif", null);
                }
            }
        }
    }
}
