using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ModelDeliverySystemData.Model;
using System.Threading;
using Telegram.Bot.Polling;
using System.Collections.Generic;

namespace TelegramCourierBot
{
    public class Program
    {
        private static ITelegramBotClient botClient;
        private static dbContext _context = new dbContext();

        public static async Task Main(string[] args)
        {
            botClient = new TelegramBotClient("7349407796:AAG5lk8cLNUEX8jpsJAIh98Y_HUqUbLGOUo");
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Привет! Меня зовут {me.FirstName}");

            // Установка команд бота
            await botClient.SetMyCommandsAsync(new[]
            {
                new BotCommand { Command = "/start", Description = "Запустить бота" },
                new BotCommand { Command = "/register", Description = "Зарегистрироваться как курьер" },
                new BotCommand { Command = "/orders", Description = "Просмотреть текущие заказы" },
                new BotCommand { Command = "/allorders", Description = "Просмотреть все заказы" },
                new BotCommand { Command = "/profile", Description = "Просмотреть профиль" },
                new BotCommand { Command = "/support", Description = "Связаться с поддержкой" }
            });

            var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // Receive all update types
            };

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            Console.WriteLine("Нажмите любую клавишу для выхода");
            Console.ReadLine();

            cts.Cancel();
        }

        private static Dictionary<long, bool> awaitingPhoneInput = new Dictionary<long, bool>();
        private static Dictionary<long, string> pendingPhoneNumber = new Dictionary<long, string>();
        private static Dictionary<long, bool> awaitingLicenseInput = new Dictionary<long, bool>();
        private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message)
            {
                var message = update.Message;
                if (message.Type == MessageType.Text)
                {
                    if (awaitingPhoneInput.TryGetValue(message.Chat.Id, out bool awaitingPhone) && awaitingPhone)
                    {
                        pendingPhoneNumber[message.Chat.Id] = message.Text;
                        awaitingPhoneInput[message.Chat.Id] = false;
                        await RequestLicenseNumber(message.Chat.Id);
                        return;
                    }
                    if (awaitingLicenseInput.TryGetValue(message.Chat.Id, out bool awaitingLicense) && awaitingLicense)
                    {
                        if (pendingPhoneNumber.TryGetValue(message.Chat.Id, out string phoneNumber))
                        {
                            await RegisterCourier(message.Chat.Id, phoneNumber, message.Text);
                            awaitingLicenseInput[message.Chat.Id] = false;
                            pendingPhoneNumber.Remove(message.Chat.Id);
                        }
                        return;
                    }

                    switch (message.Text.Split(' ').First().ToLower())
                    {
                        case "/start":
                            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Добро пожаловать в Бот Курьера! Напишите /register, чтобы зарегистрироваться.");
                            break;
                        case "/register":
                            if (IsCourierRegistered(message.Chat.Id))
                            {
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Вы уже зарегистрированы как курьер.");
                            }
                            else
                            {
                                awaitingPhoneInput[message.Chat.Id] = true;
                                await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Пожалуйста, отправьте ваш номер телефона.");
                            }
                            break;
                        case "/orders":
                            await SendOrders(message.Chat.Id, false);
                            break;
                        case "/allorders":
                            await SendOrders(message.Chat.Id, true);
                            break;
                        case "/profile":
                            await SendProfile(message.Chat.Id);
                            break;
                        case "/support":
                            await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Для поддержки свяжитесь с нашим администратором: @quantrixor");
                            break;
                    }
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var callbackQuery = update.CallbackQuery;
                if (callbackQuery.Data.StartsWith("confirm_"))
                {
                    int orderId = int.Parse(callbackQuery.Data.Split('_')[1]);
                    await ConfirmOrder(callbackQuery.Message.Chat.Id, orderId);
                }
            }
        }

        private static bool IsCourierRegistered(long chatId)
        {
            return _context.Couriers.Any(c => c.TelegramChatId == chatId);
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка: {exception.Message}");
            return Task.CompletedTask;
        }

        private static async Task RegisterCourier(long chatId, string phoneNumber, string licenseNumber)
        {
            var courier = _context.Couriers.FirstOrDefault(c => c.PhoneNumber == phoneNumber && c.LicenseNumber == licenseNumber);
            if (courier != null)
            {
                courier.TelegramChatId = chatId; // Сохранение chatId в базу данных
                _context.SaveChanges();
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Вы успешно зарегистрированы как курьер."
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Курьер с указанным номером телефона и номером лицензии не найден."
                );
            }
        }

        // Этот метод можно вызывать после того, как курьер отправит свой номер телефона
        private static async Task RequestLicenseNumber(long chatId)
        {
            awaitingLicenseInput[chatId] = true; // Установка флага ожидания номера лицензии
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Введите ваш номер лицензии."
            );
        }

        private static async Task SendOrders(long chatId, bool allOrders)
        {
            var courier = _context.Couriers.FirstOrDefault(c => c.TelegramChatId == chatId);
            if (courier == null)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Вы не зарегистрированы как курьер."
                );
                return;
            }

            var orders = _context.Orders
                .Include("DeliveryAddresses")
                .Include("OrderedItems")
                .Where(o => o.Deliveries.Any(d => d.CourierID == courier.CourierID && (allOrders || d.StatusID == 1)))
                .ToList();

            if (orders.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "У вас нет назначенных заказов."
                );
                return;
            }

            foreach (var order in orders)
            {
                string orderDetails = $"Заказ ID: {order.OrderID}\nОбщая сумма: {order.TotalAmount}\nАдрес доставки: {order.DeliveryAddresses.FirstOrDefault()?.Address}\nТовары:\n";
                foreach (var item in order.OrderedItems)
                {
                    orderDetails += $"- {item.ProductName} x{item.Quantity}\n";
                }

                List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
                var delivery = order.Deliveries.FirstOrDefault(d => d.CourierID == courier.CourierID);
                if (delivery != null && delivery.StatusID == 1) // Если заказ еще не подтвержден
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Подтвердить заказ", $"confirm_{order.OrderID}") });
                }
                else if (delivery != null && delivery.StatusID == 2) // Если заказ уже подтвержден
                {
                    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Заказ уже подтвержден", $"confirmed_{order.OrderID}") });
                }

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(buttons);

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: orderDetails,
                    replyMarkup: inlineKeyboard
                );
            }
        }

        private static async Task SendProfile(long chatId)
        {
            var courier = _context.Couriers.FirstOrDefault(c => c.TelegramChatId == chatId);
            if (courier == null)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Вы не зарегистрированы как курьер."
                );
                return;
            }

            string profileDetails = $"Имя: {courier.FirstName}\nФамилия: {courier.LastName}\nНомер телефона: {courier.PhoneNumber}\nНомер лицензии: {courier.LicenseNumber}\nID транспортного средства: {courier.VehicleID}";

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: profileDetails
            );
        }

        private static async Task ConfirmOrder(long chatId, int orderId)
        {
            var order = _context.Orders
                .Include("Deliveries")
                .FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Заказ не найден."
                );
                return;
            }

            var delivery = order.Deliveries.FirstOrDefault(d => d.StatusID == 1);
            if (delivery != null)
            {
                // Изменяем статус доставки на 'Завершен'
                delivery.StatusID = 2; // Предполагаем, что статус 2 означает 'Завершен'
                delivery.ActualDeliveryDate = DateTime.Now;
                _context.SaveChanges();

                // Проверяем, есть ли еще незавершенные заказы у этого курьера
                bool hasPendingOrders = _context.Deliveries
                    .Any(d => d.CourierID == delivery.CourierID && d.StatusID == 1);

                if (!hasPendingOrders)
                {
                    // Если других заказов нет, обновляем статус курьера на 'Доступен'
                    var courier = _context.Couriers.Find(delivery.CourierID);
                    if (courier != null)
                    {
                        courier.IsAvailable = true;
                        _context.SaveChanges();
                        await botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: $"Все заказы выполнены. Вы свободны для новых заказов!"
                        );
                    }
                }

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Заказ {order.OrderID} подтвержден как выполненный."
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Невозможно подтвердить заказ."
                );
            }
        }

    }
}
