using System;
using System.Collections.Generic;

class Player
{
    public string UserName { get; private set; }
    public int CurrentRating { get; private set; }
    public int GamesCount { get; private set; }
    private List<(string opponentName, string result, int ratingChange, int gameIndex)> gameHistory;

    public Player(string username, int currentRating = 1000)
    {
        UserName = username;
        CurrentRating = currentRating;
        GamesCount = 0;
        gameHistory = new List<(string opponentName, string result, int ratingChange, int gameIndex)>();
    }

    public void WinGame(Game game)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Game cannot be null.");
        }

        int ratingChange = game.CalculateRatingChange(true);
        CurrentRating += ratingChange;
        GamesCount++;
        gameHistory.Add((game.GetOpponent(UserName), "Win", ratingChange, GamesCount));
    }

    public void LoseGame(Game game)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Game cannot be null.");
        }

        int ratingChange = game.CalculateRatingChange(false);
        CurrentRating -= ratingChange;
        if (CurrentRating < 1)
        {
            CurrentRating = 1;
        }
        GamesCount++;
        gameHistory.Add((game.GetOpponent(UserName), "Lose", ratingChange, GamesCount));
    }

    public void GetStats()
    {
        Console.WriteLine($"Game history for {UserName}:");
        Console.WriteLine("Opponent  | Result | Rating Change | Game Index");
        foreach (var game in gameHistory)
        {
            Console.WriteLine($"{game.opponentName,-8} | {game.result,-6} | {game.ratingChange,13} | {game.gameIndex,10}");
        }
    }
}

interface IGame
{
    int CalculateRatingChange(bool isWin);
    string GetOpponent(string playerName);
}

class StandardGame : IGame
{
    public int CalculateRatingChange(bool isWin)
    {
        return isWin ? 50 : 20;
    }

    public string GetOpponent(string playerName)
    {
        // Логіка визначення опонента
        return "Opponent";
    }
}

class TrainingGame : IGame
{
    public int CalculateRatingChange(bool isWin)
    {
        return 0;
    }

    public string GetOpponent(string playerName)
    {
        return "";
    }
}

class ReducedLossGame : IGame
{
    public int CalculateRatingChange(bool isWin)
    {
        return isWin ? 50 : 10;
    }

    public string GetOpponent(string playerName)
    {
        // Логіка визначення опонента
        return "Opponent";
    }
}

class BonusWinSeriesGame : IGame
{
    private int winSeriesCount;

    public BonusWinSeriesGame(int winSeriesCount)
    {
        this.winSeriesCount = winSeriesCount;
    }

    public int CalculateRatingChange(bool isWin)
    {
        if (isWin)
        {
            winSeriesCount++;
            return winSeriesCount <= 3 ? 30 : 50;
        }
        else
        {
            winSeriesCount = 0;
            return 20;
        }
    }

    public string GetOpponent(string playerName)
    {
        // Логіка визначення опонента
        return "Opponent";
    }
}

class GameFactory
{
    public static IGame CreateStandardGame()
    {
        return new StandardGame();
    }

    public static IGame CreateTrainingGame()
    {
        return new TrainingGame();
    }

    public static IGame CreateReducedLossGame()
    {
        return new ReducedLossGame();
    }

    public static IGame CreateBonusWinSeriesGame(int winSeriesCount)
    {
        if (winSeriesCount < 0)
        {
            throw new ArgumentException("Win series count cannot be negative.", nameof(winSeriesCount));
        }

        return new BonusWinSeriesGame(winSeriesCount);
    }
}

class Program
{
    static void Main()
    {
        Player player1 = new Player("Alice");
        Player player2 = new Player("Bob");

        IGame standardGame = GameFactory.CreateStandardGame();
        IGame trainingGame = GameFactory.CreateTrainingGame();
        IGame reducedLossGame = GameFactory.CreateReducedLossGame();
        IGame bonusWinSeriesGame = GameFactory.CreateBonusWinSeriesGame(0);

        player1.WinGame(standardGame);
        player2.LoseGame(standardGame);

        player1.WinGame(trainingGame);
        player2.LoseGame(trainingGame);

        player1.WinGame(reducedLossGame);
        player2.LoseGame(reducedLossGame);

        player1.WinGame(bonusWinSeriesGame);
        player1.WinGame(bonusWinSeriesGame);
        player1.WinGame(bonusWinSeriesGame);
        player1.WinGame(bonusWinSeriesGame);
        player2.LoseGame(bonusWinSeriesGame);

        player1.GetStats();
        player2.GetStats();
    }
}
