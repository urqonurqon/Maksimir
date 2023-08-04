
public static class TextFormating {


	public static string ReplaceHTMLTags(this string text)
	{
		text = text.Replace("<p>", null);
		text = text.Replace("</p>", null);
		text = text.Replace("&nbsp;", " ");
		text = text.Replace("<strong>", "<b>");
		text = text.Replace("</strong>", "</b>");
		text = text.Replace("<em>", "<i>");
		text = text.Replace("</em>", "</i>");
		text = text.Replace("&ldquo;", "\"");
		text = text.Replace("&scaron;", "š");
		text = text.Replace("&ndash;", "–");
		text = text.Replace("&rdquo;", "\"");

		return text;
	}
}
