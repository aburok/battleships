namespace ConsoleApp1;

public record Cell(int X, int Y)
{
    public Ship? Ship { get; set; }

    public bool WasShot { get; set; }

    public bool HasShip => Ship is not null;

    public bool InSameLine(Cell other)
    {
        return this.X == other.X || this.Y == other.Y;
    }

    public IList<Cell> Neighbours { get; } = new List<Cell>();
    public Cell? Above { get; set; }
    public Cell? Below { get; set; }
    public Cell? Left { get; set; }
    public Cell? Right { get; set; }

    public string Letter => $"{(char)(Y + 65)}";
    public string Number => $"{X + 1}";
    public string Position => Letter + Number;

    public override string ToString()
    {
        return Position;
    }
}

public enum Direction
{
    Above,
    Right,
    Below,
    Left,
    
}