using DriveHubModel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DriveHub.Models.DocumentModels
{
    public class BookingsDocument : IDocument
    {
        public static Image LogoImage { get; }
        public IList<Booking> Bookings { get; }

        public decimal TotalAmount { get; } = 0;

        static BookingsDocument()
        {
            try
            {
                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo.png");
                LogoImage = Image.FromFile(logoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading logo: {ex.Message}");
                LogoImage = null;
            }
        }

        public BookingsDocument(IList<Booking> bookings)
        {
            Bookings = bookings;

            foreach (Booking booking in Bookings)
            {
                TotalAmount += booking.Invoice.Amount;
            }
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column
                        .Item().Text("Bookings Report")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                    column.Item().Text(text =>
                    {
                        text.Span("Issue date: ").SemiBold();
                        text.Span($"{DateTime.Now:d}");
                    });
                });
                if (LogoImage != null)
                {
                    row.ConstantItem(100).Image(LogoImage);
                }
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(20);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Component(new AddressComponent("From", new Address()));
                    row.ConstantItem(50);
                });
                column.Item().Element(ComposeTable);

                column.Item().PaddingRight(5).AlignRight().Text($"Total: {TotalAmount:C}").SemiBold();
            });
        }

        void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(); // Start Time
                    columns.RelativeColumn(); // Description
                    columns.RelativeColumn(); // Minutes Used
                    columns.RelativeColumn(); // Price Per Minute
                    columns.RelativeColumn(); // Total
                });

                table.Header(header =>
                {
                    header.Cell().Text("Start Time").Style(headerStyle);
                    header.Cell().Text("Description").Style(headerStyle);
                    header.Cell().Text("Minutes").Style(headerStyle);
                    header.Cell().Text("Price Per Minute").Style(headerStyle);
                    header.Cell().AlignRight().Text("Total").Style(headerStyle);

                    header.Cell().ColumnSpan(5).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                foreach (var booking in Bookings)
                {
                    var totalMinutes = (int)Math.Round((((DateTime)booking.EndTime - (DateTime)booking.StartTime).TotalMinutes), 0);
                    table.Cell().Element(CellStyle).Text($"{booking.StartTime}");
                    table.Cell().Element(CellStyle).Text("DriveHub ride share");
                    table.Cell().Element(CellStyle).Text($"{totalMinutes}");
                    table.Cell().Element(CellStyle).Text($"{booking.PricePerMinute:C}");
                    table.Cell().Element(CellStyle).AlignRight().Text($"{booking.Invoice?.Amount:C}");

                    static IContainer CellStyle(IContainer container) =>
                        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            });
        }
    }
}
