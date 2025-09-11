using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileChemy
{
	public sealed class BatchProcessor
	{
		private readonly ConversionPipeline _pipeline;

		public BatchProcessor(ConversionPipeline pipeline)
		{
			_pipeline = pipeline;
		}

		public async Task<BatchConversionResult> ProcessBatchAsync(IEnumerable<ConversionRequest> requests)
		{
			var results = new List<ConversionResult>();
			foreach (var request in requests)
			{
				results.Add(await _pipeline.ProcessAsync(request.Input, request.DocumentType, request.OutputFormat, request.Options));
			}
			return new BatchConversionResult(results);
		}
	}
}


