using System.Collections.Generic;
using System.IO;
using FileChemy.Pipeline;

namespace FileChemy
{
	public interface IDocumentConverter
	{
		ConversionResult ConvertToImage(Stream input, DocumentType type, ImageOptions options);
		ConversionResult ConvertToPdf(Stream input, DocumentType type, PdfOptions options);
		BatchConversionResult ConvertBatch(IEnumerable<ConversionRequest> requests);
	}

	public class DocumentConverter : IDocumentConverter
	{
		private readonly IConversionPipeline _pipeline;

		public DocumentConverter(IConversionPipeline pipeline)
		{
			_pipeline = pipeline;
		}

		public ConversionResult ConvertToImage(Stream input, DocumentType type, ImageOptions options)
		{
			var request = new ConversionRequest
			{
				Input = input,
				DocumentType = type,
				OutputFormat = options.ImageFormat,
				Options = new ConversionOptions
				{
					DefaultImageFormat = options.ImageFormat
				}
			};

			return _pipeline.Execute(request);
		}

		public ConversionResult ConvertToPdf(Stream input, DocumentType type, PdfOptions options)
		{
			var request = new ConversionRequest
			{
				Input = input,
				DocumentType = type,
				OutputFormat = OutputFormat.Pdf,
				Options = new ConversionOptions
				{
					DefaultPdfOptions = options
				}
			};

			return _pipeline.Execute(request);
		}

		public BatchConversionResult ConvertBatch(IEnumerable<ConversionRequest> requests)
		{
			var results = new List<ConversionResult>();
			
			foreach (var request in requests)
			{
				var result = _pipeline.Execute(request);
				results.Add(result);
			}

			return new BatchConversionResult
			{
				Results = results
			};
		}
	}
}


