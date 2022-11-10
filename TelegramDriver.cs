using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleWEBD
{
    public class TelegramDriver
    {
        #region Variables
        private static string hashmoment = "";
        private static bool addtaskcom = false;
        private static bool addcompany = false;
        private static bool addtask = false;
        private static long client = 0;
        public static ITelegramBotClient bot = new TelegramBotClient("5789330768:AAHOJx9AIbXQAHwRuMT32wxArIyTOciupzk");
        public static Company company = new Company();
        #endregion

        #region StartFunc
        public static void Start()
        {
            try
            {
                var cts = new CancellationTokenSource();
                var cancellationToken = cts.Token;
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { },
                };

                bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            #region Markups
            ReplyKeyboardMarkup replyKeyboardMarkupCompany = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Мои задачи", "Список" }, new KeyboardButton[] { "Создать", "Назад"} }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup replyKeyboardMarkupNewPartners = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Да", "Нет" } }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup replyKeyboardMarkupAddTask0 = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Пропустить" }, new KeyboardButton[] { "Добавить коментарий" }, new KeyboardButton[] { "Назад" } }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup replyKeyboardMarkupMain = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Добавить задачу", "Мои задачи" }, new KeyboardButton[] { "Настройки", "Компании" } }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup replyKeyboardMarkupMyTasks = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Приступит", "В рвботе", "Завершенные" } }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup replyKeyboardMarkupMainWithCompany = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "Добавить задачу", "Список задач" }, new KeyboardButton[] { "Настройки", "Компания" } }) { ResizeKeyboard = true };
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new[] { new KeyboardButton[] { "" } }) { ResizeKeyboard = true };

            #endregion

            #region Inlines
            InlineKeyboardMarkup InlineKeyboardRatin = (new[] { new[] { InlineKeyboardButton.WithCallbackData(text: "⭐️", callbackData: $"1"), InlineKeyboardButton.WithCallbackData(text: "⭐️⭐️", callbackData: $"2"), InlineKeyboardButton.WithCallbackData(text: "⭐️⭐️⭐️", callbackData: $"3"),}, });
            InlineKeyboardMarkup InlineKeyboardDefault = (new[] { new[] { InlineKeyboardButton.WithCallbackData(text: "⭐️", callbackData: $"1") }, });
            #endregion
            
            try
            {

                #region Callback
                if (update.CallbackQuery != null)
                {

                    string str = update.CallbackQuery.Data;
                    long TID = update.CallbackQuery.From.Id;

                    #region AddTask
                    if (str == "1")
                    {
                        Requests.Update($"UPDATE Task SET Rating = {1} WHERE Hash = '{hashmoment}'");
                        await bot.SendTextMessageAsync(TID, $"Ты можешь добавить комментарий к задаче", replyMarkup: replyKeyboardMarkupAddTask0);
                    }
                    else if (str == "2")
                    {
                        Requests.Update($"UPDATE Task SET Rating = {2} WHERE Hash = '{hashmoment}'");
                        await bot.SendTextMessageAsync(TID, $"Ты можешь добавить комментарий к задаче", replyMarkup: replyKeyboardMarkupAddTask0);
                    }
                    else if (str == "3")
                    {
                        Requests.Update($"UPDATE Task SET Rating = {3} WHERE Hash = '{hashmoment}'");
                        await bot.SendTextMessageAsync(TID, $"Ты можешь добавить комментарий к задаче", replyMarkup: replyKeyboardMarkupAddTask0);
                    }

                    #endregion

                    #region CompletedTask
                    if (str.Length == 8)
                    {
                        string task = Requests.Return($"SELECT Title FROM Task WHERE Hash = '{str}'");
                        if (task.Length > 10)
                        {
                            await bot.SendTextMessageAsync(TID, $"Задача\n{task.Remove(10)}....\n<b>Выполнена</b>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                            Requests.Update($"UPDATE Task SET Status = 1 WHERE Hash = '{str}'");
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(TID, $"Задача\n{task}....\n<b>Выполнена</b>", parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                            //TODO: Requst to completed task
                        }
                        Requests.Update($"UPDATE Task SET Status = 1 WHERE Hash = '{str}'");
                    }
                    #endregion

                    #region ShowComment

                    if (str.Length == 9)
                    {
                        string hash = str.Remove(8);
                        string comment = Requests.Return($"SELECT Body FROM Task WHERE Hash = '{hash}'");
                        await bot.SendTextMessageAsync(TID, $"<b>Коментарий</b>\n \"{comment}\"", parseMode: ParseMode.Html);
                    }
                    #endregion

                    #region ShowPart
                    if (str.Remove(1) == "v")
                    {
                        string hash = Regex.Match(str, "v([A-z0-9]*)").Groups[1].Value;
                        Requests.MyPartners(hash, TID);
                    }
                    #endregion

                }
                #endregion

                #region Buttons

                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    long TID = message.Chat.Id;
                    if (message.Text != null)
                    {
                        #region Start
                        if (message.Text.ToLower() == "/start")
                        {
                            
                            Checking(message, TID, replyKeyboardMarkupMain, replyKeyboardMarkupMainWithCompany);
                        }
                        #endregion


                        #region Partners
                        if (message.Text.Length == 14)
                        {
                            if (message.Text.Remove(4) == "join")
                            {
                                string hashc = message.Text.Replace("join", "");
                                //if (!Requests.Check("Hash", TID))
                                //{
                                //    await bot.SendTextMessageAsync(TID, $"Ты у нас впервые, приветствую");
                                //}
                                string partners = Requests.Return($"SELECT Partners FROM Company WHERE Hash = '{hashc}'");
                                partners = partners + TID.ToString() + ";";
                                Requests.Update($"UPDATE Company SET Partners = '{partners}' WHERE Hash = '{hashc}'");
                                await bot.SendTextMessageAsync(TID,
                                    $"Привет, ты вступил в компанию <b>\"{Requests.Return($"SELECT Name FROM Company WHERE Hash = '{hashc}'")}\"</b>" +
                                    $"\n\n" +
                                    $"В этом боте можно:\n" +
                                    $"● Рабоать со своимим списками задач\n" +
                                    $"● Назначать задачи другим людям в компании" +
                                    $"● Составлять списки хотелок для дня рождения\n" +
                                    $"● Ставить напоминалки о встречах или дедлайнах\n" +
                                    $"<b>И всё это в твоем мессенджере!</b>", replyMarkup: replyKeyboardMarkupMain, parseMode: ParseMode.Html);

                            }
                        }
                        #endregion


                        #region Back 
                        if (message.Text == "Назад")
                        {
                            Back(TID, replyKeyboardMarkupMain, replyKeyboardMarkupMain, "Меню");
                        }
                        #endregion


                        #region AddTask
                        if (addtaskcom && TID == client)
                        {
                            string body = message.Text;
                            Requests.Update($"UPDATE Task SET Body = '{body}' WHERE Hash = '{hashmoment}'");
                            addtaskcom = false;
                            client = 0;
                            await bot.SendTextMessageAsync(TID, $"Коментарий:\n\n" +
                                $"\"{body}\"\n\n");
                            Back(TID, replyKeyboardMarkupMain, replyKeyboardMarkupMainWithCompany, "Задача добавлена!");
                        }

                        if (addtask && TID == client)
                        {
                            addtask = false;
                            client = 0;
                            string title = message.Text;
                            hashmoment = Requests.AddTask(title, "", "", TID, TID, 0, 0);
                            await bot.SendTextMessageAsync(TID, $"Твоя задача:\n" +
                                $"{title}\n" +
                                $"Ниже выбери степень её важности", replyMarkup: InlineKeyboardRatin);
                        }

                        if (message.Text == "Добавить коментарий")
                        {
                            await bot.SendTextMessageAsync(TID, "Отправь мне коментарий");
                            addtaskcom = true;
                            client = TID;
                        }

                        if (message.Text == "Пропустить")
                        {
                            Back(TID, replyKeyboardMarkupMain, replyKeyboardMarkupMainWithCompany, "Задача добавлена!");
                        }

                        if (message.Text == "Добавить задачу")
                        {
                            await bot.SendTextMessageAsync(TID, "Отправь мне краткое описание своей задачи");
                            addtask = true;
                            client = TID;

                        }
                        #endregion


                        #region AddCompany
                        if (addcompany)
                        {
                            addcompany = false;
                            await bot.SendTextMessageAsync(TID,
                                $"Привет <b>{message.Chat.FirstName}</b>, приглашает тебя\nв компанию <b>'{message.Text}'</b>," +
                                $"чтоб ты присоединился введи в боте этот " +
                                $"код (В любом месте):\n\n<pre>join{EditCompany.AddNameCom(message.Text, TID)}</pre>", parseMode: ParseMode.Html, replyMarkup: replyKeyboardMarkupMain);
                            
                        }
                        #endregion


                        #region ListTask
                        if (message.Text == "Мои задачи")
                        {
                            Requests.ReturnMyTasks(TID, TID);
                        }

                        if (message.Text == "Список задач")
                        {

                        }
                        #endregion


                        #region Company
                        if (message.Text == "Компании")
                        {
                            await bot.SendTextMessageAsync(TID, $"Выбери действия", replyMarkup: replyKeyboardMarkupCompany, parseMode: ParseMode.Html);
                            if (Requests.Return($"SELECT Company FROM Users WHERE TID = '{TID}'") == "0")
                            {
                                
                            }
                        }

                        if (message.Text == "Список")
                        {
                            await bot.SendTextMessageAsync(TID, $"Выбери компанию ниже:");
                            string hashc = Requests.Return($"SELECT Hash FROM Users WHERE TID = '{TID}'");
                            Requests.MyCompany(hashc, TID);
                        }

                        if (message.Text == "Создать")
                        {
                            await bot.SendTextMessageAsync(TID, $"Отправь название своей компани");
                            addcompany = true;
                        }
                        #endregion
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Requests.CloseConnection();
                Logger.Error(ex);
            }
        }
        
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await Task.Run(() => { Logger.Error(exception); });
        }
        private async static void Back(long TID, ReplyKeyboardMarkup replyKeyboardMarkupMain, ReplyKeyboardMarkup replyKeyboardMarkupMainWithCompany, string txt)
        {
            addtask = false;
            addtaskcom = false;
            if (!Requests.Check("Company", TID))
            {
                await bot.SendTextMessageAsync(TID, txt, replyMarkup: replyKeyboardMarkupMain);
            }
            else
            {
                await bot.SendTextMessageAsync(TID, txt, replyMarkup: replyKeyboardMarkupMainWithCompany);
            }
        }
        private static async void Checking(Message message, long TID, ReplyKeyboardMarkup replyKeyboardMarkupMain, ReplyKeyboardMarkup replyKeyboardMarkupMainWithCompany)
        {
            if (!Requests.Check("Hash", TID))
            {
                if (message.Chat.LastName != null)
                {
                    Requests.AddUser(TID, message.Chat.FirstName, message.Chat.LastName, "test;", "0", "0", 0);
                }
                else
                {
                    Requests.AddUser(TID, message.Chat.FirstName, "0", "test;", "0", "0", 0);
                }

                if (!Requests.Check("Company", TID))
                {
                    await bot.SendTextMessageAsync(TID, "Меню", replyMarkup: replyKeyboardMarkupMain);
                }
                else
                {
                    await bot.SendTextMessageAsync(TID, "Меню", replyMarkup: replyKeyboardMarkupMainWithCompany);
                }
            }
            else
            {
                await bot.SendTextMessageAsync(TID, $"С возвращением {message.Chat.FirstName}, я тебя помню!", replyMarkup: replyKeyboardMarkupMain);
            }
        }
    }       
}
