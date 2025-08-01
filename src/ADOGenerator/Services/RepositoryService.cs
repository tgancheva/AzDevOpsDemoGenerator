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

                // Step 2: Copy files to the local repository folder
                bool copyResult = CopyToLocalRepositoryFolderAsync(localSource);

                if (!copyResult)
                {
                    Console.WriteLine("Failed to copy local repository to the target path.");
                    return false;
                }

                // Step 3: Push local repository to Azure DevOps
                return await PushLocalRepositoryAsync(localSource, cloneUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing repository: {ex.Message}");
                return false;
            }
        }

        private bool CopyToLocalRepositoryFolderAsync(ImportSourceCodeLocalSource localSource)
        {
            try
            {
                // Ensure source and target directories exist
                if (!Directory.Exists(localSource.SourcePath))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return false;
                }

                // Clear the target directory and its contents if it exists
                if (Directory.Exists(localSource.TargetPath))
                {
                    Directory.Delete(localSource.TargetPath, true);
                }

                // Create (re-create) the target directory
                Directory.CreateDirectory(localSource.TargetPath);

                // Copy files from source to target directory
                CopyDirectory(localSource.SourcePath, localSource.TargetPath);

                Console.WriteLine("Files copied successfully to local repository folder");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying local repository to the target path: {ex.Message}");
                return false;
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Get the subdirectories for the specified directory
            var dir = new DirectoryInfo(sourceDir);

            // Create the destination directory if it doesn't exist
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Get the files in the directory and copy them to the new location
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                try
                {
                    var tempPath = Path.Combine(destinationDir, file.Name);
                    file.CopyTo(tempPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to copy file {file.Name}: {ex.Message}");
                }
            }

            // Copy subdirectories and their contents to new location
            var subDirs = dir.GetDirectories();
            foreach (var subdir in subDirs)
            {
                try
                {
                    var tempPath = Path.Combine(destinationDir, subdir.Name);
                    CopyDirectory(subdir.FullName, tempPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to copy directory {subdir.Name}: {ex.Message}");
                }
            }
        }

        private async Task<bool> PushLocalRepositoryAsync(ImportSourceCodeLocalSource localSource, string remoteUrl)
        {
            try
            {
                // 1. Ensure we're in the local repository directory
                if (!Directory.Exists(Path.Combine(localSource.TargetPath, ".git")))
                {
                    Console.WriteLine("Local path is not a Git repository. Initializing...");
                    await RunGitCommandAsync(localSource.TargetPath, "init --initial-branch=dev");
                }

                // 2. Create and switch to a new dev branch
                await RunGitCommandAsync(localSource.TargetPath, $"checkout -b dev");

                // 3. Add all files and commit if there are uncommitted changes
                await RunGitCommandAsync(localSource.TargetPath, "add .");

                // 4. Check if there are changes to commit
                var statusResult = await RunGitCommandAsync(localSource.TargetPath, "status --porcelain");
                if (!string.IsNullOrWhiteSpace(statusResult))
                {
                    await RunGitCommandAsync(localSource.TargetPath, "commit -m \"Initial import from local repository\"");
                }

                // 5. Add remote origin
                var remoteUrlWithToken = remoteUrl.Replace("https://", $"https://:{_personalAccessToken}@");
                await RunGitCommandAsync(localSource.TargetPath, $"remote add origin {remoteUrlWithToken}");

                // 6. Push dev branch to origin
                await RunGitCommandAsync(localSource.TargetPath, "push -u origin dev");

                // 7. Create main branch from dev branch
                await RunGitCommandAsync(localSource.TargetPath, $"checkout -b main");

                // 8. Merge dev branch into main
                await RunGitCommandAsync(localSource.TargetPath, $"merge dev");

                // 9. Push main branch to remote
                await RunGitCommandAsync(localSource.TargetPath, "push -u origin main");

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
