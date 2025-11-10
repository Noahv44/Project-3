CSCI-310 Development Notebook
Name:
Noah Vachon And Daris Kadric

Project/Assignment:
CSCI 310 Project 3: C# Console Game - Casino

Problem/Task:
Create a console-based casino game with multiple mini-games (blackjack, craps, mines, and others) using C#. The game must include keyboard input, real-time updates, scoring, game states, and smooth gameplay without graphical libraries.

---

Development Log

Iteration 1: Initial Setup & Project Structure

What do you do?
Set up the C# console project structure with .csproj file and Program.cs. Update the developer notebook with project details. Create basic project folders and initial code structure.

Response/Result:
- Created Casino.csproj file for the C# console application
- Set up Program.cs with basic console setup and main menu structure
- Updated DevNotebook.md with Project 3 information
- Established project directory structure with game selection framework

Your Evaluation:
Good start. Project structure is in place with a functional main menu. The console setup is correct with hidden cursor and proper title. Next need to implement the individual games starting with blackjack.

---

Iteration 2: Main Menu & Game Selection

What do you do?
Implemented the main menu system with game selection, player balance display, and navigation. Created placeholder methods for each game. Set up the basic game loop and input handling.

Response/Result:
- Created attractive ASCII art main menu with game options
- Implemented keyboard input handling for menu navigation
- Added player balance tracking and display
- Set up game loop with proper state management
- Created placeholder methods for blackjack, craps, mines, roulette, and slots

Your Evaluation:
Excellent! The main menu looks professional and provides clear navigation. Input handling is responsive and the balance system is in place. Now ready to implement the first game - blackjack.

---

Iteration 3: Blackjack Game Implementation

What do you do?
Implemented a complete blackjack game with card deck, dealing, player actions (hit, stand, double down), dealer logic, and scoring. Added betting system integrated with casino balance. Created visual game display with ASCII art.

Response/Result:
- Created Card and Deck classes with proper shuffling and drawing
- Implemented BlackjackGame class with full game logic
- Added betting system with balance integration
- Player can hit, stand, or double down
- Dealer follows standard rules (hit on 16, stand on 17)
- Proper ace handling (soft 17)
- Visual display shows hands and values
- Win/lose/push detection with balance updates
- Play again functionality

Your Evaluation:
Fantastic! Blackjack is fully functional with all standard casino rules. The game is engaging and the UI is clear. Balance tracking works correctly. Next should implement another game like mines for variety.

---

Iteration 4: Mines Game Implementation

What do you do?
Implemented a minesweeper-style game with a 5x5 grid containing 5 hidden mines. Players input coordinates (A1-E5) to reveal cells. Game ends when a mine is hit or all safe cells are revealed. Added win rewards and proper game state management.

Response/Result:
- Created 5x5 grid with 5 randomly placed mines
- Implemented coordinate input system (A1-E5 format)
- Added cell revealing with neighbor mine counts
- Visual grid display with ASCII art borders
- Win condition: reveal all non-mine cells
- Lose condition: hit a mine
- Win reward: +$100 to balance
- Game over display shows all mines
- Play again functionality

Your Evaluation:
Great addition! Mines provides a different type of gameplay - puzzle-based rather than luck-based like blackjack. The coordinate system works well for console input. Visual feedback is clear with numbers showing adjacent mines. Ready to implement one more game to meet the requirements.

---

Iteration 5: Craps Game Implementation

What do you do?
Implemented a simplified craps game with come-out roll and point phases. Players roll two dice with standard craps rules: 7 or 11 wins on come-out, 2/3/12 loses, other numbers establish a point. When point is set, player rolls until hitting the point (win) or 7 (lose).

Response/Result:
- Created CrapsGame class with game state tracking
- Implemented two-phase gameplay: come-out roll and point phase
- Dice rolling with random number generation
- Standard craps rules implemented
- Win rewards: +$100 for making the point
- Visual display with dice emojis and game status
- Betting system integrated with casino balance
- Play again functionality

Your Evaluation:
Excellent! Craps adds another classic casino game. The two-phase system works well and follows traditional rules. Now have three different game types: card game (blackjack), puzzle (mines), and dice (craps). This provides good variety and meets the project requirements for multiple games.

---

Iteration 6: Mines Game Reimplementation (Stake.com Style)

What do you do?
Completely reimplemented Mines game to match Stake.com mechanics. Players now select mine count (1-24), place bets, reveal cells for gems, and can cash out anytime. Each gem revealed increases the multiplier based on a payout table. Progressive risk/reward system where players balance greed vs safety.

Response/Result:
- Player selects number of mines (1-24) before starting
- Betting system with initial bet placed
- 5x5 grid with gems (💎) and mines (💣)
- Progressive multiplier system based on mines/gems revealed
- Multiplier table implementation (1 mine/1 gem = 1.03x, 24 mines/1 gem = 24.75x, etc.)
- Cash out functionality - player can collect winnings anytime
- Real-time display of current multiplier and potential winnings
- Hit mine = lose everything, cash out = win bet × multiplier
- Visual feedback shows gems found and potential payout

Your Evaluation:
Perfect! This is now the correct Mines game matching Stake.com. The progressive multiplier system creates exciting tension - players must decide when to cash out vs continue for bigger multipliers. The risk/reward balance is engaging. Much better than the original minesweeper version.

---

Iteration 7: Craps Game Enhancement with Animated Dice

What do you do?
Completely enhanced the craps game with animated dice rolling, visual ASCII dice faces, and pass line betting. Implemented proper two 6-sided dice that animate by flipping through numbers before landing on the final result. Added detailed game rules display and improved visual feedback.

Response/Result:
- Created ASCII art dice faces for all 6 sides (1-6)
- Implemented animated dice rolling (10 animation frames)
- Dice flip through random numbers at 100ms intervals before final roll
- Visual display shows both dice side-by-side
- Pass line betting system clearly labeled
- Rules displayed on setup screen (7/11 win, 2/3/12 lose, etc.)
- Point phase shows current point and objective
- Better win/loss messaging with emojis (🎉, 💥, 📍)
- Proper bet tracking and payout calculation (2x on win)
- Current bet displayed during point rolls

Your Evaluation:
Excellent enhancement! The animated dice rolling adds great visual excitement - watching the numbers flip creates anticipation. The ASCII dice faces look professional and are easy to read. Pass line betting is now clearly explained with rules. The game feels much more authentic to real craps. Visual feedback is significantly improved with clear status messages and the point tracking.

---

Iteration 8: Roulette Game Implementation

What do you do?
Implemented a full roulette game with multiple betting options (single numbers, red/black, odd/even, low/high), animated wheel spinning, and proper payout calculations. Created visual wheel display with color-coded results and spinning animation.

Response/Result:
- Multiple betting options available:
  - Single number (0-36) pays 35:1
  - Red/Black pays 1:1
  - Odd/Even pays 1:1
  - Low (1-18) / High (19-36) pays 1:1
- American roulette wheel (0-36, with 0 as green)
- Red numbers: 1,3,5,7,9,12,14,16,18,19,21,23,25,27,30,32,34,36
- Black numbers: 2,4,6,8,10,11,13,15,17,20,22,24,26,28,29,31,33,35
- Animated wheel spinning (20 frames at 100ms)
- Visual display with colored emojis (🔴 red, ⚫ black, 🟢 green)
- Clear betting interface with payout odds displayed
- Win/loss detection with proper payout calculations
- Balance tracking and betting limits

Your Evaluation:
Excellent implementation! Roulette adds another classic casino game with different mechanics. The spinning wheel animation creates excitement and suspense. Multiple betting options give players strategy choices. Color-coded visual feedback makes results clear. Payout calculations are correct (35:1 for numbers, 1:1 for even money bets). The game now has 4 fully functional games with good variety.

---

Final Summary

The development process successfully created a multi-game casino console application with five distinct games:

1. **Blackjack** - Card game with betting, hit/stand/double down actions, dealer AI, and proper ace handling
2. **Mines** - Puzzle game with Stake.com-style progressive multipliers, cash out, and strategic risk/reward gameplay
3. **Craps** - Dice game with traditional come-out and point phases, animated dice, and pass line betting
4. **Roulette** - Classic wheel game with multiple bet types (number, red/black, odd/even, low/high), animated wheel, and proper payouts
5. **Slots** - Enhanced slot machine with multiple paylines, wild/scatter symbols, free spins, progressive jackpot, sound effects, win highlighting, statistics, and theme customization

Key Features Implemented:
- Main menu with game selection (5 fully functional games)
- Shared player balance system across all games
- Keyboard input handling for all interactions
- Visual ASCII art interfaces for each game
- Betting systems with balance tracking
- Win/lose detection and rewards
- Play again functionality for each game
- Game state management
- Clear visual feedback and displays

Technologies Used:
- C# (.NET 7.0)
- Console Application (no graphics libraries)
- Object-oriented design with separate game classes
- Random number generation for cards, dice, mine placement, slot reels

---

Iteration 9: Slots Game Enhancement

What do you do?
Implemented a comprehensive enhancement to the Slots game, adding multiple advanced features:
- Multiple paylines (1-5 options: center row, all rows, rows+diagonals, rows+diagonals+verticals, all lines)
- Variable bet sizes (bet per line selection)
- Wild symbol (🃏) that substitutes for any symbol
- Scatter symbol (🎰) that triggers free spins
- Free spin round (3 free spins for three scatters)
- Progressive jackpot for three 7️⃣ on any payline
- Sound effects using Console.Beep() for wins, jackpots, and free spins
- Win highlighting (winning symbols displayed in yellow)
- Statistics tracking (total spins, wins, losses, biggest win, free spins awarded)
- Theme customization (Classic, Animals, Fruits themes with different symbol sets)

Response/Result:
- Created theme selection system with 3 symbol sets:
  - Classic: 🍒🔔🍋⭐💎7️⃣🃏🎰
  - Animals: 🐶🐱🐭🐹🐰🦊🃏🎰
  - Fruits: 🍏🍊🍋🍉🍇🍓🃏🎰
- Implemented wild symbol logic that substitutes for any regular symbol
- Added scatter detection that awards 3 free spins for three 🎰 symbols
- Free spin round automatically continues spinning with the same bet
- Jackpot detection for three 7️⃣ with special celebration message and sound
- Win highlighting using ConsoleColor.Yellow for winning symbols
- Comprehensive statistics displayed after each session
- Sound effects: Console.Beep() for wins, Console.Beep(1000, 500) for jackpots, Console.Beep(800, 200) for free spins
- Multiple payline configurations from simple center row to complex 8-line combinations
- Variable betting allows players to control risk per line

Your Evaluation:
Excellent comprehensive enhancement! The slots game now rivals professional slot machines with:
- Strategic depth from payline and bet selection
- Excitement from wild symbols increasing win chances
- Anticipation from scatter symbols triggering free spins
- Thrilling progressive jackpot for three 7️⃣
- Visual feedback with color-coded winning symbols
- Audio feedback with console beeps for different events
- Personalization through theme selection
- Engagement tracking via statistics display

The slots game is now feature-complete with professional-grade mechanics. All five casino games are fully functional and polished, providing a comprehensive casino experience.

---

Final Summary

The development process successfully created a multi-game casino console application with five distinct games:

1. **Blackjack** - Card game with betting, hit/stand/double down actions, dealer AI, and proper ace handling
2. **Mines** - Puzzle game with Stake.com-style progressive multipliers, cash out, and strategic risk/reward gameplay
3. **Craps** - Dice game with traditional come-out and point phases, animated dice, and pass line betting
4. **Roulette** - Classic wheel game with multiple bet types (number, red/black, odd/even, low/high), animated wheel, and proper payouts
5. **Slots** - Enhanced slot machine with multiple paylines, wild/scatter symbols, free spins, progressive jackpot, sound effects, win highlighting, statistics, and theme customization

Key Features Implemented:
- Main menu with game selection (5 fully functional games)
- Shared player balance system across all games
- Keyboard input handling for all interactions
- Visual ASCII art interfaces for each game
- Betting systems with balance tracking
- Win/lose detection and rewards
- Play again functionality for each game
- Game state management
- Clear visual feedback and displays

Technologies Used:
- C# (.NET 7.0)
- Console Application (no graphics libraries)
- Object-oriented design with separate game classes

Project Requirements Met:
✅ Keyboard input for player control
✅ Start menu/game selection
✅ Game-over messages and scoring
✅ Dynamic real-time updates (card draws, dice rolls, cell reveals)
✅ Playable boundaries (grids, betting limits)
✅ Score/balance tracking
✅ Restart functionality
✅ Smooth gameplay with responsive input
✅ Progress tracking (balance, wins/losses)
✅ C# console application without graphical libraries

The casino game successfully provides an engaging multi-game experience with variety in gameplay mechanics (cards, puzzles, dice) while maintaining a cohesive theme and shared balance system.