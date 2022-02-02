using System.Globalization;

namespace CS82ANGULAR.TemplateProcessingHelpers
{
	public class TPError
	{
		/// <summary>
		/// Error message.
		/// </summary>
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Line number within the T4 template.
		/// </summary>
		public int LineNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Column number within the line.
		/// </summary>
		public int ColumnNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Overriding base implementation.
		/// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Error: {0}  at line number {1} and column number {2}", Message, LineNumber, ColumnNumber);
		}
	}
}
