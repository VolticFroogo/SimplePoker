using SimplePoker.Enums;

namespace SimplePoker.Models;

public class GameState
{
    public event Action? OnGameStateChanged;

    public int PlayerCount => _players.Count - 1;

    public HandState HandState => _handState;

    private readonly List<Player> _players = [];

    private int[] _deck;

    private HandState _handState = HandState.Preflop;

    public GameState()
    {
        _deck = GenerateShuffledDeck();
    }

    public Player AddOrFindPlayer(Guid localPlayerId)
    {
        var player = _players.FirstOrDefault(p => p.LocalPlayerId == localPlayerId);
        if (player is not null)
        {
            return player;
        }

        var isFirstPlayer = !_players.Any();

        var lowestFoundId = 0;
        for (var i = 0; i < _players.Count; i++)
        {
            if (_players[i].Id != lowestFoundId)
            {
                break;
            }

            lowestFoundId++;
        }

        player = new Player
        {
            Id = lowestFoundId,
            IsHost = isFirstPlayer,
            LocalPlayerId = localPlayerId,
        };

        _players.Add(player);
        OnGameStateChanged?.Invoke();

        return player;
    }

    public int[] GetCardsForPlayer(Player player)
    {
        if (player.IsHost)
        {
            throw new InvalidOperationException("Host player does not have cards");
        }

        var startIndex = 5 + (player.Id - 1) * 2;
        return _deck.Skip(startIndex).Take(2).ToArray();
    }

    public int[] GetCommunityCards()
    {
        return _handState switch
        {
            HandState.Preflop => [],
            HandState.Flop => [.. _deck.Take(3)],
            HandState.Turn => [.. _deck.Take(4)],
            HandState.River => [.. _deck.Take(5)],
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    public void AdvanceHandState()
    {
        if (_handState == HandState.River)
        {
            throw new InvalidOperationException("Cannot advance hand state beyond River");
        }

        _handState++;
        OnGameStateChanged?.Invoke();
    }

    public void RedealGame()
    {
        _deck = GenerateShuffledDeck();
        _handState = HandState.Preflop;
        _players.RemoveAll(player => !player.IsOnline);
        OnGameStateChanged?.Invoke();
    }

    private int[] GenerateShuffledDeck()
    {
        var deck = Enumerable.Range(0, 52).ToArray();
        Random.Shared.Shuffle(deck);

        return deck;
    }
}
