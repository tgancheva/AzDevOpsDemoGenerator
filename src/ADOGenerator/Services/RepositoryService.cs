using ADOGenerator.IServices;
using ADOGenerator.Models;
using System.Diagnostics;

namespace ADOGenerator.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly string _organization;
        private readonly string _project;
        private readonly string _personalAccessToken;

        public RepositoryService(string organization, string project, string personalAccessToken)
        {
            _organization = organization;
            _project = project;
            _personalAccessToken = personalAccessToken;
        }

        public async Task<bool> ImportLocalRepositoryAsync(ImportSourceCodeLocalSource localSource, string repositoryName)
        {
            try
            { 
                // Step 1: Get the clone URL
                var cloneUrl = $"https://{_organization}.visualstudio.com/{_project}/_git/{repositoryName}";

                // Step 2: Push local repository to Azure DevOps
                return await PushLocalRepositoryAsync(localSource, cloneUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing repository: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> PushLocalRepositoryAsync(ImportSourceCodeLocalSource localSource, string remoteUrl)
        {
            try
            {
                // Ensure we're in the local repository directory
                if (!Directory.Exists(Path.Combine(localSource.Path, ".git")))
                {
                    Console.WriteLine("Local path is not a Git repository. Initializing...");
                    await RunGitCommandAsync(localSource.Path, "init");
                }

                // Add remote origin
                var remoteUrlWithToken = remoteUrl.Replace("https://", $"https://:{_personalAccessToken}@");
                await RunGitCommandAsync(localSource.Path, $"remote add origin {remoteUrlWithToken}");

                // Add all files and commit if there are uncommitted changes
                await RunGitCommandAsync(localSource.Path, "add .");

                // Check if there are changes to commit
                var statusResult = await RunGitCommandAsync(localSource.Path, "status --porcelain");
                if (!string.IsNullOrWhiteSpace(statusResult))
                {
                    await RunGitCommandAsync(localSource.Path, "commit -m \"Initial import from local repository\"");
                }

                // Push branches to remote
                localSource.Branches.ForEach(async branch => await RunGitCommandAsync(localSource.Path, "push -u origin " + branch));

                Console.WriteLine("Local repository pushed successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pushing local repository: {ex.Message}");
                return false;
            }
        }

        private async Task<string> RunGitCommandAsync(string workingDirectory, string arguments)
        {
            try
            {
                var processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"git {arguments}\"",
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Explicitly set the PATH environment variable
                var currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine) + ";" +
                                  Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
                processInfo.EnvironmentVariables["PATH"] = currentPath;

                using var process = Process.Start(processInfo);
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"Git command failed: {error}");
                    return error;
                }

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running Git command '{arguments}': {ex.Message}");
                throw;
            }
        }
    }
}
