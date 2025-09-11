namespace FileChemy
{
	public sealed class ConversionOptions
	{
		public OutputFormat DefaultImageFormat { get; set; }
		public PdfOptions DefaultPdfOptions { get; set; }
		public bool EnableBatchProcessing { get; set; }
		
		public ConversionOptions()
		{
			DefaultImageFormat = OutputFormat.Png;
			DefaultPdfOptions = new PdfOptions();
			EnableBatchProcessing = true;
		}
	}
}


