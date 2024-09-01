namespace HotelBookingSystem.Models
{
    using DinkToPdf;
    using DinkToPdf.Contracts;

    public class PdfService
    {
        private readonly IConverter _converter;

        public PdfService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] ConvertHtmlToPdf(string htmlContent)
        {
            var document = new HtmlToPdfDocument
            {
                GlobalSettings = { PaperSize = PaperKind.A4 },
                Objects = { new ObjectSettings { HtmlContent = htmlContent } }
            };

            return _converter.Convert(document);
        }
    }

}
