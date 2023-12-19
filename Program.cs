using System.Diagnostics;
using System.IO.Pipes;

class ConsoleApp
{
	private const string PipeRegistryFileName = "NamedPipeRegistry.txt";

	static void Main(string[] args) {
		string pipeName = @"\\.\pipe\NvimPipe.1111.0";

		if (IsPipeRegistered(pipeName)) {
			OpenFileNvimPipe(pipeName, args);
		}
		else {
			Console.WriteLine($"Pipe {pipeName} does not exist, creating pipe");
			CreateNamedPipe(pipeName, args);
		}
	}

	static string GetPipeRegistryFilePath() {
		return Path.Combine(Directory.GetCurrentDirectory(), PipeRegistryFileName);
	}

	static void CreatePipeRegistryFileIfNotExists (string registryFilePath) {
		if (!File.Exists(registryFilePath)) {
			try {
				File.WriteAllText(registryFilePath, string.Empty);
			}
			catch (Exception ex) {
				Console.WriteLine($"Error creating registry file: {ex.Message}");
			}
		}
	}

	static bool IsPipeRegistered(string pipeName) {
		string registryFilePath = GetPipeRegistryFilePath();

		CreatePipeRegistryFileIfNotExists(registryFilePath);

		string[] lines = File.ReadAllLines(registryFilePath);
		foreach (string line in lines) {
			if (line.Equals(pipeName, StringComparison.OrdinalIgnoreCase)) {
				return true;
			}
		}

		return false;
	}

	static void RegisterPipe(string pipeName) {
		string registryFilePath = GetPipeRegistryFilePath();

		CreatePipeRegistryFileIfNotExists(registryFilePath);

		try {
			File.AppendAllText(registryFilePath, $"{pipeName}{Environment.NewLine}");
		}
		catch (Exception ex) {
			Console.WriteLine($"Error creating registry file: {ex.Message}");
		}
	}

	static void DeregisterPipe(string pipeName) {
		string registryFilePath = GetPipeRegistryFilePath();
		CreatePipeRegistryFileIfNotExists(registryFilePath);

		try {
			string[] lines = File.ReadAllLines(registryFilePath);
			lines = lines.Where(line => !line.Equals(pipeName, StringComparison.OrdinalIgnoreCase)).ToArray();
			File.WriteAllLines(registryFilePath, lines);
		}
		catch (Exception ex) {
			Console.WriteLine($"Error creating registry file: {ex.Message}");
		}
	}

	static void CreateNamedPipe(string pipeName, string[] arguments) {
		using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut)) {
			Console.WriteLine($"Named pipe '{pipeName}' created.");

			RegisterPipe(pipeName);
			Console.WriteLine($"Named pipe '{pipeName}' registered.");

			using(Process nvimProcess = LaunchNvimServer(pipeName)) {
				Console.WriteLine($"Launched nvim server");
				OpenFileNvimPipe(pipeName, arguments);
				Console.WriteLine($"Opened initial file in nvim");
				nvimProcess.WaitForExit();
			}
		}

		Console.WriteLine($"Neovim server closed.");
		DeregisterPipe(pipeName);
		Console.WriteLine($"Pipe {pipeName} deregistered and will be closed");
	}

	static Process LaunchNvimServer (string pipeName) {
		string nvimArgs = "--listen" + " " + pipeName;

		ProcessStartInfo psi = new ProcessStartInfo{
			FileName = "nvim.exe",
			Arguments = nvimArgs,
			CreateNoWindow = true,
			UseShellExecute = true
		};

		Process process = new Process {StartInfo = psi};
		process.Start();
		return process;
	}

	static void OpenFileNvimPipe (string pipeName, string[] arguments) {
		string remoteSendCLArgument = @"/C" + " " + "nvim" + " " + "--server" + " " + pipeName + " " + "--remote-send";

		string nvimCommand = string.Join(" ", arguments);
		nvimCommand = remoteSendCLArgument + " " + "\":e" + nvimCommand + " " + "<CR>\"";

		ProcessStartInfo psi = new ProcessStartInfo{
			FileName = "cmd.exe",
			Arguments = nvimCommand,
			CreateNoWindow = true,
			UseShellExecute = true
		};

		using (Process process = new Process {StartInfo = psi}) {
			process.Start();
			process.Close();
		}
	}
}
