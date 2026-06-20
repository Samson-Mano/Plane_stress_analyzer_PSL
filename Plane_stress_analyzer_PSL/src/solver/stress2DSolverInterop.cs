using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace src.solver
{
    public static class stress2DSolverInterop
    {


        // Declare the callback delegate
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackDelegate([MarshalAs(UnmanagedType.LPStr)] string message);



        // Import the DLL function (updated to accept callback)
        [DllImport("stress2D_solverCPP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void solve_2DstressanalysisCPP(
            [MarshalAs(UnmanagedType.LPStr)] string inputPath,
            [MarshalAs(UnmanagedType.LPStr)] string outputPath,
            double[] solver_settings,
            int solver_settings_count,
            ref bool isAnalysisSuccess,
            CallbackDelegate callback
        );


        // Kernel32 functions for explicit DLL loading
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        // Track DLL state
        private static IntPtr _dllHandle = IntPtr.Zero;
        private static bool _isDllLoaded = false;
        private static string _lastError = null;

        /// <summary>
        /// Gets whether the DLL is currently loaded
        /// </summary>
        public static bool IsDllLoaded => _isDllLoaded;

        /// <summary>
        /// Gets the last error message from DLL operations
        /// </summary>
        public static string LastError => _lastError;

        /// <summary>
        /// Checks if the DLL exists and is accessible
        /// </summary>
        /// <param name="dllPath">Optional custom path to the DLL</param>
        /// <returns>True if DLL exists and is valid</returns>
        public static bool CheckDllExists(string dllPath = null)
        {
            try
            {
                // Determine DLL path
                string dllFullPath = dllPath ?? GetDllPath();

                if (!File.Exists(dllFullPath))
                {
                    _lastError = $"DLL not found at: {dllFullPath}";
                    return false;
                }

                // Check file size (basic validation)
                var fileInfo = new FileInfo(dllFullPath);
                if (fileInfo.Length == 0)
                {
                    _lastError = $"DLL file is empty: {dllFullPath}";
                    return false;
                }

                // Check if we can read the file
                try
                {
                    using (var fs = File.OpenRead(dllFullPath))
                    {
                        // Successfully opened file
                    }
                }
                catch (Exception ex)
                {
                    _lastError = $"Cannot read DLL file: {ex.Message}";
                    return false;
                }

                _lastError = null;
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Error checking DLL existence: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Attempts to load the DLL and validate its exports
        /// </summary>
        /// <param name="dllPath">Optional custom path to the DLL</param>
        /// <returns>True if DLL loaded successfully</returns>
        public static bool Initialize(string dllPath = null)
        {
            try
            {
                // If already loaded, try to unload first
                if (_isDllLoaded && _dllHandle != IntPtr.Zero)
                {
                    Unload();
                }

                // Get DLL path
                string dllFullPath = dllPath ?? GetDllPath();

                // Check if file exists
                if (!CheckDllExists(dllFullPath))
                {
                    return false;
                }

                // Try to load the DLL
                _dllHandle = LoadLibrary(dllFullPath);

                if (_dllHandle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    _lastError = GetErrorMessage(errorCode, dllFullPath);
                    return false;
                }

                // Verify the exported function exists
                IntPtr procAddress = GetProcAddress(_dllHandle, "solve_2DstressanalysisCPP");
                if (procAddress == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    _lastError = $"Exported function 'solve_2DstressanalysisCPP' not found in DLL. Error code: {errorCode}";
                    FreeLibrary(_dllHandle);
                    _dllHandle = IntPtr.Zero;
                    return false;
                }

                _isDllLoaded = true;
                _lastError = null;
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Exception during DLL initialization: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Unloads the DLL from memory
        /// </summary>
        public static void Unload()
        {
            if (_dllHandle != IntPtr.Zero)
            {
                FreeLibrary(_dllHandle);
                _dllHandle = IntPtr.Zero;
            }
            _isDllLoaded = false;
        }

        /// <summary>
        /// Validates all solver prerequisites before calling the solver
        /// </summary>
        /// <param name="inputPath">Path to input file</param>
        /// <param name="outputPath">Path to output file</param>
        /// <param name="solver_settings">Solver settings array</param>
        /// <param name="callback">Callback delegate</param>
        /// <returns>Validation result with error message if any</returns>
        public static (bool IsValid, string ErrorMessage) ValidatePrerequisites(
            string inputPath,
            string outputPath,
            double[] solver_settings,
            CallbackDelegate callback)
        {
            // Check DLL is initialized
            if (!_isDllLoaded && _dllHandle == IntPtr.Zero)
            {
                // Try to initialize
                if (!Initialize())
                {
                    return (false, $"DLL not initialized: {_lastError}");
                }
            }

            // Check input file exists
            if (string.IsNullOrEmpty(inputPath))
            {
                return (false, "Input path is null or empty");
            }

            if (!File.Exists(inputPath))
            {
                return (false, $"Input file not found: {inputPath}");
            }

            // Check output directory exists (create if needed)
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                try
                {
                    Directory.CreateDirectory(outputDir);
                }
                catch (Exception ex)
                {
                    return (false, $"Cannot create output directory: {ex.Message}");
                }
            }

            // Validate solver settings
            if (solver_settings == null || solver_settings.Length == 0)
            {
                return (false, "Solver settings array is null or empty");
            }

            // Validate callback
            if (callback == null)
            {
                return (false, "Callback delegate is null");
            }

            return (true, null);
        }

        /// <summary>
        /// Safely calls the solver with error handling
        /// </summary>
        public static (bool Success, string ErrorMessage) SolveSafely(
            string inputPath,
            string outputPath,
            double[] solver_settings,
            CallbackDelegate callback)
        {
            // Validate prerequisites
            var validation = ValidatePrerequisites(inputPath, outputPath, solver_settings, callback);
            if (!validation.IsValid)
            {
                return (false, validation.ErrorMessage);
            }

            try
            {
                bool isAnalysisSuccess = false;

                // Call the solver
                solve_2DstressanalysisCPP(
                    inputPath,
                    outputPath,
                    solver_settings,
                    solver_settings.Length,
                    ref isAnalysisSuccess,
                    callback
                );

                return (isAnalysisSuccess, isAnalysisSuccess ? null : "Solver returned failure status");
            }
            catch (DllNotFoundException ex)
            {
                return (false, $"DLL not found: {ex.Message}");
            }
            catch (EntryPointNotFoundException ex)
            {
                return (false, $"Entry point not found in DLL: {ex.Message}");
            }
            catch (BadImageFormatException ex)
            {
                return (false, $"Architecture mismatch (32/64-bit): {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error during solver call: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets the diagnostic information about the DLL and environment
        /// </summary>
        public static string GetDiagnosticInfo()
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine("=== DLL Diagnostic Information ===");

            // Architecture info
            info.AppendLine($"Process Architecture: {(Environment.Is64BitProcess ? "64-bit" : "32-bit")}");
            info.AppendLine($"OS Architecture: {(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")}");

            // DLL search paths
            info.AppendLine("\nDLL Search Paths:");
            info.AppendLine($"  Base Directory: {AppDomain.CurrentDomain.BaseDirectory}");
            info.AppendLine($"  Current Directory: {Environment.CurrentDirectory}");

            // DLL existence check
            string dllPath = GetDllPath();
            info.AppendLine($"\nDLL Path: {dllPath}");
            info.AppendLine($"DLL Exists: {File.Exists(dllPath)}");

            if (File.Exists(dllPath))
            {
                var fileInfo = new FileInfo(dllPath);
                info.AppendLine($"DLL Size: {fileInfo.Length} bytes");
                info.AppendLine($"DLL Last Modified: {fileInfo.LastWriteTime}");

                // Try to get file version
                try
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(dllPath);
                    info.AppendLine($"File Version: {versionInfo.FileVersion}");
                    info.AppendLine($"Product Version: {versionInfo.ProductVersion}");
                }
                catch { }
            }

            // // DLL load status
            // info.AppendLine($"\nDLL Load Status: {(_isDllLoaded ? "Loaded" : "Not loaded")}");
            if (!string.IsNullOrEmpty(_lastError))
            {
                info.AppendLine($"Last Error: {_lastError}");
            }

            return info.ToString();
        }

        /// <summary>
        /// Gets the default DLL path
        /// </summary>
        private static string GetDllPath()
        {
            // Try multiple possible locations
            string[] possiblePaths = new string[]
            {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "stress2D_solverCPP.dll"),
            Path.Combine(Environment.CurrentDirectory, "stress2D_solverCPP.dll"),
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "stress2D_solverCPP.dll")
            };

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            // Return the most likely location even if it doesn't exist
            return possiblePaths[0];
        }

        /// <summary>
        /// Gets a user-friendly error message for Windows error codes
        /// </summary>
        private static string GetErrorMessage(int errorCode, string dllPath)
        {
            string message = $"Failed to load DLL. Error code: {errorCode}. Path: {dllPath}\n";

            switch (errorCode)
            {
                case 126:
                    message += "Error 126: The specified module could not be found (missing dependency).";
                    message += "\nCheck if Visual C++ Redistributable is installed or if dependent DLLs are present.";
                    break;
                case 193:
                    message += "Error 193: %1 is not a valid Win32 application (architecture mismatch).";
                    message += $"\nYour application is {(Environment.Is64BitProcess ? "64-bit" : "32-bit")} but the DLL may be the opposite.";
                    break;
                case 2:
                    message += "Error 2: The system cannot find the file specified.";
                    break;
                case 1114:
                    message += "Error 1114: A DLL initialization routine failed.";
                    break;
                default:
                    message += $"Unknown error. Check Windows system error codes for more information.";
                    break;
            }

            return message;
        }





    }
}
