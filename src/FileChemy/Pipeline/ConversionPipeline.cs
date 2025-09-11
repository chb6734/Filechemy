using FileChemy.Factories;

namespace FileChemy.Pipeline
{
	public class ConversionPipeline : IConversionPipeline
	{
		private readonly DocumentReadStep _readStep;
		private readonly RenderStep _renderStep;

		public ConversionPipeline(IDocumentReaderFactory readerFactory, IRendererFactory rendererFactory)
		{
			_readStep = new DocumentReadStep(readerFactory);
			_renderStep = new RenderStep(rendererFactory);
		}

		public ConversionResult Execute(ConversionRequest request)
		{
			try
			{
				// Step 1: 문서 읽기
				var content = _readStep.Execute(request);

				// Step 2: 렌더링 컨텍스트 생성
				var renderContext = new RenderContext
				{
					Content = content,
					OutputFormat = request.OutputFormat,
					Options = request.Options
				};

				// Step 3: 렌더링
				var result = _renderStep.Execute(renderContext);
				
				return result;
			}
			catch (System.Exception ex)
			{
				return new ConversionResult
				{
					Succeeded = false,
					ErrorMessage = ex.Message,
					Outputs = new byte[0][]
				};
			}
		}
	}
}