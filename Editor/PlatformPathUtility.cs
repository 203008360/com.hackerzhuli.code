using System.IO;
namespace Hackerzhuli.Code.Editor
{
    /// <summary>
    ///     Static utility class for platform-specific path operations.
    /// </summary>
    internal static class PlatformPathUtility
    {
        /// <summary>
        ///     Gets the real path by resolving symbolic links or shortcuts.
        /// </summary>
        /// <param name="path">The path that might be a symbolic link.</param>
        /// <returns>The resolved path if it's a symbolic link; otherwise, the original path.</returns>
        public static string GetRealPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;

#if UNITY_EDITOR_WIN
            return path;
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            // On Unix-like systems, resolve symbolic links
            return ResolveSymlink(path);
#else
            return path;
#endif
        }

        private static string ResolveSymlink(string path)
        {
            try
            {
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "readlink",
                        Arguments = $"\"{path}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    return output.Trim();
                }
            }
            catch
            {
                // If we can't resolve, return original path
            }
            return path;
        }
    }
}