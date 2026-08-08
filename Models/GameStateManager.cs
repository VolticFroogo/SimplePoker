using System.Collections.Concurrent;

namespace SimplePoker.Models;

public class GameStateManager
{
    private readonly ConcurrentDictionary<Guid, GameState> _gameStates = new();

    public GameState GetOrCreateGameState(Guid gameId)
    {
        return _gameStates.GetOrAdd(gameId, id => new GameState());
    }

    public void RemoveGameState(Guid gameId)
    {
        _gameStates.TryRemove(gameId, out _);
    }
}
