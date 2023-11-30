using System.Diagnostics;

string[] nvimCommandLineArgs = Environment.GetCommandLineArgs();
string nvimCommandLineArgsConcat = string.Join(" ", nvimCommandLineArgs, 1, nvimCommandLineArgs.Length-1);

ProcessStartInfo psi = new ProcessStartInfo{
	FileName = "nvim.exe", 
	Arguments = nvimCommandLineArgsConcat
};

Process process = new Process {StartInfo = psi};
process.Start();
process.WaitForExit();
process.Close();
