using FileChemy.Factories;

namespace FileChemy.Pipeline
{
	// Step 1: 문서 읽기
	public class DocumentReadStep : IPipelineStep<ConversionRequest, DocumentContent>
	{
		private readonly IDocumentReaderFactory _readerFactory;

		public DocumentReadStep(IDocumentReaderFactory readerFactory)
		{
			_readerFactory = readerFactory;
		}

		public DocumentContent Execute(ConversionRequest input)
		{
			var reader = _readerFactory.CreateReader(input.DocumentType);
			return reader.Read(input.Input);
		}
	}

	// Step 2: 렌더링
	public class RenderStep : IPipelineStep<RenderContext, ConversionResult>
	{
		private readonly IRendererFactory _rendererFactory;

		public RenderStep(IRendererFactory rendererFactory)
		{
			_rendererFactory = rendererFactory;
		}

		public ConversionResult Execute(RenderContext input)
		{
			if (input.OutputFormat == OutputFormat.Pdf)
			{
				var pdfRenderer = _rendererFactory.CreatePdfRenderer();
				var pdfOptions = input.Options != null && input.Options.DefaultPdfOptions != null 
					? input.Options.DefaultPdfOptions 
					: new PdfOptions();
				return pdfRenderer.Render(input.Content, pdfOptions);
			}
			else
			{
				var imageRenderer = _rendererFactory.CreateImageRenderer(input.OutputFormat);
				var imageOptions = new ImageOptions { ImageFormat = input.OutputFormat };
				return imageRenderer.Render(input.Content, imageOptions);
			}
		}
	}

	public class RenderContext
	{
		public DocumentContent Content { get; set; }
		public OutputFormat OutputFormat { get; set; }
		public ConversionOptions Options { get; set; }
	}
}