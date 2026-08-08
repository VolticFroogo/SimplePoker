namespace SimplePoker.Models;

public class Player
{
    public int Id { get; set; }

    public bool IsHost { get; set; }

    public bool IsOnline { get; set; }

    public Guid LocalPlayerId { get; set; }
}
