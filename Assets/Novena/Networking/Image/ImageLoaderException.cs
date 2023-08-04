using System;
public class ImageLoaderException : Exception {
	public new string Message { get; set; }
	public string PhotoPath { get; set; }
	public ImageLoaderException(string message, string path)
	{
		Message = message;
		PhotoPath = path;
	}
}