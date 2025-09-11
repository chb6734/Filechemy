namespace FileChemy
{
	public sealed class QualityOptimizer
	{
		public int SelectImageQuality(DocumentMetadata metadata)
		{
			if (metadata.PageCount > 50) return 80;
			return 90;
		}

		public int SelectPdfQuality(DocumentMetadata metadata)
		{
			if (metadata.PageCount > 50) return 90;
			return 100;
		}
	}
}


