using System;
using System.IO;
using FileChemy;

namespace FileChemy.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== FileChemy ���� ���α׷� ===");
            Console.WriteLine("������ �̹����� PDF�� ��ȯ�ϴ� ���̺귯���Դϴ�.");
            Console.WriteLine();

            // �⺻ �ɼ� ǥ��
            var options = FileChemy.FileChemy.DefaultOptions;
            Console.WriteLine("�⺻ ����:");
            Console.WriteLine($"- �⺻ �̹��� ����: {options.DefaultImageFormat}");
            Console.WriteLine($"- PDF ǰ��: {options.DefaultPdfOptions.Quality}");
            Console.WriteLine($"- ��Ʈ �Ӻ���: {options.DefaultPdfOptions.EmbedFonts}");
            Console.WriteLine($"- ��ġ ó�� Ȱ��ȭ: {options.EnableBatchProcessing}");
            Console.WriteLine();

            // �����ϴ� ���� ���İ� ��� ���� ǥ��
            Console.WriteLine("���� ���� ����:");
            foreach (DocumentType docType in Enum.GetValues(typeof(DocumentType)))
            {
                Console.WriteLine($"- {docType}");
            }
            Console.WriteLine();

            Console.WriteLine("���� ��� ����:");
            foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
            {
                Console.WriteLine($"- {format}");
            }
            Console.WriteLine();

            // ���� ��ȯ ��û ���� (���� ���� ���� �����)
            Console.WriteLine("=== ���� ��ȯ ���� ���� ===");
            
            var imageOptions = new ImageOptions
            {
                ImageFormat = OutputFormat.Png,
                Quality = 95,
                Dpi = 300
            };
            Console.WriteLine($"�̹��� �ɼ�: {imageOptions.ImageFormat}, ǰ��: {imageOptions.Quality}, DPI: {imageOptions.Dpi}");

            var pdfOptions = new PdfOptions
            {
                Quality = 90,
                EmbedFonts = false
            };
            Console.WriteLine($"PDF �ɼ�: ǰ��: {pdfOptions.Quality}, ��Ʈ �Ӻ���: {pdfOptions.EmbedFonts}");

            // ��Ÿ������ ����
            var metadata = new DocumentMetadata
            {
                Title = "���� ����",
                Author = "FileChemy Demo",
                PageCount = 10
            };
            Console.WriteLine($"���� ��Ÿ������: {metadata.Title} by {metadata.Author}, {metadata.PageCount} ������");

            Console.WriteLine();
            Console.WriteLine("���� ���� ��ȯ�� ���ؼ��� DocumentConverter�� ���� ���������� �����Ǿ�� �մϴ�.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}