using System.Text;

namespace ConsoleApp1;

public class BoardDrawer
{
    public const string Water = "-";
    public const string Miss = "*";
    public const string MyShip = "O";
    public const string HitShip = "X";

    public string Draw(Board board, Board enemyBoard)
    {
        var playersBoardPrint = DrawBoard(board) + Environment.NewLine + DrawLegend();
        var enemyBoardPrint = DrawEnemyBoard(enemyBoard) + Environment.NewLine + DrawEnemyLegend();
        var boards = playersBoardPrint.JoinByNewLine(enemyBoardPrint, " | ");

        return boards;
    }

    private string DrawLegend()
    {
        var builder = new StringBuilder();
        builder.AppendLine("'" + Water + "' -> water");
        builder.AppendLine("'" + Miss + "' -> miss");
        builder.AppendLine("'" + MyShip + "' -> ship");
        builder.AppendLine("'" + HitShip + "' -> hit");
        builder.AppendLine();
        return builder.ToString();
    }

    private string DrawBoard(Board board)
    {
        var print = IterateOverBoard("Players board:", board,
            (cell, builder) =>
            {
                builder.Append(cell.HasShip
                    ? (cell.WasShot ? HitShip : MyShip)
                    : (cell.WasShot ? Miss : Water));
            });
        return print;
    }

    private string DrawEnemyLegend()
    {
        var builder = new StringBuilder();
        builder.AppendLine("'" + Water + "' -> water");
        builder.AppendLine("'" + Miss + "' -> miss");
        builder.AppendLine("'" + HitShip + "' -> hit");
        builder.AppendLine();
        return builder.ToString();
    }

    private string DrawEnemyBoard(Board board)
    {
        var result = IterateOverBoard("Enemy boards: ", board,
            (cell, builder) =>
            {
                builder.Append(cell.HasShip
                    ? (cell.WasShot ? HitShip : Water)
                    : (cell.WasShot ? Miss : Water));
            });
        return result;
    }

    private string IterateOverBoard(string playersBoard, Board board, Action<Cell, StringBuilder> action)
    {
        var builder = new StringBuilder();

        builder.AppendLine(playersBoard);
        builder.Append("\\");
        int[] numbers = Enumerable.Range(1, 10)
            .ToArray();
        builder.Append(string.Join("", numbers));
        builder.AppendLine();

        foreach (var numberY in numbers)
        {
            var letter = char.ToUpper((char) (numberY + 64));
            builder.Append(letter);

            foreach (var numberX in numbers)
            {
                var cell = board.Cells[numberX - 1, numberY - 1];
                action(cell, builder);
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }
}