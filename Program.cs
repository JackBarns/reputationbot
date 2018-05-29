using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Discord;
using new_bot.Modules;

namespace new_bot
{
    class Program {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
        ioFunctionsHandler _ioHandler = new ioFunctionsHandler();
        sqlFunctionsHandler _sqlHandler = new sqlFunctionsHandler();

        public DiscordSocketClient _client;
        private CommandService _commands = new CommandService(new CommandServiceConfig
        {
            CaseSensitiveCommands = false,
            ThrowOnError = false,
            DefaultRunMode = RunMode.Async,
            IgnoreExtraArgs = true
        });
        private IServiceProvider _services;
        commandExceptionHandler cmdExceptionHandler = new commandExceptionHandler();

        public async Task RunBotAsync()
        {
            if (_ioHandler.useFileInfo() == true)
            {
                _sqlHandler.IoSql_logLine($"Using /cfg/sqlInfo.txt");
                _sqlHandler.Host = _ioHandler.getSqlParam("sqlHost");
                _sqlHandler.Port = _ioHandler.getSqlParam("sqlPort");
                _sqlHandler.Username = _ioHandler.getSqlParam("sqlUsername");
                _sqlHandler.Password = _ioHandler.getSqlParam("sqlPassword");
                _sqlHandler.IntegratedSecurity = _ioHandler.getSqlParam("sqlIntegratedSecurity");
                _sqlHandler.Database = _ioHandler.getSqlParam("sqlDatabase");
                _sqlHandler.IoSql_logLine($"Host = {_sqlHandler.Host}");
                _sqlHandler.IoSql_logLine($"Port = {_sqlHandler.Port}");
                _sqlHandler.IoSql_logLine($"Username = {_sqlHandler.Username}");
                _sqlHandler.IoSql_logLine($"Password = {_sqlHandler.Password}");
                _sqlHandler.IoSql_logLine($"Security = {_sqlHandler.IntegratedSecurity}");
                _sqlHandler.IoSql_logLine($"Database = {_sqlHandler.Database}");
            }
            _sqlHandler.Initialize();
            _sqlHandler.testConnection();
            _client = new DiscordSocketClient();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            string botToken = "enter token here";

            _client.Log += Log;

            await RegisterCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, botToken);
            await _client.StartAsync();
            await _client.SetGameAsync("!help");

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (!(_sqlHandler.IsBotChannel(message.Channel.Id)) || message is null || message.Author.IsBot) { return; }


                int argPos = 0;
                var context = new SocketCommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                   Console.WriteLine(result.ErrorReason);
                   await cmdExceptionHandler.HandleCommandException(message, result);
                }
        }
    }
}
   