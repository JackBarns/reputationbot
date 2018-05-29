using Discord.Commands;
using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using System.Collections.Generic;

namespace new_bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        sqlFunctionsHandler handler = new sqlFunctionsHandler();
        ulong logChannelId = 446413163399741461;

        [Command("!author")]
        public async Task AuthorAsync()
        {
            try
            {
                await Context.User.SendMessageAsync($":orange_book: Bot author is **{Context.Guild.GetUser(265637738370433035).Username}#{Context.Guild.GetUser(265637738370433035).Discriminator}**");
            }
            catch (Exception)
            {
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("!leaderboard")]
        public async Task leaderboardAsync()
        {
            try
            {
                List<CPlayerData> playerDataList = handler.getTopFivePlayers();
                string dataMessage = "";
                int y = 0;
                foreach (var listItem in playerDataList)
                {
                    string usr;
                    y++;
                    IUser f = Context.Client.GetUser(listItem.mPlayerId);
                    try
                    {
                        usr = $"{f.Username}#{f.Discriminator}";
                    }
                    catch { usr = $"Invalid#Invalid"; }
                dataMessage += $"{y}. User **{usr}** has **+{listItem.mpRepPoint}** reputation{Environment.NewLine}";
                }
                await Context.User.SendMessageAsync($":book: **Most Reputable Traders:**{Environment.NewLine}{Environment.NewLine}{dataMessage}");
            }
            catch (Exception y)
            {
                Console.WriteLine(y);
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("+rep")]
        public async Task givePosRepAsync(IGuildUser user, [Remainder] string reason = "")
        {
            try
            {
                if (user == Context.User as IGuildUser)
                {
                    await Context.User.SendMessageAsync($":x: Adding reputation failed{Environment.NewLine}{Environment.NewLine}You can't give yourself rep!");
                }
                else
                {
                    handler.giveRep(user.Id, Context.User.Id, reason, "+rep", Convert.ToString(Context.User));

                    switch (handler.totalReps(user.Id, "+rep"))
                    {
                        case 10:
                            IRole level1 = Context.Guild.GetRole(445210455682711562);
                            await user.AddRoleAsync(level1);
                            await user.SendMessageAsync(":tada: **Nice work!** You advanced to ``Trader Level 1`` for your reputable habits.");
                            break;
                        case 25:
                            IRole level2 = Context.Guild.GetRole(445210552420007938);
                            await user.AddRoleAsync(level2);
                            await user.SendMessageAsync(":tada: **Nice work!** You advanced to ``Trader Level 2`` for your reputable habits.");
                            break;
                        case 50:
                            IRole level3 = Context.Guild.GetRole(445210589833199617);
                            await user.AddRoleAsync(level3);
                            await user.SendMessageAsync(":tada: **Nice work!** You advanced to ``Trader Level 3`` for your reputable habits.");
                            break;
                        case 75:
                            IRole levelmaster = Context.Guild.GetRole(445210626852257794);
                            await user.AddRoleAsync(levelmaster);
                            await user.SendMessageAsync(":tada: **Nice work!** You advanced to ``Master Trader`` for your reputable habits.");
                            break;
                    }

                    if (reason == "" || reason is null)
                    {
                        await Context.Client.GetGuild(Context.Guild.Id).GetTextChannel(logChannelId).SendMessageAsync($"User '**{Convert.ToString(Context.User)}**' just gave '**{Convert.ToString(user)}**' +rep");
                    } else
                    {
                        await Context.Client.GetGuild(Context.Guild.Id).GetTextChannel(logChannelId).SendMessageAsync($"User '**{Convert.ToString(Context.User)}**' just gave '**{Convert.ToString(user)}**' +rep with reason ``{reason}``");
                    }
                    await Context.User.SendMessageAsync($":white_check_mark: You successfully gave **{Convert.ToString(user)}** +rep.");

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("-rep")]
        public async Task giveNegRepAsync(IGuildUser user, [Remainder] string reason = "")
        {
            try
            {
                if (user == Context.User as IGuildUser)
                {
                    await Context.User.SendMessageAsync($":x: Adding reputation failed{Environment.NewLine}{Environment.NewLine}You can't give yourself rep!");
                } else
                {
                    handler.giveRep(user.Id, Context.User.Id, reason, "-rep", Convert.ToString(Context.User));

                    if (reason == "" || reason is null)
                    {
                        await Context.Client.GetGuild(Context.Guild.Id).GetTextChannel(logChannelId).SendMessageAsync($"User '**{Convert.ToString(Context.User)}**' just gave '**{Convert.ToString(user)}**' -rep");
                    }
                    else
                    {
                        await Context.Client.GetGuild(Context.Guild.Id).GetTextChannel(logChannelId).SendMessageAsync($"User '**{Convert.ToString(Context.User)}**' just gave '**{Convert.ToString(user)}**' -rep with reason ``{reason}``");
                    }

                    await Context.User.SendMessageAsync($":white_check_mark: You successfully gave **{Convert.ToString(user)}** -rep.");
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("!delete rep")]
        public async Task deleteRepAsync(IGuildUser user, IGuildUser reppedBy)
        {
            try
            {
                if (handler.isAdmin(Context.User as IGuildUser) == false)
                {
                    await Context.User.SendMessageAsync(":exclamation: **Insufficient permissions!** You do not have access to this command.");
                    return;
                }
                handler.deleteRep(user.Id, reppedBy.Id);
                await Context.User.SendMessageAsync($":white_check_mark: rep from player **{Convert.ToString(user)}** to player **{Convert.ToString(reppedBy)}** has been removed.");
            }
            catch (Exception)
            {
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("!wipe rep")]
        public async Task wipeRepAsync(IGuildUser user)
        {
            try
            {
                if (handler.isAdmin(Context.User as IGuildUser) == false)
                {
                    await Context.User.SendMessageAsync(":exclamation: **Insufficient permissions!** You do not have access to this command.");
                    return;
                }
                handler.wipeRep(user.Id);
                await Context.User.SendMessageAsync($":white_check_mark: All reputation for player** {Convert.ToString(user)}** has been deleted.");
            }
            catch (Exception)
            {
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("!help")]
        public async Task HelpAsync()
        {
            try
            {
                var builder = new EmbedBuilder()
                    .WithTitle($":grey_exclamation: Command Documentation - !help")
                    .WithColor(new Color(0x9E2CCC))
                    .WithTimestamp(DateTime.UtcNow)
                    .WithFooter(footer =>
                    {
                        footer
                            .WithText("Find A Team: Tarkov Reputation Bot")
                            .WithIconUrl("https://cdn.discordapp.com/attachments/444977568366592002/445290364975185930/Discord_Logo.png");
                    })
                    .WithAuthor(author =>
                    {
                        author
                            .WithName("Find A Team: Tarkov Reputation Bot")
                            //.WithUrl("https://discord.gg/CJXCVaW")
                            .WithIconUrl("https://cdn.discordapp.com/attachments/444977568366592002/445290364975185930/Discord_Logo.png");
                    })
                    .AddField("To add positive reputation to a user", "``+rep <user> <comment>`` _(optional)_")
                    .AddField("To add negative reputation to a user", "``-rep <user> <comment>`` _(optional)_")
                    .AddField("To look at a player's reputation history", "``!rep <user>``")
                    .AddField("To view most players with highest reputation", "``!leaderboard``");

                if (handler.isAdmin(Context.User as IGuildUser) == true)
                {
                    builder.AddField("To delete a player's reputation of another players", "``!delete rep <targeted user> <user who gave rep>``");
                    builder.AddField("To wipe a user's reputation completely", "``!wipe rep <user>``");
                    builder.Title = ":grey_exclamation: Command Documentation - Staff !help";
                }

                var embed = builder.Build();



                 await Context.User.SendMessageAsync("", false, embed);
            }


            catch (Exception)
            {
                throw;
            }
            finally { await Context.Message.DeleteAsync(); }
        }

        [Command("!rep")]
        public async Task rep(IGuildUser x)
        {
            ulong playerId = x.Id;

            List<CPlayerData> playerDataList = handler.getRecentRep(playerId);
            string dataMessage = "";
            foreach (var listItem in playerDataList)
            {
                if (listItem.mReason == "" || listItem.mReason is null || listItem.mReason == "No reason provided") {
                    IUser f = Context.Client.GetUser(listItem.mPlayerId);
                    string usr = $"{f.Username}#{f.Discriminator}";
                    dataMessage += $"``{listItem.mTimeStamp}`` **{listItem.mpRepPoint}** from **{usr}**{Environment.NewLine}";
                } else
                {
                    IUser f = Context.Client.GetUser(listItem.mPlayerId);
                    string usr = $"{f.Username}#{f.Discriminator}";
                    dataMessage += $"``{listItem.mTimeStamp}`` **{listItem.mpRepPoint}** from **{usr}** with reason ``{listItem.mReason}``{Environment.NewLine}";
                }
            }

            if (dataMessage == "" || dataMessage is null)
            {
                await Context.User.SendMessageAsync(
                $":blue_book: Player **{Context.Guild.GetUser(playerId).Mention}** has recieved no repuation.");
            } else
            {
            await Context.User.SendMessageAsync(
                $":blue_book: Player **{Convert.ToString(Context.Guild.GetUser(playerId))}** has recieved **{handler.totalReps(playerId, "+rep")}** +rep and **{handler.totalReps(playerId, "-rep")}** -rep." +
                Environment.NewLine + Environment.NewLine + dataMessage);
            }
            await Context.Message.DeleteAsync();
        }
    }

    public class commandExceptionHandler
    {
        public async Task HandleCommandException(SocketUserMessage message, IResult ex)
        {
            try
            {
                var builder = new EmbedBuilder()
                    .WithTitle($":exclamation: Invalid Syntax - You entered something incorrectly, try using one of the commands below instead.")
                    //.WithDescription("A full list of updated commands and their roles.")
                    .WithColor(new Color(0x9E2CCC))
                    .WithTimestamp(DateTime.UtcNow)
                    .WithFooter(footer =>
                    {
                        footer
                            .WithText("Find A Team: Tarkov Reputation Bot")
                            .WithIconUrl("https://cdn.discordapp.com/attachments/444977568366592002/445290364975185930/Discord_Logo.png");
                    })
                    //.WithThumbnailUrl("https://cdn.discordapp.com/attachments/444977568366592002/445290364975185930/Discord_Logo.png")
                    .WithAuthor(author =>
                    {
                        author
                            .WithName("Find A Team: Tarkov Reputation Bot")
                            //.WithUrl("https://discord.gg/CJXCVaW")
                            .WithIconUrl("https://cdn.discordapp.com/attachments/444977568366592002/445290364975185930/Discord_Logo.png");
                    })
                    .AddField("To add positive reputation to a user", "``+rep <user> <comment>`` _(optional)_")
                    .AddField("To add negative reputation to a user", "``-rep <user> <comment>`` _(optional)_")
                    .AddField("To look at a player's reputation history", "``!rep <user>``")
                    .AddField("To view most players with highest reputation", "``!leaderboard``");
                var embed = builder.Build();



                await message.Author.SendMessageAsync("", false, embed);
            }
            catch
            {
                throw;
            }
            finally
            {
                await message.DeleteAsync();
            }
        }
    }
}
