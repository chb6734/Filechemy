using System.Threading.Tasks;

namespace FileChemy
{
	public sealed class JpegRenderer : IImageRenderer
	{
		public bool SupportsFormat(OutputFormat format) => format == OutputFormat.Jpeg;

		public Task<ConversionResult> RenderAsync(DocumentContent content, ImageOptions options)
		{
			return Task.FromResult(new ConversionResult(true, new[] { new byte[0] }));
		}
	}
}


