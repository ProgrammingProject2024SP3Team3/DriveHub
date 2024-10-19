using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using DriveHubModel;

namespace DriveHub.Models.DocumentModels
{
    public class BookingsDocument : IDocument
    {
        public IList<Booking> Model { get; }

        public void Compose(IDocumentContainer container)
        {
            throw new NotImplementedException();
        }
    }
}
