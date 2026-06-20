using Plane_stress_analyzer_PSL.src.events_handler;
using Plane_stress_analyzer_PSL.src.model_store;
using src.solver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plane_stress_analyzer_PSL.other_windows
{
    public partial class solver_frm : Form
    {

        private modeldata_store modeldata;

        int refined_nodecount = 0;
        int refined_edgecount = 0;
        int refined_tricount = 0;
        int refined_quadcount = 0;

        public solver_frm(ref modeldata_store modeldata)
        {
            InitializeComponent();

            this.modeldata = modeldata;
        }

        private void solver_frm_Load(object sender, EventArgs e)
        {
            comboBox_solvertype.SelectedIndex = Properties.Settings.Default.Sett_solver_type;
            comboBox_HRefinement.SelectedIndex = Properties.Settings.Default.Sett_Hrefine;
            comboBox_polynomialrefinement.SelectedIndex = Properties.Settings.Default.Sett_Prefine;
            comboBox_formulation.SelectedIndex = 0;

        }

        private async void button_solve_Click(object sender, EventArgs e)
        {

            try
            {
                // Check the inputs (Whether the boundary condition is applied or not)
                if (modeldata.fe_data.fe_constraints.cnst_set_count == 0 ||
                modeldata.fe_data.fe_loads.load_set_count == 0)
                {

                    richTextBox_AnalysisUpdate.Clear();
                    AppendStatus("No boundary conditions applied or loads applied...\n");

                    return;
                }

                calculate_solver_model_size();

                // Calculate the total size of the refined model
                AppendStatus($"Total number of nodes = {this.refined_nodecount} \n");
                AppendStatus($"Total triangle element count = {this.refined_tricount} \n");
                AppendStatus($"Total quadrilateral element count = {this.refined_quadcount} \n");

                // Continue Analysis
                if (this.refined_nodecount > 200000)
                {
                    string message = $"Refined Model Size:\n" +
                                     $"• Nodes: {this.refined_nodecount:N0}\n" +
                                     $"• Triangles: {this.refined_tricount:N0}\n" +
                                     $"• Quads: {this.refined_quadcount:N0}\n" +
                                     $"• Edges: {this.refined_edgecount:N0}\n\n" +
                                     $"This exceeds the recommended limit of 200,000 nodes.\n" +
                                     $"The analysis may be slow or run out of memory.\n\n" +
                                     $"Continue with analysis?";

                    DialogResult result = MessageBox.Show(
                        message,
                        "Large Model Warning",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (result == DialogResult.No)
                        return;
                }



                AppendStatus("\nChecking dependencies...\n");

                if (!CheckAllDependencies())
                {
                    AppendStatus("\n❌ Missing dependencies detected. Please copy all required DLLs to the application directory.\n");
                    MessageBox.Show("Missing required DLL dependencies.\n\nPlease ensure all solver DLLs are copied to the application directory.",
                                  "DLL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                AppendStatus("Starting solver initialization...\n");

                // Step 1: Get diagnostic info (optional, good for debugging)
                string diagnosticInfo = stress2DSolverInterop.GetDiagnosticInfo();
                AppendStatus(diagnosticInfo);

                // Step 2: Initialize the DLL
                AppendStatus("\nInitializing solver DLL...\n");
                if (!stress2DSolverInterop.Initialize())
                {
                    AppendStatus($"✗ DLL initialization failed: {stress2DSolverInterop.LastError}\n");
                    MessageBox.Show($"Failed to initialize solver DLL:\n\n{stress2DSolverInterop.LastError}",
                                  "DLL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                AppendStatus("✓ DLL loaded successfully!\n");

                // Step 3: Run the solver
                AppendStatus("\nStarting stress analysis...\n");
                await RunSolverAsync();
            }
            catch (Exception ex)
            {
                AppendStatus($"\n❌ Unexpected error: {ex.Message}\n");
                AppendStatus($"Stack trace: {ex.StackTrace}\n");
                MessageBox.Show($"An unexpected error occurred:\n\n{ex.Message}",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable the button
                button_solve.Enabled = true;
            }

        }



        private async Task RunSolverAsync()
        {
            try
            {
                // Step 1: Check if DLL is available
                if (!stress2DSolverInterop.CheckDllExists())
                {
                    richTextBox_AnalysisUpdate.AppendText($"ERROR: {stress2DSolverInterop.LastError}\n");
                    richTextBox_AnalysisUpdate.AppendText(stress2DSolverInterop.GetDiagnosticInfo());
                    return;
                }

                // Step 2: Initialize the DLL
                if (!stress2DSolverInterop.Initialize())
                {
                    richTextBox_AnalysisUpdate.AppendText($"Failed to initialize DLL: {stress2DSolverInterop.LastError}\n");
                    richTextBox_AnalysisUpdate.AppendText(stress2DSolverInterop.GetDiagnosticInfo());
                    return;
                }

                richTextBox_AnalysisUpdate.AppendText("DLL loaded successfully!\n");

                // Step 3: Validate input files
                string inputPath = Path.Combine(Application.StartupPath, "stress2d_analysis_input.bin");
                string outputPath = Path.Combine(Application.StartupPath, "stress2d_analysis_output.bin");

                // Delete existing input file if it exists
                if (File.Exists(inputPath))
                {
                    try
                    {
                        File.Delete(inputPath);
                        richTextBox_AnalysisUpdate.AppendText("Deleted existing input file.\n");
                    }
                    catch (IOException ex)
                    {
                        richTextBox_AnalysisUpdate.AppendText($"Warning: Could not delete existing input file: {ex.Message}\n");
                        // Try to force garbage collection to release any locks
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        File.Delete(inputPath);
                    }
                }

                // Delete existing output file if it exists
                if (File.Exists(outputPath))
                {
                    try
                    {
                        File.Delete(outputPath);
                        richTextBox_AnalysisUpdate.AppendText("Deleted existing output file.\n");
                    }
                    catch (IOException ex)
                    {
                        richTextBox_AnalysisUpdate.AppendText($"Warning: Could not delete existing output file: {ex.Message}\n");
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        File.Delete(outputPath);
                    }
                }



                // C# GUI exports model to a .bin file.
                // C# calls your C++ DLL (using P/Invoke).
                // C++ DLL reads the.bin file, performs the simulation, and writes results to another .bin.
                // C# re-imports and displays results.

                // Write the binary file
                file_events.export_binary_mesh(inputPath, modeldata.fe_data);

        
                // Write input file
                double[] solver_settings = new double[4];
                solver_settings[0] = comboBox_solvertype.SelectedIndex; // 0 = Elimination method, 1 = Lagrange method
                solver_settings[1] = comboBox_HRefinement.SelectedIndex; // 0, 1, 2
                solver_settings[2] = comboBox_polynomialrefinement.SelectedIndex; // 0, 1, 2, 3
                solver_settings[3] = comboBox_formulation.SelectedIndex; // 0, 1


                // Step 5: Safely call the solver
                var result = await Task.Run(() => stress2DSolverInterop.SolveSafely(
                    inputPath,
                    outputPath,
                    solver_settings,
                    OnStatusUpdate
                ));

                if (result.Success)
                {
                    richTextBox_AnalysisUpdate.AppendText("Solver completed successfully!\n");

                    // Process results...
                    process_results(outputPath);

                }
                else
                {
                    richTextBox_AnalysisUpdate.AppendText($"Solver failed: {result.ErrorMessage}\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox_AnalysisUpdate.AppendText($"Unexpected error: {ex.Message}\n");
                richTextBox_AnalysisUpdate.AppendText(stress2DSolverInterop.GetDiagnosticInfo());
            }
            finally
            {
                // Clean up
                stress2DSolverInterop.Unload();
            }
        }



        private void process_results(string outputPath)
        {
            // Read the binary result file
            if (!File.Exists(outputPath))
            {
                AppendStatus("Result file not found: " + outputPath + "\n");
                return;
            }

            try
            {
                using (var fs = new FileStream(outputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var reader = new BinaryReader(fs))
                    {
                        //    // Read header
                        //    var header = ReadStruct<BinaryFileHeader>(reader);

                        //    if (System.Text.Encoding.ASCII.GetString(header.Magic) != "SEMF")
                        //        throw new InvalidDataException("Invalid file format");

                        //    AppendStatus($"File version: {header.Version}");
                        //    AppendStatus($"Number of modes: {header.NumModes}");
                        //    AppendStatus($"Number of nodes: {header.NumNodes}");
                        //    AppendStatus($"Number of edges: {header.NumEdges}");
                        //    AppendStatus($"Number of triangles: {header.NumTriangles}");
                        //    AppendStatus($"Reading results started...\n");

                        //    fe_data.modalresultmeshdata = new modal_rsltdata_store();

                        //    AppendStatus($"File position : {fs.Position}\n");

                        //    // Read nodes

                        //    for (int i = 0; i < header.NumNodes; i++)
                        //    {
                        //        int node_id = reader.ReadInt32();
                        //        double node_xcoord = reader.ReadDouble();
                        //        double node_ycoord = reader.ReadDouble();

                        //        fe_data.modalresultmeshdata.modal_rslt_nodes.Add(node_id,
                        //            new modal_rsltnode_store
                        //            {
                        //                node_id = node_id,
                        //                node_pt_x_coord = node_xcoord,
                        //                node_pt_y_coord = node_ycoord
                        //            });

                        //    }

                        //    AppendStatus($"Reading results for {header.NumNodes} nodes complete \n");


                        //    // Read edges
                        //    for (int i = 0; i < header.NumEdges; i++)
                        //    {
                        //        int start_nodeid = reader.ReadInt32();
                        //        int end_nodeid = reader.ReadInt32();

                        //        fe_data.modalresultmeshdata.rslt_edges.Add(new rsltedge_store
                        //        {
                        //            startnode = start_nodeid,
                        //            endnode = end_nodeid
                        //        });
                        //    }

                        //    AppendStatus($"Reading results for {header.NumEdges} edges complete \n");


                        //    // Read triangles
                        //    for (int i = 0; i < header.NumTriangles; i++)
                        //    {
                        //        int n1 = reader.ReadInt32();
                        //        int n2 = reader.ReadInt32();
                        //        int n3 = reader.ReadInt32();

                        //        fe_data.modalresultmeshdata.rslt_tris.Add(new rslttri_store
                        //        {
                        //            tri_node1 = n1,
                        //            tri_node2 = n2,
                        //            tri_node3 = n3,
                        //        });
                        //    }

                        //    AppendStatus($"Reading results for {header.NumTriangles} triangles complete \n");

                        //    AppendStatus($"File position : {fs.Position}\n");

                        //    // Read mode index table
                        //    AppendStatus($"\nReading mode index table...\n");
                        //    fe_data.modalresultmeshdata.modes.Clear();
                        //    fe_data.modalresultmeshdata.natural_Frequencies.Clear();

                        //    // Set the Result mesh
                        //    // Read mode index table
                        //    for (int i = 0; i < header.NumModes; i++)
                        //    {
                        //        long posBeforeRead = fs.Position;

                        //        // Read manually to avoid struct alignment issues
                        //        uint modeId = reader.ReadUInt32();
                        //        double frequency = reader.ReadDouble();
                        //        ulong fileOffset = reader.ReadUInt64();
                        //        ulong dataSize = reader.ReadUInt64();

                        //        fe_data.modalresultmeshdata.natural_Frequencies.Add(frequency);

                        //        AppendStatus($"  Mode {modeId}: f={frequency:F3} Hz, offset={fileOffset}, size={dataSize}\n");

                        //        fe_data.modalresultmeshdata.modes.Add(new modeInfo
                        //        {
                        //            Id = (int)modeId,
                        //            Frequency = frequency,
                        //            FileOffset = (long)fileOffset,
                        //            DataSize = (long)dataSize
                        //        });
                        //    }

                        //    AppendStatus($"  File position at the end of mode index table: {fs.Position}\n");



                        //    fe_data.modalresultmeshdata.setResultMesh();
                        //    fe_data.modalresultmeshdata.isModalResultSet = true;
                        //    fe_data.modalresultmeshdata.updateSelectedMode(0);
                        //    fe_data.modalresultmeshdata.start_animation();

                        //    fe_data.update_openTK_uniforms(true, true, true);
                        //}

                        //// Call the main form
                        //if (this.Owner is main_frm mainForm)
                        //{
                        //    mainForm.set_ResultOption(5); // Set the result option = 5, Paint modal results
                        //}

                        AppendStatus("Results read complete!\n");
                        MessageBox.Show("Solve completed successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                AppendStatus("Error reading binary results: " + ex.Message + "\n");
                MessageBox.Show("Error reading results file:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //________________________
        }






        private bool CheckAllDependencies()
        {
            string targetDir = AppDomain.CurrentDomain.BaseDirectory;

        //    string[] requiredDlls = new string[]
        //    {
        //"modalspectral_solverCPP.dll",
        //"libarpack.dll",
        //"libgcc_s_seh-1.dll",
        //"libgfortran-5.dll",
        //"liblapack.dll",
        //"libquadmath-0.dll",
        //"libwinpthread-1.dll",
        //"openblas.dll"
        //    };


            bool allExist = true;
            string fullPath = "";
            bool exists = false;

            //foreach (string dll in requiredDlls)
            //{
            //    fullPath = Path.Combine(targetDir, dll);
            //    exists = File.Exists(fullPath);
            //    AppendStatus($"  {(exists ? "✓" : "✗")} {dll}: {(exists ? "Found" : "MISSING")}\n");
            //    if (!exists) allExist = false;
            //}

            string requiredDLL = "stress2D_solverCPP.dll";
            fullPath = Path.Combine(targetDir, requiredDLL);
            exists = File.Exists(fullPath);
            AppendStatus($"  {(exists ? "✓" : "✗")} {requiredDLL}: {(exists ? "Found" : "MISSING")}\n");
            if (!exists) allExist = false;


            return allExist;
        }



        private void calculate_solver_model_size()
        {
            int num_of_nodecount = modeldata.fe_data.fe_nodes.node_count;
            int num_of_edges = modeldata.fe_data.number_of_edges;
            int num_of_trielements = modeldata.fe_data.fe_tris.elementtri_count;
            int num_of_quadelements = modeldata.fe_data.fe_quads.elementquad_count;

            // Get refinement levels (0-based indexing)
            int h_refinement = comboBox_HRefinement.SelectedIndex; // 0, 1, 2
            int p_refinement = comboBox_polynomialrefinement.SelectedIndex; // 0, 1, 2, 3

            // Initialize with original mesh
            int h_refined_nodecount = num_of_nodecount;
            int h_refined_edgecount = num_of_edges;
            int h_refined_tricount = num_of_trielements;
            int h_refined_quadcount = num_of_quadelements;

            // === H-REFINEMENT ===
            if (h_refinement == 0) // Original mesh
            {
                // No change
            }
            else if (h_refinement == 1) // Split into 4 (h=2)
            {
                // Elements multiply by 4
                h_refined_tricount = num_of_trielements * 4;
                h_refined_quadcount = num_of_quadelements * 4;

                // Edges: Each original edge gets split into 2
                h_refined_edgecount = num_of_edges * 2;

                // Nodes: Original nodes + edge nodes + quad center nodes
                int edgeNodes = num_of_edges * 1;        // 1 new node per edge
                int quadCenterNodes = num_of_quadelements * 1; // 1 new node per quad
                                                               // Note: Triangles don't add center nodes for h=2 refinement
                h_refined_nodecount = num_of_nodecount + edgeNodes + quadCenterNodes;
            }
            else if (h_refinement == 2) // Split into 16 (h=4)
            {
                // Elements multiply by 16
                h_refined_tricount = num_of_trielements * 16;
                h_refined_quadcount = num_of_quadelements * 16;

                // Edges: Each original edge gets split into 4
                h_refined_edgecount = num_of_edges * 4;

                // Nodes: Original nodes + edge nodes + triangle internal nodes + quad internal nodes
                int edgeNodes = num_of_edges * 3;         // 3 new nodes per original edge
                int triInternalNodes = num_of_trielements * 3;  // 3 internal nodes per triangle
                int quadInternalNodes = num_of_quadelements * 9; // 9 internal nodes per quad
                h_refined_nodecount = num_of_nodecount + edgeNodes + triInternalNodes + quadInternalNodes;
            }

            // === P-REFINEMENT ===
            int p_refined_nodecount = h_refined_nodecount;
            int p_refined_edgecount = h_refined_edgecount;
            int p_refined_tricount = h_refined_tricount;
            int p_refined_quadcount = h_refined_quadcount;

            if (p_refinement == 0) // p=1 (Linear/Bilinear)
            {
                // No change - T3 + Q4
            }
            else if (p_refinement == 1) // p=2 (Quadratic)
            {
                // T6: Adds 1 node per edge
                // Q9: Adds 1 node per edge + 1 center node
                p_refined_nodecount = h_refined_nodecount
                                    + h_refined_edgecount          // Edge nodes
                                    + h_refined_quadcount;        // Quad center nodes
            }
            else if (p_refinement == 2) // p=3 (Cubic)
            {
                // T10: Adds 2 nodes per edge + 1 internal node
                // Q16: Adds 2 nodes per edge + 4 internal nodes
                p_refined_nodecount = h_refined_nodecount
                                    + (h_refined_edgecount * 2)    // Edge nodes (2 per edge)
                                    + h_refined_tricount          // Triangle internal nodes (1 per tri)
                                    + (h_refined_quadcount * 4);  // Quad internal nodes (4 per quad)
            }
            else if (p_refinement == 3) // p=4 (Quartic)
            {
                // T15: Adds 3 nodes per edge + 3 internal nodes
                // Q25: Adds 3 nodes per edge + 9 internal nodes
                p_refined_nodecount = h_refined_nodecount
                                    + (h_refined_edgecount * 3)    // Edge nodes (3 per edge)
                                    + (h_refined_tricount * 3)    // Triangle internal nodes (3 per tri)
                                    + (h_refined_quadcount * 9);  // Quad internal nodes (9 per quad)
            }

            // Store results
            this.refined_nodecount = p_refined_nodecount;
            this.refined_edgecount = p_refined_edgecount;
            this.refined_tricount = p_refined_tricount;
            this.refined_quadcount = p_refined_quadcount;
        }


        private void save_defaults()
        {
            Properties.Settings.Default.Sett_solver_type = comboBox_solvertype.SelectedIndex;
            Properties.Settings.Default.Sett_Hrefine = comboBox_HRefinement.SelectedIndex;
            Properties.Settings.Default.Sett_Prefine = comboBox_polynomialrefinement.SelectedIndex;

            Properties.Settings.Default.Save();
        }


        private void OnStatusUpdate(string message)
        {
            // Marshal back to UI thread safely
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => AppendStatus(message + "\n")));
            }
            else
            {
                AppendStatus(message + "\n");
            }
        }

        private void AppendStatus(string text)
        {
            richTextBox_AnalysisUpdate.AppendText(text);
            richTextBox_AnalysisUpdate.ScrollToCaret();
        }



    }
}
