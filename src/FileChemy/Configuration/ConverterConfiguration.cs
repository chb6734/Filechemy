namespace FileChemy
{
	public sealed class ConverterConfiguration
	{
		public OutputSettings OutputSettings { get; set; } = new OutputSettings();
		public ConversionOptions Defaults { get; set; } = new ConversionOptions();
	}
}


