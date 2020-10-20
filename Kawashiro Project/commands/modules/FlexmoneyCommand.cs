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
        /// Bot replies with your money + a random number between [1, int.MaxValue]
        /// 
        /// EXCEPT:
        ///     - If there are no numbers,          then reply with FlexMoneyNoNumber
        ///     - (BigInteger)yourMoney < 0         then reply with FlexMoneyBrokeBitch
        ///     - Bot response goes over text limit then reply with a dated Jerome Powell joke
        /// </summary>
        /// <param name="yourMoney">Money amount.</param>
        /// <returns></returns>
        [Command("Flexmoney")]
        [Summary("Flex on the bot.")]
        public async Task Flexmoney([Remainder] string yourMoney = "0")
        {
            channel = (Context.Channel as SocketTextChannel);
            string yourMoneyParsed = yourMoney;

            yourMoneyParsed.Replace(" ", string.Empty); // Remove spaces - Looking back, this may be redundant
            yourMoneyParsed.Replace(",", string.Empty); // Remove , - Looking back, this may be redundant
            yourMoneyParsed.Replace(".", string.Empty); // Remove . - Looking back, this may be redundant
            yourMoneyParsed = Regex.Replace(yourMoneyParsed, "[^0-9.-]", "");   // Remove non-numeric characters

            // If there are no numbers in the message
            if (string.IsNullOrEmpty(yourMoneyParsed))  
            {
                await ReplyAsync(
                    string.Format(ResponseManager.GetLine("FlexMoneyNoNumber"), // Load dialogue 
                    yourMoney,                                                  // [0] = The message sent
                    Context.User.Mention));                                     // [1] = The user who executed this command
                return;
            }

            // Convert to BigInteger because neither int nor int64 can help us here...
            BigInteger money = BigInteger.Parse(yourMoneyParsed);   

            // If the money is less than 0
            if (money < 0)
            {
                await ReplyAsync(
                    string.Format(ResponseManager.GetLine("FlexMoneyBrokeBitch"),  // Load dialogue 
                    money,                                                         // [0] = The message sent
                    Context.User.Mention));                                        // [1] = The user who executed this command
                return;
            }

            // Money added compared to user's amount
            uint addedAmount = (uint)Nitori.Random.Next(1, int.MaxValue);

            try
            {
                // Normal case
                await ReplyAsync(
                    string.Format(ResponseManager.GetLine("FlexMoneyResponse"), // Load dialogue
                    money + addedAmount,                                        // [0] = User's money + added amount, bot's money
                    money,                                                      // [1] = User's money
                    addedAmount,                                                // [2] = The added amount
                    Context.User.Mention));                                     // [3] = The user who executed this command
            }
            catch (ArgumentException)
            {
                // Generic ArgumentException case, 99% of the time this is called when the bot's response exceeds the text limit
                int randomChance = Nitori.Random.Next(0, 2);
                if (randomChance == 0)
                {
                    // Random chance, 50/50
                    await channel.SendFileAsync("data\\jpow.gif", "My name is Jerome Powell of the US Federal Reserve and money printer goes __**BRRRRRRR**__");
                }
                else
                {
                    // Random chance, 50/50
                    await channel.SendFileAsync("data\\toomuchmoney.gif", null);
                }
            }
        }
    }
}
