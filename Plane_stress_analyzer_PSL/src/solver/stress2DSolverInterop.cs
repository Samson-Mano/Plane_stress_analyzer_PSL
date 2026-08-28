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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct SolverSettings
        {
            public int SolverType;           // 0 = Elimination, 1 = Lagrange
            public int HRefinement;          // 0, 1, 2
            public int PRefinement;          // 0, 1, 2, 3
            public int Formulation;          // 0, 1
            public double ExtendConstraints; // 0.0 or 1.0
            public double ExtendLoads;       // 0.0 or 1.0
            public double SaveHRefinedModel; // 0.0 or 1.0
            public double SelfWeight;        // 0.0 or 1.0
            public double XAcceleration;     // Self-weight X
            public double YAcceleration;     // Self-weight Y
            public double modelorientationangle;     // 0.0 to 360.0
        }


        // Declare the callback delegate
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CallbackDelegate([MarshalAs(UnmanagedType.LPStr)] string message);



        // Import the DLL function (updated to accept callback)
        [DllImport("stress2D_solverCPP.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void solve_2DstressanalysisCPP(
            [MarshalAs(UnmanagedType.LPStr)] string inputPath,
            [MarshalAs(UnmanagedType.LPStr)] string outputPath,
            ref SolverSettings settings,
            ref bool isAnalysisSuccess,
            CallbackDelegate callback
        );


    }
}
