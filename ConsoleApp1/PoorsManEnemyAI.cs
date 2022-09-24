namespace ConsoleApp1;

public class PoorsManEnemyAI
{
    private Random _random = new Random();

    public string EnemyHit(Board board)
    {
        var availableCells = board.CellList.Where(c => c.WasShot is false).ToArray();
        var stillFlowingShip = board.CellList.Where(c => c.HasShip && c.WasShot && c.Ship.IsSunk is false).ToArray();

        foreach (var ship in stillFlowingShip)
        {
            availableCells = ship.Neighbours.Where(c => c.WasShot is false).ToArray();
            if (availableCells.Any())
            {
                break;
            }
        }

        return ShootRandomly(board, availableCells);
    }

    private string ShootRandomly(Board board, Cell[] availableCells)
    {
        var shotAtCell = availableCells[_random.Next(0, availableCells.Count() - 1)];
        var ship = board.Hit(shotAtCell);
        return ship is null
            ? $"Enemy shot at {shotAtCell} and missed."
            : $"Enemy shot at {shotAtCell} and scored a hit!";
    }
}