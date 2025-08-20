using FileChemy.Implementations;

namespace FileChemy.Factories
{
	public interface IDocumentReaderFactory
	{
		IDocumentReader CreateReader(DocumentType documentType);
	}

	public interface IRendererFactory
	{
		IImageRenderer CreateImageRenderer(OutputFormat format);
		IPdfRenderer CreatePdfRenderer();
	}

	public class DocumentReaderFactory : IDocumentReaderFactory
	{
		public IDocumentReader CreateReader(DocumentType documentType)
		{
			switch (documentType)
			{
				case DocumentType.Excel:
					return new ExcelDocumentReader();
				case DocumentType.Word:
					return new WordDocumentReader();
				default:
					throw new System.NotSupportedException($"Document type {documentType} is not supported");
			}
		}
	}

	public class RendererFactory : IRendererFactory
	{
		public IImageRenderer CreateImageRenderer(OutputFormat format)
		{
			// Strategy pattern: format에 따라 다른 renderer 반환
			switch (format)
			{
				case OutputFormat.Png:
				case OutputFormat.Jpeg:
				case OutputFormat.Bitmap:
					return new ImageRenderer();
				default:
					throw new System.NotSupportedException($"Image format {format} is not supported");
			}
		}

		public IPdfRenderer CreatePdfRenderer()
		{
			return new PdfRenderer();
		}
	}
}