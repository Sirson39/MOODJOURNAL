using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MOODJOURNAL.Models;
using Colors = QuestPDF.Helpers.Colors;

namespace MOODJOURNAL.Services
{
    public class ExportService
    {
        public byte[] GeneratePdf(List<JournalEntry> entries, string dateRange)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Helvetica"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MoodJournal Export").FontSize(24).ExtraBold().FontColor(Colors.Orange.Medium);
                            col.Item().Text(dateRange).FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        foreach (var entry in entries)
                        {
                            col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(10).Column(inner =>
                            {
                                inner.Item().Row(r =>
                                {
                                    r.RelativeItem().Text(entry.Date.ToString("dddd, MMM dd, yyyy")).FontSize(10).SemiBold();
                                    r.RelativeItem().AlignRight().Text(entry.PrimaryMood).FontSize(10).FontColor(Colors.Orange.Accent3);
                                });

                                inner.Item().Text(entry.Title).FontSize(16).Bold();
                                inner.Item().PaddingTop(5).Text(entry.ContentRaw);

                                if (entry.Tags.Any())
                                {
                                    inner.Item().PaddingTop(5).Text("Tags: " + string.Join(", ", entry.Tags)).FontSize(8).Italic();
                                }
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
