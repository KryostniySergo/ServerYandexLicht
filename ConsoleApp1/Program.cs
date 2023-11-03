using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Writer;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string file_type = "application/pdf";
            const string file_name = "sample.pdf";

            PdfDocumentBuilder builder = new PdfDocumentBuilder();

            PdfPageBuilder page = builder.AddPage(PageSize.A4);
            var fileb = File.ReadAllBytes(@"D:\MyFuckingProgramms\YANDEX_LIHTCINDER\New\ServerViewYa\ConsoleApp1\Roboto-Regular.ttf");

            var font = builder.AddTrueTypeFont(fileb);

            page.AddText("Блядьб", 12, new PdfPoint(10, 700), font);

            byte[] documentBytes = builder.Build();

            File.WriteAllBytes(@"D:\MyFuckingProgramms\YANDEX_LIHTCINDER\New\ServerViewYa\ConsoleApp1\mypdf.pdf", documentBytes);
        }
    }
}