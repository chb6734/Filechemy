using System.IO;

namespace FileChemy.Implementations
{
	public class ExcelDocumentReader : IDocumentReader
	{
		public DocumentType SupportedType => DocumentType.Excel;

		public DocumentContent Read(Stream input)
		{
			// TODO: Excel 파일 읽기 구현
			// 실제로는 EPPlus, ClosedXML 또는 Excel Interop을 사용할 예정
			
			var buffer = new byte[input.Length];
			input.Read(buffer, 0, buffer.Length);
			
			return new DocumentContent
			{
				Data = buffer,
				Type = DocumentType.Excel
			};
		}

		public DocumentMetadata GetMetadata(Stream input)
		{
			// TODO: Excel 메타데이터 읽기 구현
			return new DocumentMetadata
			{
				Title = "Excel Document",
				Author = "Unknown",
				PageCount = 1 // Excel의 경우 워크시트 수로 변경 필요
			};
		}
	}

	public class WordDocumentReader : IDocumentReader
	{
		public DocumentType SupportedType => DocumentType.Word;

		public DocumentContent Read(Stream input)
		{
			// TODO: Word 파일 읽기 구현
			// 실제로는 OpenXML SDK 또는 Word Interop을 사용할 예정
			
			var buffer = new byte[input.Length];
			input.Read(buffer, 0, buffer.Length);
			
			return new DocumentContent
			{
				Data = buffer,
				Type = DocumentType.Word
			};
		}

		public DocumentMetadata GetMetadata(Stream input)
		{
			// TODO: Word 메타데이터 읽기 구현
			return new DocumentMetadata
			{
				Title = "Word Document",
				Author = "Unknown",
				PageCount = 1 // 실제 페이지 수로 변경 필요
			};
		}
	}
}