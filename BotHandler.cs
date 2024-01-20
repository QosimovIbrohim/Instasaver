using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
namespace ScheckBot
{
    public class BotHandler
    {
        public string Token { get; set; }
        public bool Subscription = false;
        public BotHandler(string token)
        {
            Token = token;
        }

        public async Task BotHandle()
        {
            var botClient = new TelegramBotClient(Token);

            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();
            Console.WriteLine("Bot listening you");
            Console.ReadLine();

            cts.Cancel();

        }

        public async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellation)
        {
            if (update.Message is not { } message)
                return;

            string replaceMessage = message.Text!.Replace("www.", "dd");

            if (message.Text == "/start")
            {
                ChatMember membership = await client.GetChatMemberAsync("@khasdfhsjdfhjasdzhlfk", userId: message.Chat.Id);

                if (membership != null && membership.Status != ChatMemberStatus.Member && membership.Status != ChatMemberStatus.Administrator && membership.Status != ChatMemberStatus.Creator)
                {
                   await UserSubscribeChecker();
                }

                else
                {
                    Subscription = true;
                    await client.SendTextMessageAsync(chatId: message.Chat.Id,text: "Send Video Link",cancellationToken: cancellation);
                }
            }

            else if (message.Text.StartsWith("https://www.instagram.com"))
            {
                if (Subscription == true)
                {
                    try
                    {
                        await client.SendVideoAsync(
                           chatId: message.Chat.Id,
                           video: $"{replaceMessage}",
                           supportsStreaming: true,
                           cancellationToken: cancellation);
                    }
                    catch (Exception) { }

                    try
                    {
                        await client.SendPhotoAsync(chatId: message.Chat.Id,photo: $"{replaceMessage}",cancellationToken: cancellation);
                    }
                    catch (Exception) { }
                }
                return;
            }


            async Task UserSubscribeChecker()
            {
                InlineKeyboardMarkup inlineKeyboard = new(new[]
                      {
                        new []
                        {
                            InlineKeyboardButton.WithUrl(text: "Channel", url: "https://t.me/khasdfhsjdfhjasdzhlfk"),
                        },
                      }
                );

                await client.SendTextMessageAsync(chatId: message!.Chat.Id, text: "/start - click", replyMarkup: inlineKeyboard, cancellationToken: cancellation);
            }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
