// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

Console.WriteLine("Running unity nvim loader");

// ProcessStartInfo psi = new ProcessStartInfo{
// 	FileName = "cmd.exe", 
// 	RedirectStandardInput = true, 
// 	RedirectStandardOutput = true,
// 	RedirectStandardError = true,
// 	UseShellExecute = false,
// 	CreateNoWindow = false,
// 	WindowStyle = ProcessWindowStyle.Normal
// };

ProcessStartInfo psi = new ProcessStartInfo{
	FileName = "nvim.exe", 
	// RedirectStandardInput = true, 
	// RedirectStandardOutput = true,
	// RedirectStandardError = true,
	// UseShellExecute = false,
	// CreateNoWindow = false,
	// WindowStyle = ProcessWindowStyle.Normal
};

Process process = new Process {StartInfo = psi};
process.Start();
process.WaitForExit();
process.Close();
