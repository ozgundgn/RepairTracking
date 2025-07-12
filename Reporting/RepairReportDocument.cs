using System;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using RepairTracking.ViewModels;
using Colors = Avalonia.Media.Colors;

namespace RepairTracking.Reporting;

public class RepairReportDocument : IDocument
{
    private readonly RenovationViewModel _repairData;
    private readonly Byte[] image;

    public RepairReportDocument(RenovationViewModel repairData)
    {
        _repairData = repairData;
        image = File.ReadAllBytes("Assets/ozenir-png.png");
    }

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                // Define page margins
                page.Margin(50);

                // === Header ===
                page.Header().Element(ComposeHeader);

                // === Content ===
                page.Content().Element(ComposeContent);
                // === Footer ===
                page.Footer().Element(ComposeFooter);
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.ConstantItem(150).Column(column => { column.Item().Height(80).Image(image); });

            row.RelativeItem().AlignRight().Column(column =>
            {
                column.Item().AlignRight().Text("ARAÇ KABUL FORMU")
                    .SemiBold().FontSize(14).FontColor(Colors.MidnightBlue.A);
                column.Item().AlignRight().Text("BAĞIMSIZ ÖZEL SERVİS")
                    .SemiBold().FontSize(12).FontColor(Colors.MidnightBlue.A);
                column.Item().AlignRight().Text($"Tel: 0535 568 30 22").FontSize(8);
                column.Item().AlignRight().Text($"Tel: 0532 431 13 91").FontSize(8);
                column.Item().AlignRight().Text($"Veyselkarani Mah. 10.Lale Sok. No: 14, Osmangazi, BURSA").FontSize(8);
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(2);
            // Here is the table for the repair details
            column.Item().Element(ComposeHeaderInfoTable);
            column.Item().Element(ComposeDetailsTable);
            var notes = _repairData.RenovationDetails.Select(x => x.Note).Where(x => !string.IsNullOrEmpty(x)).ToList();
            var joinedNotes = string.Join(", ", notes);
            column.Item().MultiColumn(handler =>
            {
                handler.Content().Text($"NOT: {joinedNotes}").SemiBold().FontSize(12);
            });

            // Add a space between the table and the total price
            column.Item().Height(20);
            // Total Price
            var totalPrice = _repairData.RenovationDetails.Sum(x => x.Price);
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("'Bu belgenin mali değeri yoktur.'").FontSize(10);
                    col.Item().Text("'Araç sahibi, işlerin yapılmasını onaylar ve kabul eder.'").FontSize(10);
                    col.Item().Height(20);
                    col.Item().Row(rr =>
                    {
                        rr.RelativeItem().Column(cl => { cl.Item().Text("Müşteri Onayı").FontSize(10); });
                        rr.RelativeItem().Column(cl => { cl.Item().Text("Servis Onayı").FontSize(10); });
                    });
                });
                row.RelativeItem().Column(col =>
                {
                    col.Item().AlignRight().Text($"TOPLAM ÜCRET:{totalPrice:C}").SemiBold().FontSize(14);
                });
            });
        });
    }

    void ComposeDetailsTable(IContainer container)
    {
        container
            .Border(1)
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25); // Item #
                    columns.RelativeColumn(5); // Description
                    columns.RelativeColumn(2); // Name
                    columns.RelativeColumn(1); // T.Code
                    columns.RelativeColumn(2); // Price
                });

                table.Header(header =>
                {
                    header.Cell().Text("#");
                    header.Cell().AlignCenter().Text("AÇIKLAMA").SemiBold();
                    header.Cell().AlignCenter().Text("AD").SemiBold();
                    header.Cell().AlignCenter().Text("T. KOD").SemiBold();
                    header.Cell().AlignCenter().Text("TUTAR").SemiBold();
                });
                var index = 1;
                // Add rows for each renovation detail
                foreach (var detail in _repairData.RenovationDetails)
                {
                    table.Cell().TableValueCell().Text(index++);
                    table.Cell().TableValueCell().Text(detail.Description);
                    table.Cell().TableValueCell().Text(detail.Name);
                    table.Cell().TableValueCell().Text(detail.TCode);
                    table.Cell().TableValueCell().Text(detail.Price.ToString("C"));
                }
            });
    }

    void ComposeHeaderInfoTable(IContainer container)
    {
        container
            .Border(1)
            .Table(table =>
            {
                table.Header(header =>
                {
                    header.Cell().ColumnSpan(4).RowSpan(3).AlignCenter()
                        .Text("UYGULAMA EMRİ").FontSize(18).FontColor(QuestPDF.Helpers.Colors.Black);
                });
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(3);
                });

                table.Cell().TableLabelCell("İsim");
                table.Cell().TableValueCell().Text(_repairData.CustomerName);

                table.Cell().TableLabelCell("Uygulama Tarihi");
                table.Cell().TableValueCell().Text(_repairData.RepairDate);

                table.Cell().TableLabelCell("Soyisim");
                table.Cell().TableValueCell().Text(_repairData.CustomerSurname);

                table.Cell().TableLabelCell("Plaka No");
                table.Cell().TableValueCell().Text(_repairData.Vehicle?.PlateNumber);

                table.Cell().TableLabelCell("Tel");
                table.Cell().TableValueCell().Text(_repairData.Vehicle?.PlateNumber);

                table.Cell().TableLabelCell("Araç Tipi");
                table.Cell().TableValueCell().Text(_repairData.Vehicle?.Type);

                table.Cell().TableLabelCell("Email");
                table.Cell().TableValueCell().Text(_repairData.CustomerName);

                table.Cell().TableLabelCell("Şasi No");
                table.Cell().TableValueCell().Text(_repairData.Vehicle?.ChassisNo);

                table.Cell().ColumnSpan(1).RowSpan(3).TableLabelCell("Adres");
                table.Cell().ColumnSpan(1).RowSpan(3).TableValueCell().Text(_repairData.Address);

                // table.Cell().ColumnSpan(2).TableValueCell().AspectRatio(16 / 9f).Image(Placeholders.Image);

                table.Cell().Row(row =>
                {
                    row.ConstantItem(50).TableLabelCell("Km:");
                    row.RelativeItem().TableValueCell().AlignCenter().Text(_repairData.Vehicle?.Km.ToString());
                });

                table.Cell().Row(row =>
                {
                    row.ConstantItem(50).TableLabelCell("Yakıt:");
                    row.RelativeItem().TableValueCell().AlignCenter().Text(_repairData.Vehicle?.Fuel);
                });

                table.Cell().TableLabelCell("Teslim Tarihi");
                table.Cell().TableValueCell().Text(DateTime.Now.ToString("dd/MM/yyyy"));

                // table.Cell().TableLabelCell("Motor No");
                // table.Cell().TableValueCell().Text(_repairData.Vehicle.M);

                table.Cell().Row(row =>
                {
                    row.ConstantItem(50).TableLabelCell("Model:");
                    row.RelativeItem().TableValueCell().AlignCenter().Text(_repairData.Vehicle?.Model.ToString());
                });

                table.Cell().Row(row =>
                {
                    row.ConstantItem(50).TableLabelCell("Renk:");
                    row.RelativeItem().TableValueCell().AlignCenter().Text(_repairData.Vehicle?.Color);
                });
            });
    }

    void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Page ");
            x.CurrentPageNumber();
        });
    }
}