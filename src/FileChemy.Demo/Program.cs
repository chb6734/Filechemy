using System;
using System.IO;
using FileChemy;

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
            var options = FileChemy.FileChemy.DefaultOptions;
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
    }
}