using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBotLibrary;
using MySql.Data.MySqlClient;

namespace DiscordBotTXT
{
    class Program
    {
        DiscordSocketClient client;
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.MessageReceived += CommandsHandler;
            client.Log += Log;

            var token = "ODAzMzEzMTkzODUzMTI0NjA4.YA79tg.Fx61EJP23p5sB_G5C7h9LdByDcM";

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            SetStatus();

            Console.ReadKey();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task SetStatus()
        {
            client.SetGameAsync("I am ready!");

            return Task.CompletedTask;
        }
        private Task CommandsHandler(SocketMessage msg)
        {
            if (!msg.Author.IsBot & msg.Channel.Name == "bot")
            {
                SqlClass db = new SqlClass();

                DataTable table = new DataTable();

                MySqlDataAdapter adapter = new MySqlDataAdapter();

                string prefix = "!";

                string[] message = msg.Content.Split(',');

                DiscordBotCommands commands = new DiscordBotCommands();

                if (message[0].Substring(0, 1) == prefix)
                {
                    message[0] = message[0].Substring(1, message[0].Length - 1).ToLower();
                    if (message[0] == "help")
                    {
                        commands.Help(msg);
                    }
                    if (message[0] == "uncompleted")
                    {
                        commands.Uncompleted(msg);
                    }
                    if (message[0] == "emergency")
                    {
                        commands.Emergency(msg);
                    }
                    if (message[0] == "info")
                    {
                        try
                        {
                            commands.Info(msg, message);
                        }
                        catch(Exception ex)
                        {
                            msg.Channel.SendMessageAsync(ex.Message);
                        }
                    }
                    if (message[0] == "wip")
                    {
                        commands.WIP(msg);
                    }
                    if (message[0] == "complete")
                    {
                        try
                        {
                            commands.Complete(msg, message);
                        }
                        catch (Exception ex)
                        {
                            msg.Channel.SendMessageAsync(ex.Message);
                        }
                    }
                    if (message[0] == "take")
                    {
                        commands.Take(msg, message);
                    }
                    if (message[0] == "drop")
                    {
                        commands.Drop(msg, message);
                    }
                    if (message[0] == "add")
                    {
                        commands.Add(msg, message);
                    }
                    if (message[0] == "del")
                    {
                        commands.Del(msg, message);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
