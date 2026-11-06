using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Random random = new Random();

        int balance = 200;
        int clickerLevel = 1;
        int clickerUpgradeCost = 50;
        int luckLevel = 1;
        int luckUpgradeCost = 100;

        string[] symbols = { "🍎", "🍋", "🍒", "💎", "7️⃣" };

        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("           ГЛАВНОЕ МЕНЮ                ");
            Console.WriteLine("========================================");
            Console.WriteLine($"💰 Ваш баланс: {balance} монет");
            Console.WriteLine($"🏭 Уровень завода: {clickerLevel} (Доход: {clickerLevel * 5}/клик)");
            Console.WriteLine($"🎰 Уровень удачи: {luckLevel} (+{(luckLevel - 1) * 5}% в слотах)");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("1. Пойти работать на завод 🏭");
            Console.WriteLine("2. Сыграть в слоты 🎰");
            Console.WriteLine("3. Полноценный Покер (Texas Hold'em) 🃏");
            Console.WriteLine("4. Магазин улучшений 🛒");
            Console.WriteLine("5. Выйти из игры 🚪");
            Console.WriteLine("----------------------------------------");
            Console.Write("Выберите действие (1-5): ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                // --- ЗАВОД ---
                Console.Clear();
                Console.WriteLine("=== ЗАВОД \"ДЕТАЛЬ-МАШИНА\" ===");
                Console.WriteLine($"Каждое нажатие [ENTER] приносит: {clickerLevel * 5} монет.");
                Console.WriteLine("Введите 'x' и нажмите ENTER для выхода.\n");

                while (true)
                {
                    Console.Write($"Нажмите [ENTER] (Баланс: {balance}) ");
                    string workInput = Console.ReadLine();
                    if (workInput.ToLower() == "x") break;

                    int earned = clickerLevel * 5;
                    balance += earned;

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚙️ Деталь готова! +{earned} монет.");
                    Console.ResetColor();
                }
            }
            else if (choice == "2")
            {
                // --- СЛОТЫ ---
                int bet = 10;
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=== КАЗИНО СЛОТЫ ===");
                    Console.WriteLine($"Баланс: {balance} монет | Ставка: {bet} монет\n");

                    if (balance < bet)
                    {
                        Console.WriteLine("❌ Недостаточно средств!");
                        Console.ReadKey();
                        break;
                    }

                    Console.Write("Нажмите [ENTER] чтобы крутить (или 'x' для выхода): ");
                    string casinoInput = Console.ReadLine();
                    if (casinoInput.ToLower() == "x") break;

                    balance -= bet;

                    string slot1 = "", slot2 = "", slot3 = "";
                    for (int i = 0; i < 12; i++)
                    {
                        slot1 = symbols[random.Next(symbols.Length)];
                        slot2 = symbols[random.Next(symbols.Length)];
                        slot3 = symbols[random.Next(symbols.Length)];

                        Console.Write($"\r[ {slot1} | {slot2} | {slot3} ]");
                        Thread.Sleep(80);
                    }

                    int luckBonusChance = (luckLevel - 1) * 5;
                    if (random.Next(1, 101) <= luckBonusChance)
                    {
                        slot2 = slot1;
                        slot3 = slot1;
                        Console.Write($"\r[ {slot1} | {slot2} | {slot3} ] ⭐ Удача!");
                    }

                    Console.WriteLine();

                    if (slot1 == slot2 && slot2 == slot3)
                    {
                        int winAmount = bet * 5;
                        balance += winAmount;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"🎉 ДЖЕКПОТ! Вы выиграли {winAmount} монет!\n");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Проигрыш!\n");
                        Console.ResetColor();
                    }

                    Thread.Sleep(500);
                }
            }
            else if (choice == "3")
            {
                // --- ПОЛНОЦЕННЫЙ ТЕХАССКИЙ ПОКЕР ---
                int smallBlind = 10;

                while (true)
                {
                    Console.Clear();
                    if (balance < smallBlind)
                    {
                        Console.WriteLine("❌ Недостаточно денег для малого блайнда (нужно минимум 10 монет)!");
                        Console.WriteLine("Нажмите любую клавишу для выхода...");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine("=== ТЕХАССКИЙ ХОЛДЕМ ПОКЕР ===");
                    Console.WriteLine($"Баланс: {balance} монет | Малый блайнд: {smallBlind} монет");
                    Console.Write("Нажмите [ENTER] чтобы начать раздачу (или 'x' для выхода): ");
                    string pInput = Console.ReadLine();
                    if (pInput.ToLower() == "x") break;

                    // Генерация и перемешивание колоды
                    string[] suits = { "♠", "♥", "♦", "♣" };
                    List<(int Rank, string Suit)> deck = new List<(int, string)>();
                    for (int r = 2; r <= 14; r++)
                        foreach (var s in suits)
                            deck.Add((r, s));

                    deck = deck.OrderBy(x => random.Next()).ToList();

                    // Раздача карманных карт
                    var playerHand = new List<(int Rank, string Suit)> { deck[0], deck[1] };
                    var dealerHand = new List<(int Rank, string Suit)> { deck[2], deck[3] };
                    var tableCards = new List<(int Rank, string Suit)>();

                    int deckIndex = 4;
                    int pot = 0;
                    int playerBet = 0;
                    int dealerBet = 0;
                    int dealerBalance = 500; // Баланс дилера

                    // Слепые ставки (Блайнды)
                    playerBet += smallBlind;
                    balance -= smallBlind;
                    dealerBet += smallBlind * 2;
                    dealerBalance -= smallBlind * 2;
                    pot = playerBet + dealerBet;

                    bool playerFolded = false;
                    bool dealerFolded = false;

                    // Игровые раунды: 0 - Префлоп, 1 - Флоп, 2 - Терн, 3 - Ривер
                    string[] roundNames = { "ПРЕФЛОП", "ФЛОП", "ТЕРН", "РИВЕР" };

                    for (int round = 0; round < 4; round++)
                    {
                        // Выдача карт на стол
                        if (round == 1) // Флоп
                        {
                            tableCards.Add(deck[deckIndex++]);
                            tableCards.Add(deck[deckIndex++]);
                            tableCards.Add(deck[deckIndex++]);
                        }
                        else if (round == 2 || round == 3) // Терн и Ривер
                        {
                            tableCards.Add(deck[deckIndex++]);
                        }

                        // Раунд торговли
                        bool bettingRound = true;
                        int currentCall = Math.Max(playerBet, dealerBet);

                        while (bettingRound)
                        {
                            Console.Clear();
                            Console.WriteLine($"=== РАУНД: {roundNames[round]} ===");
                            Console.WriteLine($"💰 БАНК (ПОТ): {pot} монет");
                            Console.WriteLine($"💵 Ваш баланс: {balance} | Ваша ставка в раунде: {playerBet}");
                            Console.WriteLine($"🤖 Баланс Дилера: {dealerBalance} | Ставка Дилера: {dealerBet}");
                            Console.WriteLine("----------------------------------------");
                            Console.WriteLine($"🎴 Ваши карты:   {FormatCard(playerHand[0])} {FormatCard(playerHand[1])}");

                            Console.Write("🎴 Карты стола:  ");
                            if (tableCards.Count == 0) Console.Write("[ Появится на Флопе ]");
                            else foreach (var c in tableCards) Console.Write($"{FormatCard(c)} ");
                            Console.WriteLine("\n----------------------------------------");

                            int callAmount = dealerBet - playerBet;

                            Console.WriteLine("Выберите действие:");
                            if (callAmount == 0)
                            {
                                Console.WriteLine("1. Check (Чек - пропустить ход)");
                                Console.WriteLine("2. Raise (Поднять ставку)");
                                Console.WriteLine("3. Fold (Пасс - сбросить карты)");
                            }
                            else
                            {
                                Console.WriteLine($"1. Call (Уравнять {callAmount} монет)");
                                Console.WriteLine("2. Raise (Поднять ставку)");
                                Console.WriteLine("3. Fold (Пасс - сбросить карты)");
                            }

                            Console.Write("> ");
                            string action = Console.ReadLine();

                            if (action == "1") // Check / Call
                            {
                                if (callAmount > 0)
                                {
                                    if (balance < callAmount)
                                    {
                                        Console.WriteLine("Недостаточно денег для Call! Все монеты идут в ход.");
                                        callAmount = balance;
                                    }
                                    balance -= callAmount;
                                    playerBet += callAmount;
                                    pot += callAmount;
                                }
                                bettingRound = false;
                            }
                            else if (action == "2") // Raise
                            {
                                Console.Write("Введите сумму поднятия (минимум 10): ");
                                if (int.TryParse(Console.ReadLine(), out int raiseVal) && raiseVal >= 10)
                                {
                                    int totalToPay = callAmount + raiseVal;
                                    if (balance >= totalToPay)
                                    {
                                        balance -= totalToPay;
                                        playerBet += totalToPay;
                                        pot += totalToPay;
                                        bettingRound = false;
                                    }
                                    else
                                    {
                                        Console.WriteLine("❌ Не хватает денег для такого повышения!");
                                        Thread.Sleep(1000);
                                        continue;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("❌ Некорректная сумма!");
                                    Thread.Sleep(1000);
                                    continue;
                                }
                            }
                            else if (action == "3") // Fold
                            {
                                playerFolded = true;
                                bettingRound = false;
                                break;
                            }

                            // Ход Дилера (Простой ИИ)
                            if (!playerFolded && playerBet > dealerBet)
                            {
                                int diff = playerBet - dealerBet;
                                var dealerEval = EvaluateHand(dealerHand.Concat(tableCards).ToList());

                                // Дилер сбрасывает если ставка слишком велика при слабой карте
                                if (diff > 50 && dealerEval.Score < 200 && random.Next(1, 100) > 30)
                                {
                                    Console.WriteLine("\n🤖 Дилер сбросил карты (Fold)!");
                                    dealerFolded = true;
                                    Thread.Sleep(1500);
                                }
                                else
                                {
                                    Console.WriteLine($"\n🤖 Дилер уравнял вашу ставку (+{diff} монет).");
                                    dealerBalance -= diff;
                                    dealerBet += diff;
                                    pot += diff;
                                    Thread.Sleep(1500);
                                }
                            }
                        }

                        if (playerFolded || dealerFolded) break;
                    }

                    // Определение победы
                    Console.Clear();
                    Console.WriteLine("================ ПОДВЕДЕНИЕ ИТОГОВ ================");
                    Console.WriteLine($"🎴 Ваши карты:   {FormatCard(playerHand[0])} {FormatCard(playerHand[1])}");
                    Console.WriteLine($"🤖 Карты Дилера: {FormatCard(dealerHand[0])} {FormatCard(dealerHand[1])}");
                    Console.Write("🎴 Стол:         ");
                    foreach (var c in tableCards) Console.Write($"{FormatCard(c)} ");
                    Console.WriteLine("\n---------------------------------------------------");

                    if (playerFolded)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Вы сбросили карты (Fold). Дилер забирает банк!");
                        Console.ResetColor();
                    }
                    else if (dealerFolded)
                    {
                        balance += pot;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"🎉 Дилер пасовал! Вы забираете банк {pot} монет!");
                        Console.ResetColor();
                    }
                    else
                    {
                        var playerEval = EvaluateHand(playerHand.Concat(tableCards).ToList());
                        var dealerEval = EvaluateHand(dealerHand.Concat(tableCards).ToList());

                        Console.WriteLine($"Ваша комбинация:   {playerEval.Name}");
                        Console.WriteLine($"Комбинация Дилера: {dealerEval.Name}");
                        Console.WriteLine("---------------------------------------------------");

                        if (playerEval.Score > dealerEval.Score)
                        {
                            balance += pot;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"🎉 ВЫ ПОБЕДИЛИ! Заработок: +{pot} монет!");
                            Console.ResetColor();
                        }
                        else if (playerEval.Score < dealerEval.Score)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"❌ Дилер выиграл банк ({pot} монет).");
                            Console.ResetColor();
                        }
                        else
                        {
                            balance += pot / 2;
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("🤝 НИЧЬЯ! Банк делится пополам.");
                            Console.ResetColor();
                        }
                    }

                    Console.WriteLine("\nНажмите ENTER для продолжения...");
                    Console.ReadLine();
                }
            }
            else if (choice == "4")
            {
                // --- МАГАЗИН ---
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("=== МАГАЗИН УЛУЧШЕНИЙ 🛒 ===");
                    Console.WriteLine($"Ваш баланс: {balance} монет\n");
                    Console.WriteLine($"1. Модернизация завода (Ур. {clickerLevel} -> {clickerLevel + 1})");
                    Console.WriteLine($"   Цена: {clickerUpgradeCost} монет | Доход: +5 монет за деталь");
                    Console.WriteLine();
                    Console.WriteLine($"2. Подкрутка слотов (Ур. {luckLevel} -> {luckLevel + 1})");
                    Console.WriteLine($"   Цена: {luckUpgradeCost} монет | Шанс: +5% к удаче");
                    Console.WriteLine();
                    Console.WriteLine("3. Вернуться в главное меню");
                    Console.WriteLine("----------------------------------------");
                    Console.Write("Выберите вариант (1-3): ");

                    string shopChoice = Console.ReadLine();

                    if (shopChoice == "1")
                    {
                        if (balance >= clickerUpgradeCost)
                        {
                            balance -= clickerUpgradeCost;
                            clickerLevel++;
                            clickerUpgradeCost = (int)(clickerUpgradeCost * 1.8);
                            Console.WriteLine("\n✅ Завод улучшен!");
                        }
                        else Console.WriteLine("\n❌ Не хватает денег!");
                        Thread.Sleep(1000);
                    }
                    else if (shopChoice == "2")
                    {
                        if (balance >= luckUpgradeCost)
                        {
                            balance -= luckUpgradeCost;
                            luckLevel++;
                            luckUpgradeCost = (int)(luckUpgradeCost * 2.2);
                            Console.WriteLine("\n✅ Удача повышена!");
                        }
                        else Console.WriteLine("\n❌ Не хватает денег!");
                        Thread.Sleep(1000);
                    }
                    else if (shopChoice == "3") break;
                }
            }
            else if (choice == "5")
            {
                Console.WriteLine("\nСпасибо за игру! До встречи!");
                break;
            }
        }
    }

    static string FormatCard((int Rank, string Suit) card)
    {
        string rankStr = card.Rank switch
        {
            11 => "J",
            12 => "Q",
            13 => "K",
            14 => "A",
            _ => card.Rank.ToString()
        };
        return $"[{rankStr}{card.Suit}]";
    }

    static (int Score, string Name) EvaluateHand(List<(int Rank, string Suit)> cards)
    {
        if (cards == null || cards.Count == 0) return (0, "Нет карт");

        var ranks = cards.Select(c => c.Rank).OrderByDescending(r => r).ToList();
        var rankCounts = ranks.GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());

        bool isStraight = false;
        var distinctRanks = ranks.Distinct().ToList();
        if (distinctRanks.Count >= 5)
        {
            for (int i = 0; i <= distinctRanks.Count - 5; i++)
            {
                if (distinctRanks[i] - distinctRanks[i + 4] == 4)
                {
                    isStraight = true;
                    break;
                }
            }
        }

        if (isStraight) return (500 + ranks[0], "Стрит 🎴");
        if (rankCounts.ContainsValue(3)) return (400 + rankCounts.First(x => x.Value == 3).Key, "Тройка (Сет) 💎");
        if (rankCounts.Values.Count(v => v == 2) >= 2) return (300 + rankCounts.Where(x => x.Value == 2).Max(x => x.Key), "Две Пары ✌️");
        if (rankCounts.ContainsValue(2)) return (200 + rankCounts.First(x => x.Value == 2).Key, "Пара 👥");

        return (ranks[0], $"Старшая карта ({FormatCard((ranks[0], ""))})");
    }
}