using System.IO;

namespace FileChemy.Implementations
{
	public class ExcelDocumentReader : IDocumentReader
	{
		public DocumentType SupportedType => DocumentType.Excel;

		public DocumentContent Read(Stream input)
		{
			// TODO: Excel ���� �б� ����
			// �����δ� EPPlus, ClosedXML �Ǵ� Excel Interop�� ����� ����
			
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
			// TODO: Excel ��Ÿ������ �б� ����
			return new DocumentMetadata
			{
				Title = "Excel Document",
				Author = "Unknown",
				PageCount = 1 // Excel�� ��� ��ũ��Ʈ ���� ���� �ʿ�
			};
		}
	}

	public class WordDocumentReader : IDocumentReader
	{
		public DocumentType SupportedType => DocumentType.Word;

		public DocumentContent Read(Stream input)
		{
			// TODO: Word ���� �б� ����
			// �����δ� OpenXML SDK �Ǵ� Word Interop�� ����� ����
			
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
			// TODO: Word ��Ÿ������ �б� ����
			return new DocumentMetadata
			{
				Title = "Word Document",
				Author = "Unknown",
				PageCount = 1 // ���� ������ ���� ���� �ʿ�
			};
		}
	}
}