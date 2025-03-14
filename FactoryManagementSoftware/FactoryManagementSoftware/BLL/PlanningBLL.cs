﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryManagementSoftware.BLL
{
    public class PlanningBLL
    {
        public int plan_id { get; set; }
        public DateTime plan_added_date { get; set; }
        public int plan_added_by { get; set; }
        public DateTime plan_updated_date { get; set; }
        public int plan_updated_by { get; set; }
        public string plan_status { get; set; }
        public string plan_note { get; set; }

        public string part_name { get; set; }
        public string part_code { get; set; }
        public string part_color { get; set; }
        public string quo_ton { get; set; }
        public string plan_ct { get; set; }
        public string plan_pw { get; set; }
        public string plan_rw { get; set; }
        public string plan_cavity { get; set; }

        public string material_code { get; set; }
        public string material_bag_kg { get; set; }
        public string material_bag_qty { get; set; }
        public string material_recycle_use { get; set; }
        
        public string color_material_code { get; set; }
        public string color_material_usage { get; set; }
        public string color_material_qty { get; set; }

        public string production_purpose { get; set; }
        public string production_Old_target_qty { get; set; }
        public string production_target_qty { get; set; }
        public string production_able_produce_qty { get; set; }
        public string production_day { get; set; }
        public string production_hour_per_day { get; set; }
        public string production_hour { get; set; }
        public DateTime production_start_date { get; set; }
        public DateTime production_end_date { get; set; }

        public int plan_produced { get; set; }

        public int machine_id { get; set; }
        public int family_with { get; set; } = -1;
        public bool recording { get; set; }

        public bool Checked { get; set; }
        public DateTime CheckedDate { get; set; }
        public int CheckedBy { get; set; }



    }
}
