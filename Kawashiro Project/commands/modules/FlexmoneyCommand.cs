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
        /// <summary>
        /// Makes the bot reply with the same message
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Command("Flexmoney")]
        [Summary("Flex on the bot.")]
        public async Task Flexmoney([Remainder] string yourMoney = "0")
        {
            yourMoney.Replace(" ", string.Empty);
            yourMoney.Replace(",", string.Empty);
            yourMoney.Replace(".", string.Empty);
            yourMoney = Regex.Replace(yourMoney, "[^0-9.-]", "");
            BigInteger money = BigInteger.Parse(yourMoney);
            if (money < 0)
            {
                await ReplyAsync(string.Format(LineManager.GetLine("FlexMoneyBrokeBitch"), money, Context.User.Mention));
                return;
            }
            uint addedAmount = (uint)Nitori.random.Next(1, int.MaxValue);

            try
            {
                await ReplyAsync(string.Format(LineManager.GetLine("FlexMoneyResponse"), money + addedAmount, money, addedAmount, Context.User.Mention));
            }
            catch (ArgumentException)
            {
                await (Context.Channel as SocketTextChannel).SendFileAsync("data\\toomuchmoney.gif", null);
            }
        }
    }
}
