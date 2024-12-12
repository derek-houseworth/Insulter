using System.Reflection;
using System.Text;

namespace Insulter.Tests
{
	internal class TestHelper
	{
		private const string DEFAULT_LOG_FILE_NAME = "./TestHelperDebug.log";

		/// <summary>
		/// returns string containing app name and version parsed from FullName property of specified Assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static string GetAppVersion(Assembly assembly)
		{
			if (assembly is null || assembly.FullName is null) return "";
			string name = assembly.FullName[..assembly.FullName.IndexOf(',')];
			int vStart = assembly.FullName.IndexOf('=') + 1;
			int vEnd = assembly.FullName.IndexOf(',', vStart);
			string version = assembly.FullName.Substring(vStart, vEnd - vStart);
			return $"{name} {version}";

		} //GetAppVersion


		/// <summary>
		/// outputs text prefixed with local time to console and optionally to logfile with line ending character(s)
		/// </summary>
		/// <param name="outputText">string to output</param>
		/// <param name="writeToLogFile">flag indicating whether to write outputText to logfile</param>
		/// <param name="logFileName">name of log file</param>
		public static void DebugWriteLine(string outputText, bool writeToLogFile = false,
			string logFileName = DEFAULT_LOG_FILE_NAME)
		{

			DebugWrite(outputText + Environment.NewLine, writeToLogFile, logFileName);

		} //DebugWriteLine


		/// <summary>
		/// outputs text prefixed with local time to console and optionally to logfile without line ending character(s)
		/// </summary>
		/// <param name="outputText">string to output</param>
		/// <param name="writeToLogFile">flag indicating whether to write outputText to logfile</param>
		/// <param name="logFileName">name of log file</param>
		public static void DebugWrite(string outputText, bool writeToLogFile = false,
			string logFileName = DEFAULT_LOG_FILE_NAME)
		{
			string formattedTime = DateTime.Now.ToLocalTime().ToString("H:mm:ss");
			var output = new StringBuilder($"[{formattedTime}] {outputText}");
			Console.Write(output.ToString());
			if (writeToLogFile)
			{
				new Logger(logFileName).Write(output.ToString());
			}

		} //DebugWrite


		/// <summary>
		/// throws ArgumentException if value is outside range specified by min and max parameters
		/// </summary>
		/// <param name="propertyName">property name for exception message</param>
		/// <param name="value">value to test</param>
		/// <param name="min">minimum valid value</param>
		/// <param name="max">maximum valid value</param>
		/// <exception cref="ArgumentException"></exception>
		public static void ValidatePropertyValueInt(string propertyName, int value, int min, int max)
		{

			if (value < min || value > max)
			{
				throw new ArgumentException($"{propertyName} value must be in range {min} - {max}: {value}");
			}

		} //ValidatePropertyValueInt


		/// <summary>
		/// throws ArgumentException if value is outside range specified by min and max parameters
		/// </summary>
		/// <param name="propertyName">property name for exception message</param>
		/// <param name="value">value to test</param>
		/// <param name="min">minimum valid value</param>
		/// <param name="max">maximum valid value</param>
		/// <exception cref="ArgumentException"></exception>
		public static void ValidatePropertyValueDouble(string propertyName, double value, double min, double max)
		{

			if (value < min || value > max)
			{
				throw new ArgumentException($"{propertyName} value must be in range {min:N2} - {max:N2}");
			}

		} //ValidatePropertyValueDouble


		/// <summary>
		/// throws ArgumentException if value is outside range specified by min and max parameters
		/// </summary>
		public static void ValidatePropertyValueString(string propertyName, string value, string[] validValues)
		{
			if (!validValues.Contains(value))
			{
				throw new ArgumentException($"{propertyName} not in list of valid values: {string.Join(", ", validValues)}");
			}

		} //ValidatePropertyValueString

	}
}
