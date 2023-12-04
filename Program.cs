using System.Diagnostics;
using System.IO.Pipes;

class ConsoleApp
{
	static void Main(string[] args) {
		string pipeName = @"\\.\pipe\NvimPipe.1111.0";

		if (IsPipeOpen(pipeName)) {
			Console.WriteLine($"Pipe {pipeName} already exists");
			OpenFileNvimPipe(pipeName, args);
		}
		else {
			Console.WriteLine($"Pipe {pipeName} does not exist, creating pipe");
			CreateNamedPipe(pipeName);
		}
	}

	static bool IsPipeOpen(string pipeName) {
		try {
			using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
			{
				pipeClient.Connect(1000);
				return pipeClient.IsConnected;
			}
		}
		catch (TimeoutException) {
			return false;
		}
		catch (Exception ex) {
			Console.WriteLine($"Error checking pipe status: {ex.Message}");
			return false;
		}
	}

	static void CreateNamedPipe(string pipeName) {
		using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut)) {
			Console.WriteLine($"Named pipe '{pipeName}' created.");
			LaunchNvim(pipeName);
		}
	}

	static void LaunchNvim (string pipeName) {
		string nvimArgs = "--listen" + " " + pipeName;
		Console.WriteLine($"Args are: {nvimArgs}");

		ProcessStartInfo psi = new ProcessStartInfo{
			FileName = "nvim.exe",
			Arguments = nvimArgs,
			CreateNoWindow = false,
			UseShellExecute = true
		};

		using (Process process = new Process {StartInfo = psi}) {
			process.Start();
			process.WaitForExit();
		}
	}

	static void OpenFileNvimPipe (string pipeName, string[] arguments) {
		string nvimCommand = string.Join(" ", arguments);

		foreach (string arg in arguments)
		{
			Console.WriteLine(arg);
		}

		Console.WriteLine($"Args: {nvimCommand}");
		nvimCommand = @"/K" + " " + "nvim" + " " + "--server" + " " + pipeName + " " + "--remote" + " " + nvimCommand;

		ProcessStartInfo psi = new ProcessStartInfo{
			FileName = "cmd.exe",
			Arguments = nvimCommand,
			// RedirectStandardInput = true,
			// RedirectStandardOutput = true,
			// RedirectStandardError = true,
			UseShellExecute = true,
			CreateNoWindow = false
		};

		using (Process process = new Process {StartInfo = psi}) {
			process.Start();
		}
	}
}

// string[] nvimCommandLineArgs = Environment.GetCommandLineArgs();
// string nvimCommandLineArgsConcat = string.Join(" ", nvimCommandLineArgs, 1, nvimCommandLineArgs.Length-1);
// nvimCommandLineArgsConcat = "--remote" + " "  + nvimCommandLineArgsConcat;
//
// ProcessStartInfo psi = new ProcessStartInfo{
// 	FileName = "nvim.exe", 
// 	Arguments = nvimCommandLineArgsConcat
// };
//
// Process process = new Process {StartInfo = psi};
// process.Start();
// process.WaitForExit();
// process.Close();
