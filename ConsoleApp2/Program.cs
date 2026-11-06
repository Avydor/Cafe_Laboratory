using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackjackGame
{
    enum Suit { Червы = 1, Бубны, Крести, Пики }

    class Card
    {
        public Suit Suit { get; }
        public string Rank { get; }
        public int Value { get; }

        public Card(Suit suit, string rank, int value)
        {
            Suit = suit;
            Rank = rank;
            Value = value;
        }

        public override string ToString()
        {
            char suitSymbol = Suit switch
            {
                Suit.Червы => '♥',
                Suit.Бубны => '♦',
                Suit.Крести => '♣',
                Suit.Пики => '♠',
                _ => '?'
            };
            return $"[{Rank} {suitSymbol}]";
        }
    }

    class Deck
    {
        private List<Card> cards = new List<Card>();
        private Random random = new Random();

        public Deck() => Reset();

        public void Reset()
        {
            cards.Clear();
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Валет", "Дама", "Король", "Туз" };

            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                for (int i = 0; i < ranks.Length; i++)
                {
                    int value = i switch
                    {
                        < 9 => i + 2,
                        < 12 => 10,
                        _ => 11
                    };
                    cards.Add(new Card(suit, ranks[i], value));
                }
            }
            Shuffle();
        }

        public void Shuffle() => cards = cards.OrderBy(_ => random.Next()).ToList();

        public Card DrawCard()
        {
            if (cards.Count == 0) Reset();
            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }

    class Player
    {
        public string Name { get; }
        public List<Card> Hand { get; } = new List<Card>();

        public Player(string name) => Name = name;

        public void ClearHand() => Hand.Clear();

        public int GetScore()
        {
            int score = Hand.Sum(c => c.Value);
            int acesCount = Hand.Count(c => c.Rank == "Туз");

            while (score > 21 && acesCount > 0)
            {
                score -= 10;
                acesCount--;
            }
            return score;
        }

        public void ShowHand(bool hideSecondCard = false)
        {
            Console.Write($"{Name}: ");
            if (hideSecondCard && Hand.Count >= 2)
                Console.WriteLine($"{Hand[0]} [скрытая карта]");
            else
                Console.WriteLine(string.Join(" ", Hand) + $" | Очков: {GetScore()}");
        }
    }

    class Program
    {
        static int playerWins = 0;
        static int dealerWins = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Player player = new Player("Игрок");
            Player dealer = new Player("Дилер");

            // ГЛАВНЫЙ ЦИКЛ ПЕРЕИГРАВШИХ
            while (true)
            {
                Console.Clear();
                PlayRound(player, dealer);

                Console.WriteLine($"\n📊 Общий счёт — Вы: {playerWins} | Дилер: {dealerWins}");
                Console.WriteLine("\nХочешь сыграть ещё раз? (Нажми 'Enter' или 'Д' для продолжения, любую другую клавишу для выхода)");

                string input = Console.ReadLine()?.Trim().ToLower();

                // Если пользователь ввел что-то кроме пустой строки, "д" или "y" — выходим из цикла
                if (!string.IsNullOrEmpty(input) && input != "д" && input != "y")
                {
                    Console.WriteLine("\nСпасибо за игру! До встречи!");
                    break;
                }
            }
        }

        static void PlayRound(Player player, Player dealer)
        {
            // Сбрасываем руки с прошлого раунда
            player.ClearHand();
            dealer.ClearHand();

            Deck deck = new Deck();

            // Раздача карт
            player.Hand.Add(deck.DrawCard());
            dealer.Hand.Add(deck.DrawCard());
            player.Hand.Add(deck.DrawCard());
            dealer.Hand.Add(deck.DrawCard());

            Console.WriteLine("=== 🎮 ИГРА 21 ОЧКО 🎮 ===\n");
            dealer.ShowHand(hideSecondCard: true);
            player.ShowHand();

            // Проверка на блэкджек у игрока
            if (player.GetScore() == 21)
            {
                Console.WriteLine("\n✨ Блэкджек! Ты победил с раздачи!");
                playerWins++;
                return;
            }

            // Ход игрока
            while (true)
            {
                Console.WriteLine("\n1 — Взять карту");
                Console.WriteLine("2 — Пас");
                Console.Write("Выбери действие: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Card drawn = deck.DrawCard();
                    player.Hand.Add(drawn);
                    Console.WriteLine($"\nВытянули: {drawn}");
                    player.ShowHand();

                    if (player.GetScore() > 21)
                    {
                        Console.WriteLine("\n❌ Перебор! Ты проиграл.");
                        dealerWins++;
                        return;
                    }
                    if (player.GetScore() == 21) break;
                }
                else if (choice == "2")
                {
                    break;
                }
            }

            // Ход дилера
            Console.WriteLine("\n--- Ход дилера ---");
            dealer.ShowHand();

            while (dealer.GetScore() < 17)
            {
                Card drawn = deck.DrawCard();
                dealer.Hand.Add(drawn);
                Console.WriteLine($"Дилер берёт карту: {drawn}");
                dealer.ShowHand();
            }

            // Итоги
            int pScore = player.GetScore();
            int dScore = dealer.GetScore();

            Console.WriteLine("\n=== ИТОГИ ===");
            Console.WriteLine($"Очки игрока: {pScore}");
            Console.WriteLine($"Очки дилера: {dScore}\n");

            if (dScore > 21 || pScore > dScore)
            {
                Console.WriteLine("🎉 Ты выиграл!");
                playerWins++;
            }
            else if (pScore < dScore)
            {
                Console.WriteLine("❌ Дилер выиграл.");
                dealerWins++;
            }
            else
            {
                Console.WriteLine("🤝 Ничья!");
            }
        }
    }
}