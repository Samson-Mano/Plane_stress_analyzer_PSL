using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plane_stress_analyzer_PSL.src.model_store.fe_objects
{

    public class material_data
    {
        public int material_id = 0;
        public string material_name = "";

        public double youngs_modulus = 0.0; // E
        public double poissons_ratio = 0.0; // G
        public double material_density = 0.0; // Rho

        public int number_of_elements_appliedto = 0;

    }


    public class fedata_store
    {
        public int p_order = 4; // higher order p - refinement

        public node_list_store fe_nodes;
        public elementtri_list_store fe_tris;
        public elementquad_list_store fe_quads;

        public nodecnst_list_store fe_constraints;
        public nodeload_list_store fe_loads;

        public Dictionary<int, material_data> fe_materials;
        public List<int> materialids;
        // public label_list_store materiallabels;


        public fedata_store()
        {
            // (Re)Initialize the data
            p_order = 4;

            fe_nodes = new node_list_store();
            fe_tris = new elementtri_list_store();
            fe_quads = new elementquad_list_store();

            fe_constraints = new nodecnst_list_store();
            fe_loads = new nodeload_list_store();

            fe_materials = new Dictionary<int, material_data>();
            materialids = new List<int>();



        }




    }
}
