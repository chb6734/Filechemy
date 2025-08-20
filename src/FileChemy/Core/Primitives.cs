using System.Collections.Generic;
using System.IO;

namespace FileChemy
{
	public enum DocumentType
	{
		Excel,
		Word
	}

	public enum OutputFormat
	{
		Png,
		Jpeg,
		Bitmap,
		Pdf
	}

	public sealed class DocumentContent
	{
		public string FilePath { get; set; }
		public byte[] Data { get; set; }
		public DocumentType Type { get; set; }
	}

	public sealed class ImageOptions
	{
		public OutputFormat ImageFormat { get; set; }
		public int Quality { get; set; }
		public int Dpi { get; set; }
		
		public ImageOptions()
		{
			ImageFormat = OutputFormat.Png;
			Quality = 90;
			Dpi = 150;
		}
	}

	public sealed class PdfOptions
	{
		public int Quality { get; set; }
		public bool EmbedFonts { get; set; }
		
		public PdfOptions()
		{
			Quality = 100;
			EmbedFonts = true;
		}
	}

	public sealed class ConversionResult
	{
		public bool Succeeded { get; set; }
		public IList<byte[]> Outputs { get; set; }
		public string ErrorMessage { get; set; }
	}

	public sealed class ConversionRequest
	{
		public Stream Input { get; set; }
		public DocumentType DocumentType { get; set; }
		public OutputFormat OutputFormat { get; set; }
		public ConversionOptions Options { get; set; }
	}

	public sealed class BatchConversionResult
	{
		public IList<ConversionResult> Results { get; set; }
	}

	public sealed class DocumentMetadata
	{
		public string Title { get; set; }
		public string Author { get; set; }
		public int PageCount { get; set; }
	}
}


