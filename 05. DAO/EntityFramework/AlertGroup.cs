//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TKM.DAO.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class AlertGroup
    {
        public int ID { get; set; }
        public string AlertName { get; set; }
        public string AlertNameTrans { get; set; }
        public string Severity { get; set; }
        public string AlertDescription { get; set; }
        public string AlertDescriptionTrans { get; set; }
        public string Recommendations { get; set; }
        public string RecommendationsTrans { get; set; }
        public string AlertVariants { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}
