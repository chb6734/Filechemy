using FileChemy.Factories;

namespace FileChemy.Pipeline
{
	public interface IPipelineStep<TInput, TOutput>
	{
		TOutput Execute(TInput input);
	}

	public interface IConversionPipeline
	{
		ConversionResult Execute(ConversionRequest request);
	}
}