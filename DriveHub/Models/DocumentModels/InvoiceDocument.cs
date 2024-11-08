using DriveHubModel;
using global::QuestPDF.Helpers;
using global::QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;

namespace DriveHub.Models.DocumentModels
{
    public class InvoiceDocument : IDocument
    {
        public static Image LogoImage { get; }
        public Booking Model { get; }

        public int TotalMinutes { get; set; }

        static InvoiceDocument()
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

        public InvoiceDocument(Booking model)
        {
            Model = model;
            TotalMinutes = (int)Math.Round((((DateTime)Model.EndTime - (DateTime)Model.StartTime).TotalMinutes), 0);
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
                        .Item().Text($"Tax Invoice #{Model.Invoice.InvoiceNumber}")
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                    column.Item().Text(text =>
                    {
                        text.Span("Issue date: ").SemiBold();
                        text.Span($"{Model.Invoice.DateTime:d}");
                    });
                });
                if (LogoImage != null)
                {
                    row.ConstantItem(75).Image(LogoImage);
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
                    row.RelativeItem().Component(new Invoicee("To", Model.ApplicationUser));
                });
                column.Item().Element(ComposeTable);
                column.Item().PaddingVertical(-6).PaddingRight(5).AlignRight().Text($"Subtotal: {(Model.Invoice.Amount * 0.89m):C}");
                column.Item().PaddingVertical(-6).PaddingRight(5).AlignRight().Text($"GST: {(Model.Invoice.Amount * 0.11m):C}");
                column.Item().PaddingVertical(-6).PaddingRight(5).AlignRight().Text($"Total paid: {Model.Invoice.Amount:C}").Bold();
            });
        }

        void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);  // Start Time
                    columns.RelativeColumn(4);  // Trip
                    columns.RelativeColumn();   // Minutes
                    columns.RelativeColumn();   // Price p/m
                });

                table.Header(header =>
                {
                    header.Cell().Text("Start Time").Style(headerStyle);
                    header.Cell().Text("Trip").Style(headerStyle);
                    header.Cell().Text("Minutes Used").Style(headerStyle);
                    header.Cell().AlignRight().Text("Price Per Minute").Style(headerStyle);

                    header.Cell().ColumnSpan(4).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
                });

                table.Cell().Element(CellStyle).Text($"{Model.StartTime}");
                table.Cell().Element(CellStyle).Text($"{Model.StartPod.Site.SiteName} to {Model.EndPod?.Site.SiteName}");
                table.Cell().Element(CellStyle).Text($"{TotalMinutes}");
                table.Cell().Element(CellStyle).AlignRight().Text($"{Model.PricePerMinute * 1:C}");

                static IContainer CellStyle(IContainer container) =>
                    container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
            });
        }
    }
}

public class Address
{
    public string CompanyName { get; set; } = "DriveHub";
    public string Street { get; set; } = "1/45 Obviously Fake St";
    public string City { get; set; } = "Melbourne";
    public string State { get; set; } = "VIC";
    public string Email { get; set; } = "billing@drivehub.au";
    public string Phone { get; set; } = "03 9845 1223";
}

public class AddressComponent : IComponent
{
    private string Title { get; }
    private Address Address { get; }

    public AddressComponent(string title, Address address)
    {
        Title = title;
        Address = address;
    }

    public void Compose(IContainer container)
    {
        container.ShowEntire().Column(column =>
        {
            column.Spacing(2);
            column.Item().Text(Title).SemiBold();
            column.Item().PaddingBottom(5).LineHorizontal(1);
            column.Item().Text(Address.CompanyName);
            column.Item().Text(Address.Street);
            column.Item().Text($"{Address.City}, {Address.State}");
            column.Item().Text(Address.Email);
            column.Item().Text(Address.Phone);
        });
    }
}

public class Invoicee : IComponent
{
    private string Title { get; }
    ApplicationUser? ApplicationUser { get; set; }

    public Invoicee(string title, ApplicationUser user)
    {
        Title = title;
        ApplicationUser = user;
    }

    public void Compose(IContainer container)
    {
        string name;
        string email;
        if (ApplicationUser != null)
        {
            name = $"{ApplicationUser.FirstName} {ApplicationUser.LastName}";
            email = ApplicationUser.UserName;
        }
        else
        {
            name = "";
            email = "";
        }
        container.ShowEntire().Column(column =>
        {
            column.Spacing(2);
            column.Item().Text(Title).SemiBold();
            column.Item().PaddingBottom(5).LineHorizontal(1);
            column.Item().Text(name);
            column.Item().Text(email);
        });
    }
}
