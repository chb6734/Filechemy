using System.IO;

namespace FileChemy
{
	public interface IRenderer<TOptions>
	{
		ConversionResult Render(DocumentContent content, TOptions options);
		bool SupportsFormat(OutputFormat format);
	}
}


