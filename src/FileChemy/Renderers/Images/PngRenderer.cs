using System.Threading.Tasks;

namespace FileChemy
{
	public sealed class PngRenderer : IImageRenderer
	{
		public bool SupportsFormat(OutputFormat format) => format == OutputFormat.Png;

		public Task<ConversionResult> RenderAsync(DocumentContent content, ImageOptions options)
		{
			return Task.FromResult(new ConversionResult(true, new[] { new byte[0] }));
		}
	}
}


