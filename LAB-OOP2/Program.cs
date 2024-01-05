using System;
using System.Collections.Generic;

class Player
{
    public string UserName { get; private set; }
    public int CurrentRating { get; private set; }
    public int GamesCount { get; private set; }
    private List<(string opponentName, string result, int ratingChange, int gameIndex)> gameHistory;
    private GameAccount gameAccount;

    public Player(string username, GameAccount account, int currentRating = 1000)
    {
        UserName = username;
        CurrentRating = currentRating;
        GamesCount = 0;
        gameHistory = new List<(string opponentName, string result, int ratingChange, int gameIndex)>();
        gameAccount = account;
    }

    public void WinGame(Game game)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Game cannot be null.");
        }

        int ratingChange = game.CalculateRating(this, gameAccount);
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

        int ratingChange = game.CalculateRating(this, gameAccount);
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
    int CalculateRating(Player player, GameAccount account);
    string GetOpponent(string playerName);
}

class GameAccount
{
    public virtual int CalculatePoints(bool isWin)
    {
        return isWin ? 50 : 20;
    }
}

class StandardAccount : GameAccount
{
    // Standard implementation
}

class ReducedLossAccount : GameAccount
{
    public override int CalculatePoints(bool isWin)
    {
        return isWin ? 50 : 10;
    }
}

class BonusWinSeriesAccount : GameAccount
{
    private int winSeriesCount;

    public override int CalculatePoints(bool isWin)
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
}

abstract class Game
{
    public abstract int CalculateRating(Player player, GameAccount account);
    public abstract string GetOpponent(string playerName);
}

class StandardGame : Game
{
    public override int CalculateRating(Player player, GameAccount account)
    {
        return player.CurrentRating + account.CalculatePoints(true);
    }

    public override string GetOpponent(string playerName)
    {
        // Enhance this logic to get a dynamic opponent based on actual game conditions
        return playerName == "Alice" ? "Bob" : "Alice";
    }
}

class TrainingGame : Game
{
    public override int CalculateRating(Player player, GameAccount account)
    {
        // Training game does not affect the rating
        return player.CurrentRating;
    }

    public override string GetOpponent(string playerName)
    {
        return "";
    }
}

class OnePlayerRatingGame : Game
{
    public override int CalculateRating(Player player, GameAccount account)
    {
        // Rating is calculated based on a single player's performance
        return account.CalculatePoints(true);
    }

    public override string GetOpponent(string playerName)
    {
        return "";
    }
}

class Program
{
    static void Main()
    {
        Player player1 = new Player("Alice", new StandardAccount());
        Player player2 = new Player("Bob", new ReducedLossAccount());

        Game standardGame = new StandardGame();
        Game trainingGame = new TrainingGame();
        Game onePlayerRatingGame = new OnePlayerRatingGame();

        player1.WinGame(standardGame);
        player2.LoseGame(standardGame);

        player1.WinGame(trainingGame);
        player2.LoseGame(trainingGame);

        player1.WinGame(onePlayerRatingGame);
        player1.WinGame(onePlayerRatingGame);

        player1.GetStats();
        player2.GetStats();
    }
}
