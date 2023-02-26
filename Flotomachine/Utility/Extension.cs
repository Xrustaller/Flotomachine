using System;

namespace Flotomachine.Utility;

public static class Extension
{
	public static string ToShortString(this Version version) => $"{version.Major}.{version.Minor}.{version.Build}";
}