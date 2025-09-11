using System.IO;

namespace FileChemy
{
	public interface IDocumentReader
	{
		DocumentType SupportedType { get; }
		DocumentContent Read(Stream input);
		DocumentMetadata GetMetadata(Stream input);
	}
}


