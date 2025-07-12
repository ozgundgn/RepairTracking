
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public static class TableExtensions
{
    private static IContainer TableCellStyle(this IContainer container, string backgroundColor)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Black)
            .Background(backgroundColor)
            .Padding(5);
    }
    
    public static void TableLabelCell(this IContainer container, string text)
    {
        container
            .TableCellStyle(Colors.Grey.Lighten3)
            .Text(text)
            .Bold();
    }
    
    public static IContainer TableValueCell(this IContainer container)
    {
        return container.TableCellStyle(Colors.Transparent);
    }
}