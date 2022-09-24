namespace ConsoleApp1;

public class ShipRandomInitializer
{
    readonly Random _random = new Random();
    
    public void InitializeRandomly(Board board)
    {
        foreach (var ship in board.Ships)
        {
            var availableCells = board.CellList.Where(c => c.Ship is null)
                .ToArray();

            RandomPlacement result = null;
            while(true)
            {
                result = TryGetRandomCells(board, availableCells, ship.Size);
                if(result.Success)
                    break;
            }

            ship.SetPosition(result.Start, result.End, board);
        }
    }
    
    private record RandomPlacement(bool Success, Cell Start, Cell End);

    private static RandomPlacement FailAttempt = new(false, null, null);

    private RandomPlacement TryGetRandomCells(Board board, Cell[] availableCells, int size)
    {
        var randomCell = availableCells[_random.Next(availableCells.Count())];

        // 0 - North, 1 - East, 2 - South, 3 - West
        var shipOrientation = _random.Next(3);

        var startPositionX = randomCell.X;
        var startPositionY = randomCell.Y;
        var endPositionX = startPositionX;
        var endPositionY = startPositionY;

        switch (shipOrientation)
        {
            case 0:
                endPositionY = endPositionY - size + 1;
                if (endPositionY < 0) return FailAttempt;
                break;
            case 1:
                endPositionX = endPositionX + size - 1;
                if (endPositionX > 9) return FailAttempt;
                break;
            case 2:
                endPositionY = endPositionY + size - 1;
                if (endPositionY > 9) return FailAttempt;
                break;
            case 3:
                endPositionX = endPositionX - size + 1;
                if (endPositionX < 0) return FailAttempt;
                break;
        }

        var endCell = board.Cells[endPositionX, endPositionY];
        var cells = board.GetCells(randomCell, endCell);

        if (cells.All(c => c.Ship is null))
        {
            return new RandomPlacement(true, randomCell, endCell);
        }

        return FailAttempt;
    }
}