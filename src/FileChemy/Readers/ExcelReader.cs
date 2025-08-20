using System.IO;
using System.Threading.Tasks;

namespace FileChemy
{
	public sealed class ExcelReader : IDocumentReader
	{
		public DocumentType SupportedType => DocumentType.Excel;

		public Task<DocumentContent> ReadAsync(Stream input)
		{
			return Task.FromResult(new DocumentContent());
		}

		public Task<DocumentMetadata> GetMetadataAsync(Stream input)
		{
			return Task.FromResult(new DocumentMetadata());
		}
	}
}


