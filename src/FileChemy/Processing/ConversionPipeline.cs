using System.IO;
using System.Threading.Tasks;

namespace FileChemy
{
	public sealed class ConversionPipeline
	{
		private readonly IDocumentReader _excelReader;
		private readonly IDocumentReader _wordReader;
		private readonly IImageRenderer _pngRenderer;
		private readonly IImageRenderer _jpegRenderer;
		private readonly IImageRenderer _bitmapRenderer;
		private readonly IPdfRenderer _pdfRenderer;

		public ConversionPipeline(
			IDocumentReader excelReader,
			IDocumentReader wordReader,
			IImageRenderer pngRenderer,
			IImageRenderer jpegRenderer,
			IImageRenderer bitmapRenderer,
			IPdfRenderer pdfRenderer)
		{
			_excelReader = excelReader;
			_wordReader = wordReader;
			_pngRenderer = pngRenderer;
			_jpegRenderer = jpegRenderer;
			_bitmapRenderer = bitmapRenderer;
			_pdfRenderer = pdfRenderer;
		}

		public async Task<ConversionResult> ProcessAsync(Stream input, DocumentType inputType, OutputFormat outputFormat, ConversionOptions options)
		{
			var reader = inputType == DocumentType.Excel ? _excelReader : _wordReader;
			var content = await reader.ReadAsync(input);

			if (outputFormat == OutputFormat.Pdf)
			{
				return await _pdfRenderer.RenderAsync(content, options.DefaultPdfOptions);
			}

			var imageOptions = new ImageOptions { ImageFormat = outputFormat };
			var renderer = outputFormat switch
			{
				OutputFormat.Png => _pngRenderer,
				OutputFormat.Jpeg => _jpegRenderer,
				OutputFormat.Bitmap => _bitmapRenderer,
				_ => _pngRenderer
			};

			return await renderer.RenderAsync(content, imageOptions);
		}
	}
}


