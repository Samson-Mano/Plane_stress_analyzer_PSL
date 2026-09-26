# Plane Stress Analyzer

A C# front-end for a plane stress/plane strain finite element analyzer with a C++ solver, supporting h- and p-refinement, Abaqus-style input, interactive OpenTK visualization, and stress line (PSL) post-processing.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Visualization Controls](#visualization-controls)
- [Visualization](#visualization)
- [How to Use the Software](#how-to-use-the-software)
  - [Step 1: Importing the Model](#step-1-importing-the-model)
  - [Step 2: Applying Loads or Constraints](#step-2-applying-loads-or-constraints)
  - [Step 3: Material Update](#step-3-material-update)
  - [Step 4: Solve](#step-4-solve)
  - [Step 5: Post-Processing](#step-5-post-processing)
- [Examples](#examples)
- [Build Instructions](#build-instructions)
- [Repository](#repository)

---

## Overview

This repository contains a C# implementation of a **plane stress analyzer** that serves as a front end for model import, load/constraint application, material assignment, and post-processing of results. The solver is developed in **C++** and linked to the C# front end via a **DLL**.

The solver focuses on **h-refinement** and **p-refinement** finite element analysis:

- **h-refinement** — allows the mesh to be refined from 1 element to 4 or 16 elements. There is an option to save the h-refined model, which can be re-imported into the tool for further refinement if necessary.
- **p-refinement** — supports the following polynomial shape function formulations:

| Order | Triangular Elements | Quadrilateral Elements |
|-------|---------------------|------------------------|
| P = 1 | Linear T3           | Bilinear Q4            |
| P = 2 | Quadratic T6        | Q9                     |
| P = 3 | Cubic T10           | Q16                    |
| P = 4 | Quartic T15         | Q25                    |

The solve can be performed using either the **Elimination method** or the **Lagrange Augmentation method**.

The input model format follows the **Abaqus** format, and example models are provided in the repository.

Post-processing allows visualization of displacement and stress components. **Stress line visualization** is provided in two types:

- **Type 1** — plots stress lines in a grid pattern.
- **Type 2** — plots stress lines in a continuous plot as streamlines.

---

## Features

- Finite element analysis of plane stress / plane strain formulation
- Support for refinement and higher-order triangular and quadrilateral elements
- Abaqus-style input file support
- Interactive visualization using OpenTK
- Post-processing of displacement and stress results
- Stress line plots

---

## Visualization Controls

| Action | Shortcut |
|--------|----------|
| Zoom In / Out | `Ctrl` + Scroll Wheel |
| Pan | `Ctrl` + Right Click Drag |
| Zoom to Fit | `Ctrl` + `F` |
| Select Nodes / Elements | `Shift` + Left Click Drag |
| Deselect | `Shift` + Right Click Drag |

---

## Visualization

- OpenTK 3.3 based rendering
- Contour plots of displacement and stress plots
- Streamline plots of stress lines

![Animated Example 1](Images/gif_plate_with_hole.gif)
![Animated Example 2](Images/gif_padeye.gif)
![Animated Example 3](Images/gif_fourpoint_loading.gif)
![Animated Example 4](Images/gif_circular_arch.gif)
![Animated Example 5](Images/gif_beam_column.gif)
![Animated Example 6](Images/gif_annular_disc.gif)
![Animated Example 7](Images/gif_cantileverbeam_column.gif)

---

## How to Use the Software

### Step 1: Importing the Model

The model format is in Abaqus `*.inp` format. Example models are located in the repository at:

```
/Plane_stress_analyzer_PSL/Example_model/
```

The mesh text file (`*.txt`) follows the format shown below:

```
**
**   Template:  Plane Stress Analyzer
**
*NODE
         1,  -100.0   ,  -100.0   ,  0.0
         2,   100.0   ,  -100.0   ,  0.0
         3,   100.0   ,   100.0   ,  0.0
         4,  -100.0   ,   100.0   ,  0.0
         ......

*ELEMENT,TYPE=S4
        12,    23,    13,     7,    20
        13,    18,    23,    20,     9
        14,     9,    20,    24,    21
        15,    20,     7,    14,    24
        .......

*ELEMENT,TYPE=S3
         0,     0,     1,     2
         1,     2,     1,     3
         3,     2,     3,     4
         .......
```

### Step 2: Applying Loads or Constraints

Open the **Load/Constraint** form from the **Boundary Condition** menu.

- Use `Shift` + Left Click Drag to select nodes.
- Use `Shift` + Right Click to deselect/refine the selection.

Apply the load amplitude and load angle. A load set will be created and visualized in the model. Constraints follow a similar workflow.

### Step 3: Material Update

A default material is applied to the mesh. A new material can be created and applied to meshes by selecting/deselecting using `Shift` + Left Click Drag and `Shift` + Right Click.

Deleting a material will revert the mesh it was applied to back to the default material.

### Step 4: Solve

The solver window is accessed through the **Solve** menu. The solver window allows the DLL connection to the C++ solver to perform the solve.

- Select **h-refinement** and **p-refinement** order.
- Select **Extend the constraints and loads to intermediate refined node** option if necessary.
- Perform solve. The callback from the solver gives information and progress of the solve.
- Results will be automatically loaded to the tool.

If **Save h-refined model** is selected, the bin file will be saved in the same location as the application. It can be imported later using the **Import bin file** option in the **File** menu.

### Step 5: Post-Processing

**Solve → Results** menu allows the selection of various result options:

- Displacement magnitude
- Stress XX
- Stress YY
- Tau XY
- Equivalent or Von Mises stress
- Principal Stress 1
- Principal Stress 2
- Max Shear Stress
- **PSL Type 1** — grid-based stress line visualization (also allows reaction force visualization)
- **PSL Type 2** — streamline-based continuous stress line plot visualization

**Solve → Result Option** menu allows modification of result options:

- **Deflection scale** can be adjusted for visualization.
- **Contour lines** can be turned on and off.
- **Contour level lines** can be changed to 5, 10, 20, 40, or 80 lines.
- Results can be animated using the **Play Animation** / **Stop Animation** button.
- Animation speed can be adjusted to play in real time.
- **Contour plot maximum and minimum range** can be adjusted. Concentrated stresses at a few nodes might skew the contour plot and show a uniform color in most locations. By adjusting the maximum and minimum range (which varies between 1.0 and 0.0), for example by selecting 0.8 for maximum contour range, values above 0.8 times the maximum will not be plotted, allowing for better visualization of the stresses.

**Help → General Instruction** gives a few more instructions on how to use the tool.

---

## Examples

### Problem 1: Classic Thin Plate with Hole in the Center

![Problem 1 Model](Images/prob1_1model_thin_plate_with_holecenter.png)
![Problem 1 Displ](Images/prob1_2displ_thin_plate_with_holecenter.png)
![Problem 1 Stress](Images/prob1_3stress_thin_plate_with_holecenter.png)
![Problem 1 PSL](Images/prob1_4psl_thin_plate_with_holecenter.png)

### Problem 2: Thin Plane with Hole in the Center with Symmetrical Boundary Condition

![Problem 2 Model](Images/prob2_1model_thin_plate_with_holecenter_symm.png)
![Problem 2 Displ](Images/prob2_2displ_thin_plate_with_holecenter_symm.png)
![Problem 2 Stress](Images/prob2_3stress_thin_plate_with_holecenter_symm.png)
![Problem 2 PSL](Images/prob2_4psl_thin_plate_with_holecenter_symm.png)

### Problem 3: Thin Plane with Notches

![Problem 3 Model](Images/prob3_1model_thin_plate_with_notches.png)
![Problem 3 Displ](Images/prob3_2displ_thin_plate_with_notches.png)
![Problem 3 Stress](Images/prob3_3stress_thin_plate_with_notches.png)
![Problem 3 PSL](Images/prob3_4psl_thin_plate_with_notches.png)

### Problem 4: Four Point Loading of Beam

![Problem 4 Model](Images/prob4_1model_beam_fourpoint_loading.png)
![Problem 4 Displ](Images/prob4_2displ_beam_fourpoint_loading.png)
![Problem 4 Stress1](Images/prob4_3stress1_beam_fourpoint_loading.png)
![Problem 4 Stress2](Images/prob4_3stress2_beam_fourpoint_loading.png)
![Problem 4 PSL](Images/prob4_4psl_beam_fourpoint_loading.png)

### Problem 5: Padeye with Inclined Load

![Problem 5 Model](Images/prob5_1model_padeye.png)
![Problem 5 Displ](Images/prob5_2displ_padeye.png)
![Problem 5 Stress](Images/prob5_3stress_padeye.png)
![Problem 5 PSL](Images/prob5_4psl_padeye.png)

### Problem 6: Circular Arch

![Problem 6 Model](Images/prob6_1model_circular_arch.png)
![Problem 6 Displ](Images/prob6_2displ_circular_arch.png)
![Problem 6 Stress1](Images/prob6_3stress1_circular_arch.png)
![Problem 6 Stress2](Images/prob6_3stress2_circular_arch.png)
![Problem 6 PSL](Images/prob6_4psl_circular_arch.png)

### Problem 7: Annular Disc

![Problem 7 Model](Images/prob7_1model_annular_disc.png)
![Problem 7 Displ](Images/prob7_2displ_annular_disc.png)
![Problem 7 Stress1](Images/prob7_3stress1_annular_disc.png)
![Problem 7 Stress2](Images/prob7_3stress2_annular_disc.png)
![Problem 7 PSL](Images/prob7_4psl_annular_disc.png)

### Problem 8: Beam Column

![Problem 8 Model](Images/prob8_1model_beamcolumn.png)
![Problem 8 Displ](Images/prob8_2displ_beamcolumn.png)
![Problem 8 Stress](Images/prob8_3stress_beamcolumn.png)
![Problem 8 PSL](Images/prob8_4psl_beamcolumn.png)

### Problem 9: Frame

![Problem 9 Model](Images/prob9_1model_beamtwocolumn.png)
![Problem 9 Displ](Images/prob9_2displ_beamtwocolumn.png)
![Problem 9 Stress](Images/prob9_3stress_beamtwocolumn.png)
![Problem 9 PSL](Images/prob9_4psl_beamtwocolumn.png)

---

## Build Instructions

**1. Clone the repository:**

```bash
git clone https://github.com/Samson-Mano/Plane_stress_analyzer_PSL.git
cd Plane_stress_analyzer_PSL
```

**2. Download the pre-built software:**

Use [https://download-directory.github.io/](https://download-directory.github.io/) and download the `Release` folder to run the software:

```
Plane_stress_analyzer_PSL/Plane_stress_analyzer_PSL/bin/x64/Release/
```

---

## Repository

- **GitHub:** [https://github.com/Samson-Mano/Plane_stress_analyzer_PSL](https://github.com/Samson-Mano/Plane_stress_analyzer_PSL)

---

## License

This project is licensed under the **MIT License**.

```
MIT License

Copyright (c) 2025 Samson Mano

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```


