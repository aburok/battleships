using ConsoleApp1;
using NUnit.Framework;
using Shouldly;

namespace BattleShips.Tests;

public class BoardTests
{
    [Test]
    [TestCase("A1", 0, 0)]
    [TestCase("A10", 9, 0)]
    [TestCase("J10", 9, 9)]
    [TestCase("J1", 0, 9)]
    [TestCase("F5", 4, 5)]
    public void Given_Board_When_GetCells_Then_ReturnCells(string position, int x, int y)
    {
        var board = new Board();

        var cell = board.GetCell(position);
        
        cell.X.ShouldBe(x);
        cell.Y.ShouldBe(y);
    }
    
    [Test]
    public void Given_Board_When_GetCells_Then_ReturnCells()
    {
        var board = new Board();

        var cell = board.GetCell("A5");
        
        cell.X.ShouldBe(4);
        cell.Y.ShouldBe(0);
    }
    
    [Test]
    public void Given_Board_When_GetCellsOutsideBoard_Then_ReturnException()
    {
        var board = new Board();

        var cell = board.GetCell("Z5");
        
        cell.ShouldBeNull();
    }
}