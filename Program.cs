using System;

namespace Casino
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Casino Game";
            Console.CursorVisible = false;

            // Initialize game
            CasinoGame game = new CasinoGame();
            game.Run();
        }
    }

    class Card
    {
        public string Suit { get; }
        public string Rank { get; }
        public int Value { get; }

        public Card(string suit, string rank, int value)
        {
            Suit = suit;
            Rank = rank;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Rank}{Suit[0]}";
        }
    }

    class Deck
    {
        private List<Card> cards;
        private Random random = new Random();

        public Deck()
        {
            cards = new List<Card>();
            string[] suits = { "♠", "♥", "♦", "♣" };
            string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
            int[] values = { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10 };

            foreach (string suit in suits)
            {
                for (int i = 0; i < ranks.Length; i++)
                {
                    cards.Add(new Card(suit, ranks[i], values[i]));
                }
            }
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        public Card Draw()
        {
            if (cards.Count == 0) throw new InvalidOperationException("Deck is empty");
            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            return card;
        }

        public int CardsRemaining()
        {
            return cards.Count;
        }
    }

    class BlackjackGame
    {
        private Deck deck;
        private List<Card> playerHand;
        private List<Card> dealerHand;
        private List<Card> splitHand;
        private int playerBet;
        private int splitBet;
        private bool gameOver;
        private bool hasSplit;
        private bool playingSplitHand;
        private int runningCount;
        private bool insuranceTaken;
        private int insuranceBet;

        public BlackjackGame()
        {
            deck = new Deck();
            playerHand = new List<Card>();
            dealerHand = new List<Card>();
            splitHand = new List<Card>();
            runningCount = 0;
        }

        public void Play(CasinoGame casino)
        {
            while (true)
            {
                // Place bet
                playerBet = GetBet(casino.playerBalance);
                if (playerBet == 0) return; // Player chose to go back

                casino.playerBalance -= playerBet;

                // Start new game
                StartNewGame();

                // Player's turn
                PlayerTurn(casino);

                if (!gameOver)
                {
                    // Dealer's turn
                    DealerTurn();
                }

                // Determine winner
                DetermineWinner(casino);

                // Ask to play again
                if (!PlayAgain()) break;
            }
        }

        private int GetBet(int balance)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║              BLACKJACK               ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine($"║ Balance: ${balance,-26} ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("Enter bet amount (or 0 to go back): $");

            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int bet))
                {
                    if (bet == 0) return 0;
                    if (bet > 0 && bet <= balance) return bet;
                }
                Console.Write("Invalid bet. Enter bet amount: $");
            }
        }

        private void StartNewGame()
        {
            deck = new Deck();
            deck.Shuffle();
            playerHand.Clear();
            dealerHand.Clear();
            splitHand.Clear();
            gameOver = false;
            hasSplit = false;
            playingSplitHand = false;
            insuranceTaken = false;
            insuranceBet = 0;

            // Deal initial cards
            playerHand.Add(deck.Draw()!);
            dealerHand.Add(deck.Draw()!);
            playerHand.Add(deck.Draw()!);
            dealerHand.Add(deck.Draw()!);
            
            // Update running count
            UpdateCount(playerHand[0]);
            UpdateCount(playerHand[1]);
            UpdateCount(dealerHand[0]);
        }

        private void PlayerTurn(CasinoGame casino)
        {
            // Check for insurance if dealer shows Ace
            if (dealerHand[0].Rank == "A" && !insuranceTaken)
            {
                DisplayGame(false);
                Console.WriteLine("Dealer shows Ace! Take insurance? (Y/N)");
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Y && casino.playerBalance >= playerBet / 2)
                {
                    insuranceBet = playerBet / 2;
                    casino.playerBalance -= insuranceBet;
                    insuranceTaken = true;
                    Console.WriteLine($"Insurance bet placed: ${insuranceBet}");
                    Thread.Sleep(1000);
                }
            }
            
            // Check for blackjack
            if (GetHandValue(playerHand) == 21)
            {
                gameOver = true;
                return;
            }
            
            bool firstTurn = true;
            while (true)
            {
                DisplayGame(false);
                
                // Build menu
                string menu = "1. Hit  2. Stand";
                if (firstTurn && casino.playerBalance >= playerBet)
                    menu += "  3. Double Down";
                if (firstTurn && CanSplit() && casino.playerBalance >= playerBet && !hasSplit)
                    menu += "  4. Split";
                if (firstTurn)
                    menu += "  5. Surrender";
                
                Console.WriteLine(menu);
                Console.Write("Choose action: ");

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Hit();
                        firstTurn = false;
                        if (GetHandValue(GetCurrentHand()) > 21)
                        {
                            if (hasSplit && !playingSplitHand)
                            {
                                playingSplitHand = true;
                                firstTurn = true;
                            }
                            else
                            {
                                gameOver = true;
                                return;
                            }
                        }
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        if (hasSplit && !playingSplitHand)
                        {
                            playingSplitHand = true;
                            firstTurn = true;
                        }
                        else
                        {
                            return; // Stand
                        }
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        if (firstTurn && casino.playerBalance >= playerBet)
                        {
                            casino.playerBalance -= playerBet;
                            if (playingSplitHand)
                                splitBet *= 2;
                            else
                                playerBet *= 2;
                            Hit();
                            if (hasSplit && !playingSplitHand)
                            {
                                playingSplitHand = true;
                            }
                            else
                            {
                                return; // Must stand after double down
                            }
                        }
                        break;
                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:
                        if (firstTurn && CanSplit() && casino.playerBalance >= playerBet && !hasSplit)
                        {
                            casino.playerBalance -= playerBet;
                            splitBet = playerBet;
                            hasSplit = true;
                            splitHand.Add(playerHand[1]);
                            playerHand.RemoveAt(1);
                            playerHand.Add(deck.Draw());
                            splitHand.Add(deck.Draw());
                            UpdateCount(playerHand[1]);
                            UpdateCount(splitHand[1]);
                            firstTurn = false;
                        }
                        break;
                    case ConsoleKey.D5:
                    case ConsoleKey.NumPad5:
                        if (firstTurn)
                        {
                            // Surrender - get half bet back
                            casino.playerBalance += playerBet / 2;
                            gameOver = true;
                            Console.WriteLine("\nSurrendered. Half bet returned.");
                            Thread.Sleep(1500);
                            return;
                        }
                        break;
                }
            }
        }

        private void DealerTurn()
        {
            while (GetHandValue(dealerHand) < 17)
            {
                Card drawn = deck.Draw();
                dealerHand.Add(drawn);
                UpdateCount(drawn);
                DisplayGame(true);
                Thread.Sleep(1000);
            }
        }

        private void DetermineWinner(CasinoGame casino)
        {
            DisplayGame(true);

            int playerValue = GetHandValue(playerHand);
            int dealerValue = GetHandValue(dealerHand);
            bool dealerBlackjack = dealerValue == 21 && dealerHand.Count == 2;
            bool playerBlackjack = playerValue == 21 && playerHand.Count == 2;

            // Check insurance
            if (insuranceTaken)
            {
                if (dealerBlackjack)
                {
                    casino.playerBalance += insuranceBet * 3; // Insurance pays 2:1 plus original bet
                    Console.WriteLine($"✅ Insurance wins! +${insuranceBet * 2}");
                }
                else
                {
                    Console.WriteLine($"❌ Insurance loses. -${insuranceBet}");
                }
                Thread.Sleep(1000);
            }

            // Main hand result
            if (playerValue > 21)
            {
                Console.WriteLine("💥 Player busts! Dealer wins.");
            }
            else if (dealerValue > 21)
            {
                Console.WriteLine("💥 Dealer busts! Player wins.");
                int payout = playerBlackjack ? (int)(playerBet * 2.5) : playerBet * 2;
                casino.playerBalance += payout;
            }
            else if (playerBlackjack && !dealerBlackjack)
            {
                Console.WriteLine("🃏 BLACKJACK! Player wins 3:2!");
                casino.playerBalance += (int)(playerBet * 2.5);
            }
            else if (dealerBlackjack && !playerBlackjack)
            {
                Console.WriteLine("🃏 Dealer has Blackjack! Dealer wins.");
            }
            else if (playerValue > dealerValue)
            {
                Console.WriteLine("🎉 Player wins!");
                casino.playerBalance += playerBet * 2;
            }
            else if (dealerValue > playerValue)
            {
                Console.WriteLine("😞 Dealer wins!");
            }
            else
            {
                Console.WriteLine("🤝 Push! It's a tie.");
                casino.playerBalance += playerBet;
            }

            // Split hand result
            if (hasSplit)
            {
                int splitValue = GetHandValue(splitHand);
                Console.WriteLine("\n--- Split Hand ---");
                if (splitValue > 21)
                {
                    Console.WriteLine("💥 Split hand busts! Dealer wins.");
                }
                else if (splitValue > dealerValue || dealerValue > 21)
                {
                    Console.WriteLine("🎉 Split hand wins!");
                    casino.playerBalance += splitBet * 2;
                }
                else if (dealerValue > splitValue)
                {
                    Console.WriteLine("😞 Split hand loses!");
                }
                else
                {
                    Console.WriteLine("🤝 Split hand pushes!");
                    casino.playerBalance += splitBet;
                }
            }

            Console.WriteLine($"\nNew balance: ${casino.playerBalance}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        private void Hit()
        {
            Card drawn = deck.Draw();
            GetCurrentHand().Add(drawn);
            UpdateCount(drawn);
        }

        private List<Card> GetCurrentHand()
        {
            return (hasSplit && playingSplitHand) ? splitHand : playerHand;
        }

        private bool CanSplit()
        {
            return playerHand.Count == 2 && playerHand[0].Rank == playerHand[1].Rank;
        }

        private void UpdateCount(Card card)
        {
            // Hi-Lo card counting system
            if (card.Value >= 2 && card.Value <= 6)
                runningCount++;
            else if (card.Value == 10 || card.Rank == "A")
                runningCount--;
        }

        private int GetHandValue(List<Card> hand)
        {
            int value = 0;
            int aces = 0;

            foreach (Card card in hand)
            {
                if (card.Rank == "A")
                {
                    aces++;
                    value += 11;
                }
                else
                {
                    value += card.Value;
                }
            }

            // Adjust for aces
            while (value > 21 && aces > 0)
            {
                value -= 10;
                aces--;
            }

            return value;
        }

        private void DisplayGame(bool showDealerCard)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║              BLACKJACK               ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine($"║ Cards Left: {deck.CardsRemaining(),-23} ║");
            Console.WriteLine($"║ Count: {(runningCount >= 0 ? "+" : "")}{runningCount,-28} ║");
            Console.WriteLine("╠══════════════════════════════════════╣");

            // Dealer hand
            Console.Write("║ Dealer: ");
            if (showDealerCard)
            {
                foreach (Card card in dealerHand)
                {
                    Console.Write($"{card} ");
                }
                Console.WriteLine($"({GetHandValue(dealerHand)}){new string(' ', 20 - dealerHand.Count * 3 - GetHandValue(dealerHand).ToString().Length)}║");
            }
            else
            {
                Console.Write($"{dealerHand[0]} ??");
                Console.WriteLine($"{new string(' ', 25)}║");
            }

            Console.WriteLine("║                                      ║");

            // Player hand
            Console.Write("║ Player: ");
            foreach (Card card in playerHand)
            {
                Console.Write($"{card} ");
            }
            int pValue = GetHandValue(playerHand);
            string handStatus = hasSplit && !playingSplitHand ? " *" : "";
            Console.WriteLine($"({pValue}){handStatus}{new string(' ', 19 - playerHand.Count * 3 - pValue.ToString().Length - handStatus.Length)}║");

            // Split hand if exists
            if (hasSplit)
            {
                Console.Write("║ Split:  ");
                foreach (Card card in splitHand)
                {
                    Console.Write($"{card} ");
                }
                int sValue = GetHandValue(splitHand);
                string splitStatus = playingSplitHand ? " *" : "";
                Console.WriteLine($"({sValue}){splitStatus}{new string(' ', 19 - splitHand.Count * 3 - sValue.ToString().Length - splitStatus.Length)}║");
            }

            Console.WriteLine("║                                      ║");
            Console.WriteLine($"║ Bet: ${playerBet,-29} ║");
            if (hasSplit)
                Console.WriteLine($"║ Split Bet: ${splitBet,-24} ║");
            if (insuranceTaken)
                Console.WriteLine($"║ Insurance: ${insuranceBet,-24} ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
        }

        private bool PlayAgain()
        {
            Console.Clear();
            Console.WriteLine("Play again? (Y/N)");
            ConsoleKey key = Console.ReadKey(true).Key;
            return key == ConsoleKey.Y;
        }
    }

    class MinesGame
    {
        private const int GRID_SIZE = 5;
        private int mineCount;
        private int bet;
        private bool[,] mines;
        private bool[,] revealed;
        private bool gameOver;
        private bool hitMine;
        private int gemsRevealed;
        private double currentMultiplier;
        private Random random = new Random();
        private List<(int, int)> revealHistory = new List<(int, int)>();
        private bool undoUsed = false;
        private int longestStreak = 0;
        private int currentStreak = 0;

        // Simplified multiplier table (based on Stake.com)
        private static readonly Dictionary<int, Dictionary<int, double>> MultiplierTable = new Dictionary<int, Dictionary<int, double>>()
        {
            { 1, new Dictionary<int, double> { {1, 1.03}, {2, 1.08}, {3, 1.12}, {4, 1.18}, {5, 1.24}, {10, 1.65}, {15, 2.47}, {20, 4.95}, {24, 24.75} } },
            { 3, new Dictionary<int, double> { {1, 1.12}, {2, 1.29}, {3, 1.48}, {4, 1.71}, {5, 2.00}, {10, 5.00}, {15, 18.97}, {20, 227.70}, {22, 2277} } },
            { 5, new Dictionary<int, double> { {1, 1.24}, {2, 1.56}, {3, 2.00}, {4, 2.58}, {5, 3.39}, {10, 17.52}, {15, 208.72}, {20, 52598.70} } },
            { 8, new Dictionary<int, double> { {1, 1.46}, {2, 2.18}, {3, 3.35}, {4, 5.26}, {5, 8.50}, {10, 166.40}, {15, 23794.65}, {17, 1070759.25} } },
            { 10, new Dictionary<int, double> { {1, 1.65}, {2, 2.83}, {3, 5.00}, {4, 9.17}, {5, 17.52}, {10, 1077.61}, {15, 3236072.40} } },
            { 15, new Dictionary<int, double> { {1, 2.47}, {2, 6.60}, {3, 18.97}, {4, 59.64}, {5, 208.72}, {10, 3236072.40} } },
            { 20, new Dictionary<int, double> { {1, 4.95}, {2, 29.70}, {3, 227.70}, {4, 2504.70}, {5, 52598.70} } },
            { 24, new Dictionary<int, double> { {1, 24.75} } }
        };

        public void Play(CasinoGame casino)
        {
            while (true)
            {
                // Setup phase
                if (!SetupGame(casino)) return;

                // Game loop
                while (!gameOver)
                {
                    DisplayGame(casino);
                    HandleInput(casino);
                }

                // Game over
                DisplayGameOver(casino);
                if (!PlayAgain()) return;
            }
        }

        private bool SetupGame(CasinoGame casino)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║           MINES - SETUP              ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine($"║ Balance: ${casino.playerBalance,-26} ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            
            // Get bet
            Console.Write("Enter bet amount (or 0 to go back): $");
            string betInput = Console.ReadLine() ?? "";
            if (!int.TryParse(betInput, out bet) || bet <= 0) return false;
            if (bet > casino.playerBalance)
            {
                Console.WriteLine("Insufficient balance!");
                Thread.Sleep(1500);
                return false;
            }

            // Get mine count
            Console.Write("Enter number of mines (1-24): ");
            string mineInput = Console.ReadLine() ?? "";
            if (!int.TryParse(mineInput, out mineCount) || mineCount < 1 || mineCount > 24)
            {
                Console.WriteLine("Invalid mine count!");
                Thread.Sleep(1500);
                return false;
            }

            // Deduct bet
            casino.playerBalance -= bet;

            // Initialize game
            mines = new bool[GRID_SIZE, GRID_SIZE];
            revealed = new bool[GRID_SIZE, GRID_SIZE];
            gameOver = false;
            hitMine = false;
            gemsRevealed = 0;
            currentMultiplier = 1.0;
            revealHistory.Clear();
            undoUsed = false;

            // Place mines
            int minesPlaced = 0;
            while (minesPlaced < mineCount)
            {
                int row = random.Next(GRID_SIZE);
                int col = random.Next(GRID_SIZE);
                if (!mines[row, col])
                {
                    mines[row, col] = true;
                    minesPlaced++;
                }
            }

            return true;
        }

        private void HandleInput(CasinoGame casino)
        {
            Console.WriteLine("Commands: A1-E5 (pick), C (cash out), U (undo), P (pattern), A (auto-pick), Q (quit)");
            string input = Console.ReadLine()?.ToUpper() ?? "";

            if (input == "Q")
            {
                gameOver = true;
                hitMine = true; // Forfeit
                return;
            }

            if (input == "C")
            {
                // Cash out
                gameOver = true;
                currentStreak++;
                if (currentStreak > longestStreak) longestStreak = currentStreak;
                return;
            }

            if (input == "U")
            {
                UndoLastPick();
                return;
            }

            if (input == "P")
            {
                PatternReveal();
                return;
            }

            if (input == "A")
            {
                AutoPick();
                return;
            }

            if (TryParseCoordinates(input, out int row, out int col))
            {
                RevealCell(row, col);
            }
            else
            {
                Console.WriteLine("Invalid input! Press any key...");
                Console.ReadKey(true);
            }
        }

        private bool TryParseCoordinates(string input, out int row, out int col)
        {
            row = -1;
            col = -1;

            if (input.Length != 2) return false;

            char letter = input[0];
            char number = input[1];

            if (letter >= 'A' && letter <= 'E')
                row = letter - 'A';
            else return false;

            if (number >= '1' && number <= '5')
                col = number - '1';
            else return false;

            return true;
        }

        private void RevealCell(int row, int col)
        {
            if (revealed[row, col])
            {
                Console.WriteLine("Already revealed! Press any key...");
                Console.ReadKey(true);
                return;
            }

            revealed[row, col] = true;
            revealHistory.Add((row, col));

            if (mines[row, col])
            {
                // Hit a mine
                Console.Beep(200, 300);
                gameOver = true;
                hitMine = true;
                currentStreak = 0;
            }
            else
            {
                // Found a gem
                Console.Beep(800, 100);
                gemsRevealed++;
                currentMultiplier = GetMultiplier(mineCount, gemsRevealed);
            }
        }

        private void UndoLastPick()
        {
            if (undoUsed)
            {
                Console.WriteLine("Undo already used this game! Press any key...");
                Console.ReadKey(true);
                return;
            }

            if (revealHistory.Count == 0)
            {
                Console.WriteLine("Nothing to undo! Press any key...");
                Console.ReadKey(true);
                return;
            }

            var (row, col) = revealHistory[revealHistory.Count - 1];
            revealHistory.RemoveAt(revealHistory.Count - 1);
            revealed[row, col] = false;
            
            if (!mines[row, col])
            {
                gemsRevealed--;
                currentMultiplier = gemsRevealed > 0 ? GetMultiplier(mineCount, gemsRevealed) : 1.0;
            }

            undoUsed = true;
            Console.WriteLine("Last pick undone! (-10% multiplier penalty). Press any key...");
            currentMultiplier *= 0.9;
            Console.ReadKey(true);
        }

        private void PatternReveal()
        {
            Console.WriteLine("Pattern: 1. Cross  2. Square  3. Diagonal");
            Console.Write("Choose pattern: ");
            string choice = Console.ReadKey(true).KeyChar.ToString();

            Console.Write("\nCenter cell (e.g., C3): ");
            string input = Console.ReadLine()?.ToUpper() ?? "";
            
            if (!TryParseCoordinates(input, out int row, out int col))
            {
                Console.WriteLine("Invalid coordinates! Press any key...");
                Console.ReadKey(true);
                return;
            }

            List<(int, int)> pattern = new List<(int, int)>();
            
            switch (choice)
            {
                case "1": // Cross
                    pattern.Add((row, col));
                    if (row > 0) pattern.Add((row - 1, col));
                    if (row < GRID_SIZE - 1) pattern.Add((row + 1, col));
                    if (col > 0) pattern.Add((row, col - 1));
                    if (col < GRID_SIZE - 1) pattern.Add((row, col + 1));
                    break;
                case "2": // Square
                    for (int r = Math.Max(0, row - 1); r <= Math.Min(GRID_SIZE - 1, row + 1); r++)
                        for (int c = Math.Max(0, col - 1); c <= Math.Min(GRID_SIZE - 1, col + 1); c++)
                            pattern.Add((r, c));
                    break;
                case "3": // Diagonal
                    pattern.Add((row, col));
                    if (row > 0 && col > 0) pattern.Add((row - 1, col - 1));
                    if (row < GRID_SIZE - 1 && col < GRID_SIZE - 1) pattern.Add((row + 1, col + 1));
                    if (row > 0 && col < GRID_SIZE - 1) pattern.Add((row - 1, col + 1));
                    if (row < GRID_SIZE - 1 && col > 0) pattern.Add((row + 1, col - 1));
                    break;
                default:
                    Console.WriteLine("Invalid pattern! Press any key...");
                    Console.ReadKey(true);
                    return;
            }

            foreach (var (r, c) in pattern)
            {
                if (!revealed[r, c])
                {
                    RevealCell(r, c);
                    if (hitMine)
                    {
                        Console.WriteLine("Hit mine in pattern! Press any key...");
                        Console.ReadKey(true);
                        return;
                    }
                    Thread.Sleep(200);
                }
            }
        }

        private void AutoPick()
        {
            Console.Write("How many cells to auto-pick (1-5)? ");
            string input = Console.ReadLine() ?? "";
            if (!int.TryParse(input, out int count) || count < 1 || count > 5)
            {
                Console.WriteLine("Invalid number! Press any key...");
                Console.ReadKey(true);
                return;
            }

            int picked = 0;
            for (int attempt = 0; attempt < 100 && picked < count; attempt++)
            {
                int row = random.Next(GRID_SIZE);
                int col = random.Next(GRID_SIZE);
                
                if (!revealed[row, col])
                {
                    RevealCell(row, col);
                    picked++;
                    Thread.Sleep(300);
                    
                    if (hitMine)
                    {
                        Console.WriteLine("Auto-pick hit a mine! Press any key...");
                        Console.ReadKey(true);
                        return;
                    }
                }
            }
        }

        private double GetMultiplier(int mines, int gems)
        {
            // Find closest mine count in table
            int[] availableMines = { 1, 3, 5, 8, 10, 15, 20, 24 };
            int closestMines = availableMines.OrderBy(m => Math.Abs(m - mines)).First();

            if (MultiplierTable.ContainsKey(closestMines))
            {
                var table = MultiplierTable[closestMines];
                if (table.ContainsKey(gems))
                    return table[gems];
                
                // Find closest gem count
                int closestGems = table.Keys.OrderBy(g => Math.Abs(g - gems)).First();
                return table[closestGems];
            }

            // Fallback calculation
            double baseMultiplier = 1.0;
            int safeCells = 25 - mines;
            for (int i = 0; i < gems; i++)
            {
                baseMultiplier *= (double)(safeCells) / (25 - i);
                baseMultiplier *= 0.97; // House edge
            }
            return Math.Round(baseMultiplier, 2);
        }

        private void DisplayGame(CasinoGame casino)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║                MINES                 ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine($"║ Bet: ${bet,-30} ║");
            Console.WriteLine($"║ Mines: {mineCount,-28} ║");
            Console.WriteLine($"║ Gems Found: {gemsRevealed,-23} ║");
            Console.WriteLine($"║ Multiplier: {currentMultiplier}x{new string(' ', 30 - currentMultiplier.ToString().Length)}║");
            Console.WriteLine($"║ Potential Win: ${(int)(bet * currentMultiplier),-18} ║");
            
            // Risk indicator
            int unrevealed = 25 - gemsRevealed;
            double risk = unrevealed > 0 ? (double)mineCount / unrevealed * 100 : 0;
            string riskLevel = risk < 20 ? "🟢 LOW" : risk < 50 ? "🟡 MEDIUM" : "🔴 HIGH";
            Console.WriteLine($"║ Risk: {risk:F1}% {riskLevel,-22} ║");
            Console.WriteLine($"║ Streak: {currentStreak} (Best: {longestStreak}){new string(' ', 21 - currentStreak.ToString().Length - longestStreak.ToString().Length)}║");
            Console.WriteLine($"║ Undo: {(undoUsed ? "Used" : "Available"),-27} ║");
            
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║     1   2   3   4   5               ║");
            Console.WriteLine("║   ┌───┬───┬───┬───┬───┐             ║");

            for (int row = 0; row < GRID_SIZE; row++)
            {
                Console.Write($"║ {Convert.ToChar('A' + row)} │");
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    if (revealed[row, col])
                    {
                        if (mines[row, col])
                            Console.Write(" 💣");
                        else
                            Console.Write(" 💎");
                    }
                    else
                    {
                        Console.Write(" ■ ");
                    }
                    Console.Write("│");
                }
                Console.WriteLine("             ║");
                if (row < GRID_SIZE - 1)
                    Console.WriteLine("║   ├───┼───┼───┼───┼───┤             ║");
            }

            Console.WriteLine("║   └───┴───┴───┴───┴───┘             ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
        }

        private void DisplayGameOver(CasinoGame casino)
        {
            // Reveal all cells
            for (int row = 0; row < GRID_SIZE; row++)
                for (int col = 0; col < GRID_SIZE; col++)
                    revealed[row, col] = true;

            DisplayGame(casino);

            if (hitMine)
            {
                Console.WriteLine("\n💥 BOOM! You hit a mine! Lost $" + bet);
                Console.WriteLine($"Streak ended at {currentStreak}");
            }
            else
            {
                int winAmount = (int)(bet * currentMultiplier);
                casino.playerBalance += winAmount;
                Console.WriteLine($"\n💰 Cashed out! Won ${winAmount}!");
            }

            Console.WriteLine($"New balance: ${casino.playerBalance}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private bool PlayAgain()
        {
            Console.Clear();
            Console.WriteLine("Play again? (Y/N)");
            ConsoleKey key = Console.ReadKey(true).Key;
            return key == ConsoleKey.Y;
        }
    }

    class CrapsGame
    {
        private Random random = new Random();
        private int point = 0;
        private bool gameActive = false;
        private int currentBet = 0;
        private Dictionary<string, int> sideBets = new Dictionary<string, int>();

        // Dice face art
        private static readonly string[][] DiceFaces = new string[][]
        {
            // Die showing 1
            new string[] { "┌─────┐", "│     │", "│  ●  │", "│     │", "└─────┘" },
            // Die showing 2
            new string[] { "┌─────┐", "│ ●   │", "│     │", "│   ● │", "└─────┘" },
            // Die showing 3
            new string[] { "┌─────┐", "│ ●   │", "│  ●  │", "│   ● │", "└─────┘" },
            // Die showing 4
            new string[] { "┌─────┐", "│ ● ● │", "│     │", "│ ● ● │", "└─────┘" },
            // Die showing 5
            new string[] { "┌─────┐", "│ ● ● │", "│  ●  │", "│ ● ● │", "└─────┘" },
            // Die showing 6
            new string[] { "┌─────┐", "│ ● ● │", "│ ● ● │", "│ ● ● │", "└─────┘" }
        };

        public void Play(CasinoGame casino)
        {
            while (true)
            {
                if (!gameActive)
                {
                    // Come out roll
                    DisplayGameState(casino, "COME OUT ROLL");
                    currentBet = GetBet(casino);
                    if (currentBet == 0) return;

                    casino.playerBalance -= currentBet;

                    Console.WriteLine("\nPress any key to roll the dice...");
                    Console.ReadKey(true);

                    (int die1, int die2, int total) = RollDiceAnimated();

                    // Evaluate come out roll
                    if (total == 7 || total == 11)
                    {
                        Console.WriteLine("\n🎉 NATURAL! Pass line wins!");
                        int winnings = currentBet * 2;
                        casino.playerBalance += winnings;
                        Console.WriteLine($"You win ${winnings}!");
                        DisplayResult(casino);
                    }
                    else if (total == 2 || total == 3 || total == 12)
                    {
                        Console.WriteLine("\n💥 CRAPS! Pass line loses!");
                        Console.WriteLine($"You lose ${currentBet}");
                        DisplayResult(casino);
                    }
                    else
                    {
                        point = total;
                        gameActive = true;
                        Console.WriteLine($"\n📍 POINT ESTABLISHED: {point}");
                        Console.WriteLine("Now trying to roll the point before a 7!");
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey(true);
                    }
                }
                else
                {
                    // Point established - rolling for the point
                    DisplayGameState(casino, $"POINT: {point}");
                    Console.WriteLine($"\nCurrent bet: ${currentBet}");
                    Console.WriteLine($"Trying to roll: {point}");
                    Console.WriteLine("Avoid rolling: 7");
                    
                    // Offer side bets
                    OfferSideBets(casino);
                    
                    Console.WriteLine("\nPress any key to roll...");
                    Console.ReadKey(true);

                    (int die1, int die2, int total) = RollDiceAnimated();

                    // Check side bets
                    EvaluateSideBets(total, casino);

                    if (total == point)
                    {
                        Console.WriteLine($"\n🎉 MADE THE POINT! You rolled {point}!");
                        int winnings = currentBet * 2;
                        casino.playerBalance += winnings;
                        Console.WriteLine($"You win ${winnings}!");
                        gameActive = false;
                        point = 0;
                        DisplayResult(casino);
                    }
                    else if (total == 7)
                    {
                        Console.WriteLine("\n💥 SEVEN OUT! You lose!");
                        Console.WriteLine($"Lost ${currentBet}");
                        gameActive = false;
                        point = 0;
                        DisplayResult(casino);
                    }
                    else
                    {
                        Console.WriteLine($"\nRolled {total}. Keep rolling for the point!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    }
                }

                if (!gameActive)
                {
                    sideBets.Clear();
                    if (!PlayAgain()) return;
                }
            }
        }

        private void OfferSideBets(CasinoGame casino)
        {
            sideBets.Clear();
            Console.WriteLine("\n--- PLACE SIDE BETS (or press Enter to skip) ---");
            Console.WriteLine("1. Field Bet (2,3,4,9,10,11,12) - Pays 1:1 (2x on 2/12)");
            Console.WriteLine($"2. Place Bet on {point} - Pays based on number");
            Console.WriteLine("3. Any 7 - Pays 4:1");
            Console.WriteLine("4. Any Craps (2,3,12) - Pays 7:1");
            Console.Write("Select bet (1-4) or Enter to skip: ");
            
            string choice = Console.ReadLine() ?? "";
            if (string.IsNullOrEmpty(choice)) return;
            
            Console.Write($"Enter bet amount (balance: ${casino.playerBalance}): $");
            string betInput = Console.ReadLine() ?? "";
            if (!int.TryParse(betInput, out int bet) || bet <= 0 || bet > casino.playerBalance) return;
            
            casino.playerBalance -= bet;
            
            switch (choice)
            {
                case "1":
                    sideBets["field"] = bet;
                    Console.WriteLine($"Field bet ${bet} placed!");
                    break;
                case "2":
                    sideBets[$"place{point}"] = bet;
                    Console.WriteLine($"Place bet on {point} for ${bet} placed!");
                    break;
                case "3":
                    sideBets["any7"] = bet;
                    Console.WriteLine($"Any 7 bet ${bet} placed!");
                    break;
                case "4":
                    sideBets["anycraps"] = bet;
                    Console.WriteLine($"Any Craps bet ${bet} placed!");
                    break;
            }
            Thread.Sleep(1000);
        }

        private void EvaluateSideBets(int total, CasinoGame casino)
        {
            if (sideBets.Count == 0) return;
            
            Console.WriteLine("\n--- Side Bet Results ---");
            
            // Field bet
            if (sideBets.ContainsKey("field"))
            {
                int bet = sideBets["field"];
                if (total == 2 || total == 12)
                {
                    int win = bet * 3; // 2:1 pays triple
                    casino.playerBalance += win;
                    Console.WriteLine($"✅ Field wins ${win} (2x payout)!");
                    Console.Beep(1000, 200);
                }
                else if (new[] { 3, 4, 9, 10, 11 }.Contains(total))
                {
                    int win = bet * 2;
                    casino.playerBalance += win;
                    Console.WriteLine($"✅ Field wins ${win}!");
                    Console.Beep(800, 150);
                }
                else
                {
                    Console.WriteLine($"❌ Field loses ${bet}");
                }
            }
            
            // Place bet
            if (sideBets.ContainsKey($"place{point}") && total == point)
            {
                int bet = sideBets[$"place{point}"];
                int multiplier = point == 4 || point == 10 ? 2 : point == 5 || point == 9 ? 3 : 2;
                int win = bet * multiplier;
                casino.playerBalance += win;
                Console.WriteLine($"✅ Place bet wins ${win}!");
                Console.Beep(1200, 200);
            }
            
            // Any 7
            if (sideBets.ContainsKey("any7"))
            {
                int bet = sideBets["any7"];
                if (total == 7)
                {
                    int win = bet * 5; // 4:1 pays 5x
                    casino.playerBalance += win;
                    Console.WriteLine($"✅ Any 7 wins ${win}!");
                    Console.Beep(1500, 300);
                }
                else
                {
                    Console.WriteLine($"❌ Any 7 loses ${bet}");
                }
            }
            
            // Any Craps
            if (sideBets.ContainsKey("anycraps"))
            {
                int bet = sideBets["anycraps"];
                if (total == 2 || total == 3 || total == 12)
                {
                    int win = bet * 8; // 7:1 pays 8x
                    casino.playerBalance += win;
                    Console.WriteLine($"✅ Any Craps wins ${win}!");
                    Console.Beep(1300, 250);
                }
                else
                {
                    Console.WriteLine($"❌ Any Craps loses ${bet}");
                }
            }
            
            Thread.Sleep(2000);
        }

        private int GetBet(CasinoGame casino)
        {
            Console.Write($"\nEnter bet on PASS LINE (balance: ${casino.playerBalance}) or 0 to quit: $");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int bet))
                {
                    if (bet == 0) return 0;
                    if (bet > 0 && bet <= casino.playerBalance) return bet;
                }
                Console.Write("Invalid bet. Enter bet amount: $");
            }
        }

        private (int, int, int) RollDiceAnimated()
        {
            int die1 = 0, die2 = 0;
            
            // Animate dice rolling (10 frames)
            for (int i = 0; i < 10; i++)
            {
                die1 = random.Next(1, 7);
                die2 = random.Next(1, 7);
                DisplayDice(die1, die2, die1 + die2, true);
                Thread.Sleep(100);
            }

            // Final roll
            die1 = random.Next(1, 7);
            die2 = random.Next(1, 7);
            int total = die1 + die2;
            
            DisplayDice(die1, die2, total, false);
            Thread.Sleep(500);

            return (die1, die2, total);
        }

        private void DisplayDice(int die1, int die2, int total, bool rolling)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║                CRAPS                 ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            
            if (gameActive)
                Console.WriteLine($"║ POINT: {point,-30} ║");
            else
                Console.WriteLine("║ COME OUT ROLL                        ║");
            
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║                                      ║");

            // Display dice side by side
            string[] dice1 = DiceFaces[die1 - 1];
            string[] dice2 = DiceFaces[die2 - 1];

            for (int line = 0; line < 5; line++)
            {
                Console.WriteLine($"║     {dice1[line]}  {dice2[line]}      ║");
            }

            Console.WriteLine("║                                      ║");
            
            if (rolling)
            {
                Console.WriteLine("║          🎲 ROLLING... 🎲            ║");
            }
            else
            {
                Console.WriteLine($"║         TOTAL: {total,-2}                   ║");
            }
            
            Console.WriteLine("╚══════════════════════════════════════╝");
        }

        private void DisplayGameState(CasinoGame casino, string status)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║                CRAPS                 ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine($"║ Balance: ${casino.playerBalance,-26} ║");
            Console.WriteLine($"║ Status: {status,-28} ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║        🎲  PASS LINE  🎲             ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║  Come Out Roll:                      ║");
            Console.WriteLine("║    • 7 or 11 = WIN                   ║");
            Console.WriteLine("║    • 2, 3, or 12 = LOSE              ║");
            Console.WriteLine("║    • Other = Establish Point         ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║  Point Roll:                         ║");
            Console.WriteLine("║    • Roll Point = WIN                ║");
            Console.WriteLine("║    • Roll 7 = LOSE                   ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
        }

        private void DisplayResult(CasinoGame casino)
        {
            Console.WriteLine($"\nNew balance: ${casino.playerBalance}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private bool PlayAgain()
        {
            Console.Clear();
            Console.WriteLine("Play again? (Y/N)");
            ConsoleKey key = Console.ReadKey(true).Key;
            return key == ConsoleKey.Y;
        }
    }

    class RouletteGame
    {
        private Random random = new Random();
        
        // Roulette wheel layout (American roulette with 0 and 00)
        private static readonly int[] RedNumbers = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };
        private static readonly int[] BlackNumbers = { 2, 4, 6, 8, 10, 11, 13, 15, 17, 20, 22, 24, 26, 28, 29, 31, 33, 35 };
        
        // Game modes
        private bool isEuropean = true; // true = European (single 0), false = American (0 and 00)
        
        // Statistics and history
        private List<int> spinHistory = new List<int>();
        private Dictionary<string, int> betTypeStats = new Dictionary<string, int>
        {
            {"Number", 0}, {"Red", 0}, {"Black", 0}, {"Odd", 0}, {"Even", 0}, 
            {"Low", 0}, {"High", 0}, {"Split", 0}, {"Street", 0}, {"Corner", 0}, 
            {"Line", 0}, {"Neighbor", 0}
        };
        private int totalSpins = 0;
        private int totalWins = 0;
        private int biggestWin = 0;
        private int totalWagered = 0;
        private int totalPayout = 0;
        
        public void Play(CasinoGame casino)
        {
            // Select game mode first time
            if (totalSpins == 0)
            {
                SelectGameMode();
            }
            
            while (true)
            {
                DisplayTable(casino);
                
                // Get bet type
                Console.WriteLine("\n🎲 SELECT BET TYPE:");
                Console.WriteLine("╔═══════════════════════════════════════════════════════╗");
                Console.WriteLine("║ OUTSIDE BETS (Even Money)                             ║");
                Console.WriteLine("║  1. 🔴 Red (1:1)         2. ⚫ Black (1:1)            ║");
                Console.WriteLine("║  3. 🔢 Odd (1:1)         4. 🔢 Even (1:1)            ║");
                Console.WriteLine("║  5. ⬇️  Low 1-18 (1:1)   6. ⬆️  High 19-36 (1:1)      ║");
                Console.WriteLine("╠═══════════════════════════════════════════════════════╣");
                Console.WriteLine("║ INSIDE BETS (High Payouts)                            ║");
                Console.WriteLine("║  7. 🎯 Single Number (35:1)                           ║");
                Console.WriteLine("║  8. ➗ Split - 2 numbers (17:1)                        ║");
                Console.WriteLine("║  9. 🔢 Street - 3 numbers (11:1)                      ║");
                Console.WriteLine("║ 10. ▪️  Corner - 4 numbers (8:1)                      ║");
                Console.WriteLine("║ 11. ➖ Line - 6 numbers (5:1)                         ║");
                Console.WriteLine("╠═══════════════════════════════════════════════════════╣");
                Console.WriteLine("║ SPECIAL BETS                                          ║");
                Console.WriteLine("║ 12. 🎯 Neighbor Bet - Number + 2 neighbors (varies)   ║");
                Console.WriteLine("╠═══════════════════════════════════════════════════════╣");
                Console.WriteLine("║  H. 📊 View History & Stats    M. 🎰 Change Mode      ║");
                Console.WriteLine("║  Q. ⬅️  Quit to Main Menu                             ║");
                Console.WriteLine("╚═══════════════════════════════════════════════════════╝");
                Console.Write("\nChoice: ");
                
                string choice = Console.ReadKey(true).KeyChar.ToString().ToUpper();
                
                if (choice == "Q") return;
                if (choice == "H") { ShowStatistics(); continue; }
                if (choice == "M") { SelectGameMode(); continue; }
                
                if (!int.TryParse(choice, out int betType) || betType < 1 || betType > 12)
                {
                    Console.WriteLine("\n❌ Invalid choice!");
                    Thread.Sleep(1000);
                    continue;
                }
                
                // Process bet based on type
                ProcessBet(casino, betType);
                
                if (!PlayAgain()) return;
            }
        }
        
        private void SelectGameMode()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║         SELECT GAME MODE             ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║ 1. 🇪🇺 European Roulette (0 only)    ║");
            Console.WriteLine("║    House Edge: 2.70%                 ║");
            Console.WriteLine("║    Numbers: 0-36 (37 total)          ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║ 2. 🇺🇸 American Roulette (0 & 00)    ║");
            Console.WriteLine("║    House Edge: 5.26%                 ║");
            Console.WriteLine("║    Numbers: 0,00,1-36 (38 total)     ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("\nChoice (1 or 2): ");
            
            ConsoleKey key = Console.ReadKey(true).Key;
            isEuropean = (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1);
            
            Console.WriteLine($"\n\n✅ {(isEuropean ? "European" : "American")} Roulette selected!");
            Thread.Sleep(1500);
        }
        
        private void ProcessBet(CasinoGame casino, int betType)
        {
            List<int> numbers = new List<int>();
            string betName = "";
            int payout = 0;
            
            // Get numbers based on bet type
            switch (betType)
            {
                case 1: // Red
                    numbers.AddRange(RedNumbers);
                    betName = "Red";
                    payout = 2;
                    break;
                case 2: // Black
                    numbers.AddRange(BlackNumbers);
                    betName = "Black";
                    payout = 2;
                    break;
                case 3: // Odd
                    numbers.AddRange(Enumerable.Range(1, 36).Where(n => n % 2 == 1));
                    betName = "Odd";
                    payout = 2;
                    break;
                case 4: // Even
                    numbers.AddRange(Enumerable.Range(1, 36).Where(n => n % 2 == 0));
                    betName = "Even";
                    payout = 2;
                    break;
                case 5: // Low
                    numbers.AddRange(Enumerable.Range(1, 18));
                    betName = "Low";
                    payout = 2;
                    break;
                case 6: // High
                    numbers.AddRange(Enumerable.Range(19, 18));
                    betName = "High";
                    payout = 2;
                    break;
                case 7: // Single Number
                    numbers = GetSingleNumber();
                    if (numbers.Count == 0) return;
                    betName = "Number";
                    payout = 36;
                    break;
                case 8: // Split
                    numbers = GetSplitBet();
                    if (numbers.Count == 0) return;
                    betName = "Split";
                    payout = 18;
                    break;
                case 9: // Street
                    numbers = GetStreetBet();
                    if (numbers.Count == 0) return;
                    betName = "Street";
                    payout = 12;
                    break;
                case 10: // Corner
                    numbers = GetCornerBet();
                    if (numbers.Count == 0) return;
                    betName = "Corner";
                    payout = 9;
                    break;
                case 11: // Line
                    numbers = GetLineBet();
                    if (numbers.Count == 0) return;
                    betName = "Line";
                    payout = 6;
                    break;
                case 12: // Neighbor
                    numbers = GetNeighborBet();
                    if (numbers.Count == 0) return;
                    betName = "Neighbor";
                    payout = 36; // Individual number payout, split across neighbors
                    break;
            }
            
            // Get bet amount
            Console.Write($"\n\n💵 Enter bet amount (balance: ${casino.playerBalance}): $");
            string betInput = Console.ReadLine() ?? "";
            if (!int.TryParse(betInput, out int bet) || bet <= 0) return;
            if (bet > casino.playerBalance)
            {
                Console.WriteLine("❌ Insufficient balance!");
                Thread.Sleep(1500);
                return;
            }
            
            // Record bet
            totalWagered += bet;
            betTypeStats[betName]++;
            casino.playerBalance -= bet;
            
            // Spin the wheel
            Console.WriteLine("\n🎰 Press any key to spin the wheel...");
            Console.ReadKey(true);
            
            int result = SpinWheel();
            
            // Check if won
            bool won = numbers.Contains(result);
            
            if (won)
            {
                int winAmount = (bet * payout) / (betType == 12 ? 5 : 1); // Neighbor bet splits among 5 numbers
                casino.playerBalance += winAmount;
                totalPayout += winAmount;
                totalWins++;
                
                if (winAmount > biggestWin) biggestWin = winAmount;
                
                Console.Beep(1500, 300);
                Console.WriteLine($"\n🎉 YOU WIN! Bet: {betName} | Payout: ${winAmount}");
                Console.WriteLine($"Numbers: {string.Join(", ", numbers)} | Result: {result}");
            }
            else
            {
                Console.Beep(200, 200);
                Console.WriteLine($"\n💔 You lose ${bet}");
                Console.WriteLine($"Numbers: {string.Join(", ", numbers)} | Result: {result}");
            }
            
            Console.WriteLine($"\n💰 New balance: ${casino.playerBalance}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
        
        private List<int> GetSingleNumber()
        {
            int maxNum = isEuropean ? 36 : 37;
            Console.Write($"\n\n🎯 Enter number (0-{maxNum}): ");
            string input = Console.ReadLine() ?? "";
            if (int.TryParse(input, out int num) && num >= 0 && num <= maxNum)
                return new List<int> { num };
            
            Console.WriteLine("❌ Invalid number!");
            Thread.Sleep(1000);
            return new List<int>();
        }
        
        private List<int> GetSplitBet()
        {
            Console.Write("\n\n➗ Enter two adjacent numbers (e.g., '5 8'): ");
            string input = Console.ReadLine() ?? "";
            var parts = input.Split(' ');
            
            if (parts.Length == 2 && 
                int.TryParse(parts[0], out int num1) && 
                int.TryParse(parts[1], out int num2))
            {
                // Validate adjacency (horizontal or vertical on table)
                if (Math.Abs(num1 - num2) == 1 || Math.Abs(num1 - num2) == 3)
                    return new List<int> { num1, num2 };
            }
            
            Console.WriteLine("❌ Invalid split bet!");
            Thread.Sleep(1000);
            return new List<int>();
        }
        
        private List<int> GetStreetBet()
        {
            Console.Write("\n\n🔢 Enter first number of street (row of 3): ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int num) && num >= 1 && num <= 34 && (num - 1) % 3 == 0)
            {
                return new List<int> { num, num + 1, num + 2 };
            }
            
            Console.WriteLine("❌ Invalid street bet! Must be first number of a row (1, 4, 7, ...)");
            Thread.Sleep(1000);
            return new List<int>();
        }
        
        private List<int> GetCornerBet()
        {
            Console.Write("\n\n▪️  Enter top-left number of 2x2 square: ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int num) && num >= 1 && num <= 32)
            {
                // Validate it forms a valid corner
                if ((num - 1) % 3 != 2 && num + 3 <= 36)
                    return new List<int> { num, num + 1, num + 3, num + 4 };
            }
            
            Console.WriteLine("❌ Invalid corner bet!");
            Thread.Sleep(1000);
            return new List<int>();
        }
        
        private List<int> GetLineBet()
        {
            Console.Write("\n\n➖ Enter first number of line (two rows, 6 numbers): ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int num) && num >= 1 && num <= 31 && (num - 1) % 3 == 0)
            {
                return new List<int> { num, num + 1, num + 2, num + 3, num + 4, num + 5 };
            }
            
            Console.WriteLine("❌ Invalid line bet! Must be first number of a row (1, 4, 7, ...)");
            Thread.Sleep(1000);
            return new List<int>();
        }
        
        private List<int> GetNeighborBet()
        {
            // European wheel order
            int[] wheelOrder = { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };
            
            Console.Write("\n\n🎯 Enter center number (bets on it + 2 neighbors each side): ");
            string input = Console.ReadLine() ?? "";
            
            if (int.TryParse(input, out int centerNum) && centerNum >= 0 && centerNum <= 36)
            {
                int index = Array.IndexOf(wheelOrder, centerNum);
                List<int> neighbors = new List<int>();
                
                for (int i = -2; i <= 2; i++)
                {
                    int neighborIndex = (index + i + wheelOrder.Length) % wheelOrder.Length;
                    neighbors.Add(wheelOrder[neighborIndex]);
                }
                
                Console.WriteLine($"📍 Betting on: {string.Join(", ", neighbors)}");
                return neighbors;
            }
            
            Console.WriteLine("❌ Invalid number!");
            Thread.Sleep(1000);
            return new List<int>();
        }
        
        private void ShowStatistics()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║              📊 ROULETTE STATISTICS 📊               ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine($"║ Game Mode: {(isEuropean ? "🇪🇺 European" : "🇺🇸 American"),-37} ║");
            Console.WriteLine($"║ Total Spins: {totalSpins,-38} ║");
            Console.WriteLine($"║ Total Wins: {totalWins,-39} ║");
            Console.WriteLine($"║ Win Rate: {(totalSpins > 0 ? (totalWins * 100.0 / totalSpins).ToString("F1") : "0.0")}%{"",-38} ║");
            Console.WriteLine($"║ Biggest Win: ${biggestWin,-37} ║");
            Console.WriteLine($"║ Total Wagered: ${totalWagered,-34} ║");
            Console.WriteLine($"║ Total Payout: ${totalPayout,-35} ║");
            Console.WriteLine($"║ Net: ${(totalPayout - totalWagered),-42} ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 🎯 BET TYPE USAGE:                                   ║");
            
            foreach (var kvp in betTypeStats.OrderByDescending(x => x.Value).Take(8))
            {
                if (kvp.Value > 0)
                    Console.WriteLine($"║   {kvp.Key,-15} {kvp.Value,-30} bets ║");
            }
            
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 📜 RECENT SPINS (Last 10):                           ║");
            
            if (spinHistory.Count > 0)
            {
                var recent = spinHistory.TakeLast(10).Reverse();
                Console.Write("║   ");
                foreach (var spin in recent)
                {
                    string color = GetColor(spin);
                    string symbol = color == "RED" ? "🔴" : (color == "BLACK" ? "⚫" : "🟢");
                    Console.Write($"{symbol}{spin} ");
                }
                Console.WriteLine($"{"",-30}║");
            }
            else
            {
                Console.WriteLine("║   No spins yet!                                      ║");
            }
            
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }
        
        private int SpinWheel()
        {
            totalSpins++;
            
            // Animate wheel spinning
            for (int i = 0; i < 20; i++)
            {
                int tempNum = random.Next(0, isEuropean ? 37 : 38);
                DisplayWheel(tempNum, true);
                Thread.Sleep(100);
            }
            
            // Final result
            int result = random.Next(0, isEuropean ? 37 : 38);
            spinHistory.Add(result);
            DisplayWheel(result, false);
            Thread.Sleep(1500);
            
            return result;
        }
        
        private void DisplayWheel(int number, bool spinning)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║              ROULETTE                ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║            🎡  WHEEL  🎡             ║");
            Console.WriteLine("║                                      ║");
            
            if (spinning)
            {
                Console.WriteLine("║           ⚪ SPINNING... ⚪           ║");
            }
            else
            {
                string color = GetColor(number);
                string colorSymbol = color == "RED" ? "🔴" : (color == "BLACK" ? "⚫" : "🟢");
                Console.WriteLine($"║                                      ║");
                Console.WriteLine($"║             {colorSymbol}  {number,-2}  {colorSymbol}              ║");
                Console.WriteLine($"║            {color,-6}                   ║");
            }
            
            Console.WriteLine("║                                      ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
        }
        
        private void DisplayTable(CasinoGame casino)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  🎰 ROULETTE 🎰                      ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine($"║ 💰 Balance: ${casino.playerBalance,-38} ║");
            Console.WriteLine($"║ 🎮 Mode: {(isEuropean ? "European (0)" : "American (0, 00)"),-39} ║");
            Console.WriteLine($"║ 📊 Spins: {totalSpins,-40} Wins: {totalWins,-4} ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine("║                    ROULETTE TABLE                    ║");
            Console.WriteLine("║                                                      ║");
            
            if (isEuropean)
                Console.WriteLine("║  🟢 0 (Green)                                        ║");
            else
                Console.WriteLine("║  🟢 0 (Green)        🟢 00 (Green)                   ║");
            
            Console.WriteLine("║                                                      ║");
            Console.WriteLine("║  🔴 RED NUMBERS:                                     ║");
            Console.WriteLine("║    1  3  5  7  9  12  14  16  18  19  21  23  25     ║");
            Console.WriteLine("║    27  30  32  34  36                                ║");
            Console.WriteLine("║                                                      ║");
            Console.WriteLine("║  ⚫ BLACK NUMBERS:                                   ║");
            Console.WriteLine("║    2  4  6  8  10  11  13  15  17  20  22  24  26    ║");
            Console.WriteLine("║    28  29  31  33  35                                ║");
            Console.WriteLine("║                                                      ║");
            if (spinHistory.Count > 0)
            {
                var lastFive = spinHistory.TakeLast(5).Reverse();
                Console.Write("║  📜 Last 5: ");
                foreach (var spin in lastFive)
                {
                    string color = GetColor(spin);
                    string symbol = color == "RED" ? "🔴" : (color == "BLACK" ? "⚫" : "🟢");
                    Console.Write($"{symbol}{spin} ");
                }
                Console.WriteLine($"{"",-28}║");
            }
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        }
        
        private string GetColor(int number)
        {
            if (number == 0 || number == 37) return "GREEN"; // 37 represents 00 in American
            if (RedNumbers.Contains(number)) return "RED";
            if (BlackNumbers.Contains(number)) return "BLACK";
            return "GREEN";
        }
        
        private bool PlayAgain()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║          Continue Playing?           ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║  Y - Yes, continue                   ║");
            Console.WriteLine("║  N - Return to main menu             ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("\nChoice: ");
            ConsoleKey key = Console.ReadKey(true).Key;
            return key == ConsoleKey.Y;
        }
    }

    class CasinoGame
    {
        public int playerBalance = 1000;
        private bool running = true;
        private BlackjackGame blackjack = new BlackjackGame();
        private MinesGame mines = new MinesGame();
        private CrapsGame craps = new CrapsGame();
        private RouletteGame roulette = new RouletteGame();

        public void Run()
        {
            while (running)
            {
                ShowMainMenu();
                HandleMainMenuInput();
            }
        }

        private void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║              CASINO GAME             ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine($"║ Balance: ${playerBalance,-26} ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║ 1. Blackjack                        ║");
            Console.WriteLine("║ 2. Craps                            ║");
            Console.WriteLine("║ 3. Mines                            ║");
            Console.WriteLine("║ 4. Roulette                         ║");
            Console.WriteLine("║ 5. Slots                            ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║ Q. Quit                             ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("Choose a game (1-5) or Q to quit: ");
        }

        private void HandleMainMenuInput()
        {
            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    blackjack.Play(this);
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    craps.Play(this);
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    mines.Play(this);
                    break;
                case ConsoleKey.D4:
                case ConsoleKey.NumPad4:
                    roulette.Play(this);
                    break;
                case ConsoleKey.D5:
                case ConsoleKey.NumPad5:
                    PlaySlots();
                    break;
                case ConsoleKey.Q:
                    running = false;
                    break;
            }
        }



        private SlotsGame slots = new SlotsGame();

        private void PlaySlots()
        {
            slots.Play(this);
        }
    class SlotsGame
    {
        private static readonly string[][] SymbolThemes = new string[][] {
            new string[] { "🍒", "🔔", "🍋", "⭐", "💎", "7️⃣", "🃏", "🎰" }, // classic
            new string[] { "🐶", "🐱", "🐭", "🐹", "🐰", "🦊", "🃏", "🎰" }, // animals
            new string[] { "🍏", "🍊", "🍋", "🍉", "🍇", "🍓", "🃏", "🎰" }  // fruits
        };
        private static readonly int[] Payouts = { 2, 5, 3, 10, 20, 100, 0, 0 }; // payouts (wild/scatter no direct payout)
        private const int WildIdx = 6; // 🃏
        private const int ScatterIdx = 7; // 🎰
        private Random random = new Random();

        // Statistics
        private int totalSpins = 0, totalWins = 0, totalLosses = 0, biggestWin = 0, freeSpinsAwarded = 0;

        public void Play(CasinoGame casino)
        {
            int theme = SelectTheme();
            string[] Symbols = SymbolThemes[theme];
            while (true)
            {
                int paylines = GetPaylines();
                int betPerLine = GetBetPerLine(casino.playerBalance, paylines);
                if (betPerLine == 0) return;
                int totalBet = betPerLine * paylines;
                casino.playerBalance -= totalBet;

                int spins = 1;
                bool freeSpinActive = false;
                do
                {
                    string[][] reels = SpinReelsAnimated(Symbols);
                    var (payout, winLines, jackpot, freeSpins) = CalculatePayout(reels, betPerLine, paylines, Symbols);
                    DisplayResult(reels, payout, totalBet, casino, paylines, betPerLine, winLines, jackpot, freeSpinActive, freeSpins);
                    totalSpins++;
                    if (payout > 0) { totalWins++; if (payout > biggestWin) biggestWin = payout; } else totalLosses++;
                    if (jackpot) Console.Beep(1000, 500);
                    if (freeSpins > 0) { freeSpinsAwarded += freeSpins; spins += freeSpins; freeSpinActive = true; Console.Beep(800, 200); }
                    else freeSpinActive = false;
                } while (--spins > 0);

                ShowStats();
                if (!PlayAgain()) return;
            }
        }

        private int SelectTheme()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║        SLOTS - THEME SELECT          ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║ 1. Classic (🍒🔔🍋⭐💎7️⃣🃏🎰)           ║");
            Console.WriteLine("║ 2. Animals (🐶🐱🐭🐹🐰🦊🃏🎰)            ║");
            Console.WriteLine("║ 3. Fruits (🍏🍊🍋🍉🍇🍓🃏🎰)             ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("Choose theme (1-3): ");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int t) && t >= 1 && t <= 3) return t - 1;
                Console.Write("Invalid. Choose theme (1-3): ");
            }
        }

        private int GetPaylines()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║               SLOTS                  ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║ Select number of paylines (1-5):     ║");
            Console.WriteLine("║ 1. Center row                        ║");
            Console.WriteLine("║ 2. All rows (top, center, bottom)    ║");
            Console.WriteLine("║ 3. Rows + diagonals                  ║");
            Console.WriteLine("║ 4. Rows + diagonals + verticals      ║");
            Console.WriteLine("║ 5. All lines (rows, cols, diagonals) ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("Enter choice (1-5): ");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int lines) && lines >= 1 && lines <= 5)
                    return lines;
                Console.Write("Invalid. Enter choice (1-5): ");
            }
        }

        private int GetBetPerLine(int balance, int paylines)
        {
            Console.Write($"Enter bet per line (balance: ${balance}, total bet: ${paylines}x): $");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int bet))
                {
                    int total = bet * paylines;
                    if (bet == 0) return 0;
                    if (bet > 0 && total <= balance) return bet;
                }
                Console.Write("Invalid bet. Enter bet per line: $");
            }
        }

        private string[][] SpinReelsAnimated(string[] Symbols)
        {
            int rows = 3, cols = 3;
            string[][] reels = new string[rows][];
            for (int r = 0; r < rows; r++) reels[r] = new string[cols];

            // Animate spinning
            for (int frame = 0; frame < 15; frame++)
            {
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        reels[r][c] = Symbols[random.Next(Symbols.Length)];
                DisplayReels(reels, spinning: true, winLines: null);
                Thread.Sleep(80);
            }

            // Final result
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    reels[r][c] = Symbols[random.Next(Symbols.Length)];
            return reels;
        }

        private void DisplayReels(string[][] reels, bool spinning, List<(int[], int[])> winLines)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║               SLOTS                  ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║      ┌─────┬─────┬─────┐             ║");
            for (int r = 0; r < 3; r++)
            {
                Console.Write("║      │");
                for (int c = 0; c < 3; c++)
                {
                    bool highlight = false;
                    if (winLines != null)
                        foreach (var (rows, cols) in winLines)
                            for (int i = 0; i < 3; i++)
                                if (rows[i] == r && cols[i] == c) highlight = true;
                    if (highlight) Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($" {reels[r][c]} │");
                    if (highlight) Console.ResetColor();
                }
                Console.WriteLine("             ║");
                if (r < 2) Console.WriteLine("║      ├─────┼─────┼─────┤             ║");
            }
            Console.WriteLine("║      └─────┴─────┴─────┘             ║");
            Console.WriteLine("║                                      ║");
            if (spinning)
                Console.WriteLine("║         🎰 SPINNING... 🎰            ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
        }

        private (int payout, List<(int[], int[])> winLines, bool jackpot, int freeSpins) CalculatePayout(string[][] reels, int betPerLine, int paylines, string[] Symbols)
        {
            int payout = 0;
            bool jackpot = false;
            int freeSpins = 0;
            List<(int[], int[])> winLines = new List<(int[], int[])>();
            var lines = new List<(int[], int[])>();
            // Rows
            if (paylines >= 1) lines.Add((new int[]{1,1,1}, new int[]{0,1,2}));
            if (paylines >= 2) { lines.Add((new int[]{0,0,0}, new int[]{0,1,2})); lines.Add((new int[]{2,2,2}, new int[]{0,1,2})); }
            if (paylines >= 3) { lines.Add((new int[]{0,1,2}, new int[]{0,1,2})); lines.Add((new int[]{2,1,0}, new int[]{0,1,2})); }
            if (paylines >= 4) { lines.Add((new int[]{0,1,2}, new int[]{0,0,0})); lines.Add((new int[]{0,1,2}, new int[]{1,1,1})); lines.Add((new int[]{0,1,2}, new int[]{2,2,2})); }
            if (paylines == 5) { lines.Add((new int[]{0,0,0}, new int[]{0,1,2})); lines.Add((new int[]{1,1,1}, new int[]{0,1,2})); lines.Add((new int[]{2,2,2}, new int[]{0,1,2})); lines.Add((new int[]{0,1,2}, new int[]{0,0,0})); lines.Add((new int[]{0,1,2}, new int[]{1,1,1})); lines.Add((new int[]{0,1,2}, new int[]{2,2,2})); lines.Add((new int[]{0,1,2}, new int[]{0,1,2})); lines.Add((new int[]{2,1,0}, new int[]{0,1,2})); }
            
            foreach (var (rows, cols) in lines)
            {
                string s0 = reels[rows[0]][cols[0]];
                string s1 = reels[rows[1]][cols[1]];
                string s2 = reels[rows[2]][cols[2]];
                
                // Check for scatter (🎰)
                if (s0 == Symbols[ScatterIdx] && s1 == Symbols[ScatterIdx] && s2 == Symbols[ScatterIdx])
                {
                    freeSpins += 3;
                    winLines.Add((rows, cols));
                    continue;
                }
                
                // Check for wilds (🃏) - substitute for any symbol
                bool hasWild = (s0 == Symbols[WildIdx] || s1 == Symbols[WildIdx] || s2 == Symbols[WildIdx]);
                
                if (hasWild)
                {
                    // All wilds
                    if (s0 == Symbols[WildIdx] && s1 == Symbols[WildIdx] && s2 == Symbols[WildIdx])
                    {
                        payout += betPerLine * 100;
                        winLines.Add((rows, cols));
                        continue;
                    }
                    
                    // Try to match with wilds
                    for (int i = 0; i < Symbols.Length - 2; i++)
                    {
                        int matchCount = 0;
                        if (s0 == Symbols[i] || s0 == Symbols[WildIdx]) matchCount++;
                        if (s1 == Symbols[i] || s1 == Symbols[WildIdx]) matchCount++;
                        if (s2 == Symbols[i] || s2 == Symbols[WildIdx]) matchCount++;
                        
                        if (matchCount == 3 && (s0 == Symbols[i] || s1 == Symbols[i] || s2 == Symbols[i]))
                        {
                            payout += betPerLine * Payouts[i];
                            winLines.Add((rows, cols));
                            if (i == 5) jackpot = true; // 7️⃣
                            break;
                        }
                    }
                }
                else
                {
                    // Normal match (no wilds)
                    if (s0 == s1 && s1 == s2)
                    {
                        int idx = Array.IndexOf(Symbols, s0);
                        if (idx >= 0 && idx < Payouts.Length)
                        {
                            payout += betPerLine * Payouts[idx];
                            winLines.Add((rows, cols));
                            if (idx == 5) jackpot = true; // 7️⃣
                        }
                    }
                }
            }
            return (payout, winLines, jackpot, freeSpins);
        }

        private void DisplayResult(string[][] reels, int payout, int totalBet, CasinoGame casino, int paylines, int betPerLine, List<(int[], int[])> winLines, bool jackpot, bool freeSpinActive, int freeSpins)
        {
            DisplayReels(reels, spinning: false, winLines);
            if (payout > 0)
            {
                casino.playerBalance += payout;
                Console.Beep();
                Console.WriteLine($"\n🎉 WIN! You win ${payout}!");
                if (jackpot) Console.WriteLine("💰💰💰 JACKPOT! Three 7️⃣ on a payline! 💰💰💰");
            }
            else
            {
                Console.WriteLine($"\n💔 No win. Lost ${totalBet}.");
            }
            if (freeSpins > 0) Console.WriteLine($"🎰 SCATTER! {freeSpins} free spins awarded!");
            if (freeSpinActive) Console.WriteLine("🔄 Free spin round!");
            Console.WriteLine($"New balance: ${casino.playerBalance}");
            Console.WriteLine($"Paylines played: {paylines}, Bet per line: ${betPerLine}");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey(true);
        }

        private void ShowStats()
        {
            Console.WriteLine($"\n--- SLOTS STATS ---");
            Console.WriteLine($"Total spins: {totalSpins}");
            Console.WriteLine($"Wins: {totalWins}, Losses: {totalLosses}");
            Console.WriteLine($"Biggest win: ${biggestWin}");
            Console.WriteLine($"Free spins awarded: {freeSpinsAwarded}");
        }

        private bool PlayAgain()
        {
            Console.Clear();
            Console.WriteLine("Play again? (Y/N)");
            ConsoleKey key = Console.ReadKey(true).Key;
            return key == ConsoleKey.Y;
        }
    }
}
}