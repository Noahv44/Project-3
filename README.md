# Casino Console Game - Project Overview

## Project Information
- **Course**: CSCI 310 GUI & Game
- **Project**: Project 3 - C# Console Game
- **Team**: Noah Vachon and Daris Kadric
- **Date**: November 3, 2025

## Project Description
A multi-game casino console application featuring three distinct casino games with shared balance tracking and a professional menu system. The project demonstrates object-oriented design, game state management, and console input/output handling in C#.

## Games Implemented

### 1. Blackjack
- Full blackjack implementation with standard casino rules
- Player actions: Hit, Stand, Double Down
- Dealer AI that hits on 16 and stands on 17
- Proper ace handling (soft 17)
- Betting system with balance integration
- Visual card display with suits and values

### 2. Mines (Minesweeper-style)
- 5x5 grid with 5 randomly placed mines
- Coordinate input system (A1-E5)
- Number indicators showing adjacent mine counts
- Win condition: Reveal all non-mine cells
- Lose condition: Hit a mine
- Win reward: +$100 bonus
- Visual grid with ASCII art borders

### 3. Craps
- Traditional craps rules with two game phases
- Come-out roll: 7/11 wins, 2/3/12 loses
- Point phase: Make the point to win, 7 loses
- Dice rolling with visual feedback
- Betting system integrated with casino balance
- Win rewards for successful point rolls

## Technical Features

### Core Systems
- **Main Menu**: Professional ASCII art menu with game selection
- **Balance Tracking**: Shared player balance across all games ($1000 starting balance)
- **Input Handling**: Responsive keyboard input for all game interactions
- **Game State Management**: Proper state tracking for each game
- **Visual Design**: ASCII art interfaces with borders and clear layouts

### Code Structure
```
Casino.csproj          - Project configuration file
Program.cs             - Main game code
  ├─ Program           - Entry point
  ├─ Card              - Card representation
  ├─ Deck              - Card deck with shuffling
  ├─ BlackjackGame     - Blackjack implementation
  ├─ MinesGame         - Mines implementation
  ├─ CrapsGame         - Craps implementation
  └─ CasinoGame        - Main menu and game coordination
```

## Project Requirements Compliance

✅ **Game Interaction**
- Keyboard input for all player control
- Professional start menu with game selection
- Game-over messages with final scores/balance
- Return to menu functionality

✅ **Gameplay Mechanics**
- Dynamic real-time updates (card draws, dice rolls, mine reveals)
- Clear playable boundaries (grids, betting limits, game rules)
- Various actions (hit/stand, reveal cells, roll dice)

✅ **Game State and Score**
- Balance tracking across all games
- Game state management (in-progress, game-over)
- Restart functionality (play again after each game)
- Win/lose detection with appropriate rewards

✅ **Technical Requirements**
- Written in C# for console application
- No graphical libraries used
- Smooth gameplay with responsive input
- Non-blocking input handling

✅ **Scoring & Feedback**
- Balance serves as progress indicator
- Clear visual feedback for all actions
- Win/lose messages with balance updates

## How to Build and Run

### Build the Project
```powershell
cd "c:\Users\noahv\OneDrive\Desktop\GUI & Games\Project 3"
dotnet build
```

### Run the Game
```powershell
dotnet run
```

### Game Controls
- **Main Menu**: Number keys (1-5) to select game, Q to quit
- **Blackjack**: 1=Hit, 2=Stand, 3=Double Down
- **Mines**: Type coordinates (e.g., A1, B3), Q to quit
- **Craps**: Press any key to roll dice
- **All Games**: Y/N for play again prompts

## Game Design Highlights

### Variety
Three different game types provide varied gameplay:
- **Skill-based**: Blackjack (card counting, strategy)
- **Puzzle-based**: Mines (deduction, logic)
- **Chance-based**: Craps (dice probability)

### Visual Polish
- ASCII art borders and layouts
- Clear information display (balance, cards, grid)
- Dice and mine emojis for visual interest
- Consistent styling across all games

### User Experience
- Easy-to-understand controls
- Clear instructions and feedback
- Smooth transitions between games
- Balance persistence across games
- Play again options for continuous gameplay

## Future Enhancements (Placeholder Games)
The main menu includes slots for 5 games, with 2 still marked as "Coming Soon":
- **Roulette**: Number/color betting with spinning wheel
- **Slots**: Multi-reel slot machine with paylines

## Development Notes
- Built with .NET 7.0 SDK
- Clean object-oriented design with separate game classes
- Shared CasinoGame class coordinates all games
- Random number generation for unpredictability
- Nullable reference handling for type safety

## Testing
All three games have been tested for:
- Correct rule implementation
- Balance updates
- Win/lose conditions
- Edge cases (bust in blackjack, all cells revealed in mines, etc.)
- Smooth gameplay flow
- Return to menu functionality

## Conclusion
This casino console game successfully demonstrates C# programming skills, game design principles, and console application development. The project meets all specified requirements while providing an engaging multi-game experience with professional presentation and smooth gameplay.
