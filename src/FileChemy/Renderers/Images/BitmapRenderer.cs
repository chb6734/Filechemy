using System.Threading.Tasks;

namespace FileChemy
{
	public sealed class BitmapRenderer : IImageRenderer
	{
		public bool SupportsFormat(OutputFormat format) => format == OutputFormat.Bitmap;

		public Task<ConversionResult> RenderAsync(DocumentContent content, ImageOptions options)
		{
			return Task.FromResult(new ConversionResult(true, new[] { new byte[0] }));
		}
	}
}


