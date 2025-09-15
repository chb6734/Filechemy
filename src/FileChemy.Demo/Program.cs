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
            Console.WriteLine("=== FileChemy 데모 프로그램 ===");
            Console.WriteLine("문서를 이미지나 PDF로 변환하는 라이브러리입니다.");
            Console.WriteLine();

            // 기본 옵션 표시
            var options = FileChemy.DefaultOptions;
            Console.WriteLine("기본 설정:");
            Console.WriteLine($"- 기본 이미지 포맷: {options.DefaultImageFormat}");
            Console.WriteLine($"- PDF 품질: {options.DefaultPdfOptions.Quality}");
            Console.WriteLine($"- 폰트 임베딩: {options.DefaultPdfOptions.EmbedFonts}");
            Console.WriteLine($"- 배치 처리 활성화: {options.EnableBatchProcessing}");
            Console.WriteLine();

            // 지원하는 문서 형식과 출력 포맷 표시
            Console.WriteLine("지원 문서 형식:");
            foreach (DocumentType docType in Enum.GetValues(typeof(DocumentType)))
            {
                Console.WriteLine($"- {docType}");
            }
            Console.WriteLine();

            Console.WriteLine("지원 출력 포맷:");
            foreach (OutputFormat format in Enum.GetValues(typeof(OutputFormat)))
            {
                Console.WriteLine($"- {format}");
            }
            Console.WriteLine();

            // Excel 파일 테스트 기능 추가
            Console.WriteLine("=== Excel 파일 테스트 ===");
            Console.WriteLine("Excel 파일을 테스트하시겠습니까? (y/n)");
            
            if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
            {
                Console.WriteLine();
                TestExcelFile();
            }
            Console.WriteLine();

            // 샘플 변환 요청 생성 (실제 파일 없이 데모용)
            Console.WriteLine("=== 샘플 변환 설정 예시 ===");
            
            var imageOptions = new ImageOptions
            {
                ImageFormat = OutputFormat.Png,
                Quality = 95,
                Dpi = 300
            };
            Console.WriteLine($"이미지 옵션: {imageOptions.ImageFormat}, 품질: {imageOptions.Quality}, DPI: {imageOptions.Dpi}");

            var pdfOptions = new PdfOptions
            {
                Quality = 90,
                EmbedFonts = false
            };
            Console.WriteLine($"PDF 옵션: 품질: {pdfOptions.Quality}, 폰트 임베딩: {pdfOptions.EmbedFonts}");

            // 메타데이터 샘플
            var metadata = new DocumentMetadata
            {
                Title = "샘플 문서",
                Author = "FileChemy Demo",
                PageCount = 10
            };
            Console.WriteLine($"문서 메타데이터: {metadata.Title} by {metadata.Author}, {metadata.PageCount} 페이지");

            Console.WriteLine();
            Console.WriteLine("실제 파일 변환을 위해서는 DocumentConverter와 관련 렌더러들이 구현되어야 합니다.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void TestExcelFile()
        {
            Console.WriteLine("Excel 파일 경로를 입력하세요 (예: C:\\test.xlsx):");
            string filePath = Console.ReadLine().Replace("\"", "");

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("파일이 존재하지 않습니다. 테스트 Excel 파일을 생성합니다...");
                filePath = CreateTestExcelFile();
            }

            try
            {
                Console.WriteLine($"Excel 파일 읽기 시작: {filePath}");
                
                // ExcelDocumentReader를 직접 사용하여 테스트
                var excelReader = new ExcelDocumentReader();
                
                using (var fileStream = File.OpenRead(filePath))
                {
                    Console.WriteLine("Excel 파일을 읽는 중...");
                    var documentContent = excelReader.Read(fileStream);
                    
                    Console.WriteLine("=== Excel 읽기 결과 ===");
                    Console.WriteLine($"파일 경로: {documentContent.FilePath}");
                    Console.WriteLine($"문서 타입: {documentContent.Type}");
                    Console.WriteLine($"데이터 크기: {documentContent.Data.Length} bytes");
                    
                    if (documentContent.Data.Length > 0)
                    {
                        // 데이터를 문자열로 변환하여 출력
                        string dataString = System.Text.Encoding.UTF8.GetString(documentContent.Data);
                        Console.WriteLine("=== Excel 내용 ===");
                        Console.WriteLine(dataString);
                    }
                }
                
                Console.WriteLine("Excel 파일 읽기 완료!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류 발생: {ex.Message}");
                Console.WriteLine($"상세 오류: {ex.ToString()}");
            }
        }

        static string CreateTestExcelFile()
        {
            Console.WriteLine("테스트용 Excel 파일 생성 기능은 Excel이 설치된 환경에서만 작동합니다.");
            Console.WriteLine("기존 Excel 파일의 경로를 다시 입력해 주세요:");
            
            string newPath = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPath) && File.Exists(newPath))
            {
                return newPath;
            }
            
            throw new FileNotFoundException("유효한 Excel 파일을 찾을 수 없습니다.");
        }
    }
}