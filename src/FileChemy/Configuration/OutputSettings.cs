namespace FileChemy
{
	public sealed class OutputSettings
	{
		public string OutputDirectory { get; set; } = ".";
		public string FileNamePattern { get; set; } = "{name}_{index}";
	}
}


