using System.Text;

public static class StringExtensions
{
    public static string JoinByNewLine(this string text, string otherText, string? delimiter = "")
    {
        var textLines = text.Split(Environment.NewLine);
        var otherTextLines = otherText.Split(Environment.NewLine);

        var maxLineCount = Math.Max(textLines.Length, otherTextLines.Length);
        var maxTextWidth = textLines.Max(t => t.Length) + 1;
        var maxOtherTextWidth = otherTextLines.Max(t => t.Length) + 1;

        var format = "{0,-" + maxTextWidth + "}{1}{2,-" + maxOtherTextWidth + "}";
        
        var builder = new StringBuilder();
        for (int i = 0; i < maxLineCount; i++)
        {
            var textLine = textLines.Length > i ? textLines[i] : string.Empty;
            var otherTextLine = otherTextLines.Length > i ? otherTextLines[i] : string.Empty;
            builder.AppendFormat(format,
                textLine,
                delimiter,
                otherTextLine);
            builder.AppendLine();
        }

        return builder.ToString();
    }
}