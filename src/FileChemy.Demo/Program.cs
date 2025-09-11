using System;
using System.IO;
using FileChemy;
using FileChemy.Implementations;

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
            var options = FileChemy.DefaultOptions;
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

            // Excel ���� �׽�Ʈ ��� �߰�
            Console.WriteLine("=== Excel ���� �׽�Ʈ ===");
            Console.WriteLine("Excel ������ �׽�Ʈ�Ͻðڽ��ϱ�? (y/n)");
            
            if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
            {
                Console.WriteLine();
                TestExcelFile();
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

        static void TestExcelFile()
        {
            Console.WriteLine("Excel ���� ��θ� �Է��ϼ��� (��: C:\\test.xlsx):");
            string filePath = Console.ReadLine().Replace("\"", "");

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("������ �������� �ʽ��ϴ�. �׽�Ʈ Excel ������ �����մϴ�...");
                filePath = CreateTestExcelFile();
            }

            try
            {
                Console.WriteLine($"Excel ���� �б� ����: {filePath}");
                
                // ExcelDocumentReader�� ���� ����Ͽ� �׽�Ʈ
                var excelReader = new ExcelDocumentReader();
                
                using (var fileStream = File.OpenRead(filePath))
                {
                    Console.WriteLine("Excel ������ �д� ��...");
                    var documentContent = excelReader.Read(fileStream);
                    
                    Console.WriteLine("=== Excel �б� ��� ===");
                    Console.WriteLine($"���� ���: {documentContent.FilePath}");
                    Console.WriteLine($"���� Ÿ��: {documentContent.Type}");
                    Console.WriteLine($"������ ũ��: {documentContent.Data.Length} bytes");
                    
                    if (documentContent.Data.Length > 0)
                    {
                        // �����͸� ���ڿ��� ��ȯ�Ͽ� ���
                        string dataString = System.Text.Encoding.UTF8.GetString(documentContent.Data);
                        Console.WriteLine("=== Excel ���� ===");
                        Console.WriteLine(dataString);
                    }
                }
                
                Console.WriteLine("Excel ���� �б� �Ϸ�!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"���� �߻�: {ex.Message}");
                Console.WriteLine($"�� ����: {ex.ToString()}");
            }
        }

        static string CreateTestExcelFile()
        {
            Console.WriteLine("�׽�Ʈ�� Excel ���� ���� ����� Excel�� ��ġ�� ȯ�濡���� �۵��մϴ�.");
            Console.WriteLine("���� Excel ������ ��θ� �ٽ� �Է��� �ּ���:");
            
            string newPath = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPath) && File.Exists(newPath))
            {
                return newPath;
            }
            
            throw new FileNotFoundException("��ȿ�� Excel ������ ã�� �� �����ϴ�.");
        }
    }
}