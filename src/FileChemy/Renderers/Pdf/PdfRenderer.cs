using System.Threading.Tasks;

namespace FileChemy
{
	public sealed class PdfRenderer : IPdfRenderer
	{
		public bool SupportsFormat(OutputFormat format) => format == OutputFormat.Pdf;

		public Task<ConversionResult> RenderAsync(DocumentContent content, PdfOptions options)
		{
			return Task.FromResult(new ConversionResult(true, new[] { new byte[0] }));
		}
	}
}


