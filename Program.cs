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
			CreateNamedPipe(pipeName);
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

	static void CreateNamedPipe(string pipeName) {
		using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut)) {
			Console.WriteLine($"Named pipe '{pipeName}' created.");
			RegisterPipe(pipeName);
			Console.WriteLine($"Named pipe '{pipeName}' registered.");
			LaunchNvim(pipeName);
			Console.WriteLine($"Launched nvim");
		}
		DeregisterPipe(pipeName);
		Console.WriteLine($"Neovim Instance closed, pipe closed and deregistered");
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
			UseShellExecute = true,
			CreateNoWindow = false
		};

		using (Process process = new Process {StartInfo = psi}) {
			process.Start();
			process.Close();
		}
	}
}
