//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestService
{
    using System;
    using System.Collections.Generic;
    
    public partial class Operators_Locations
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Operators_Locations()
        {
            this.ImportProcesses = new HashSet<ImportProcess>();
        }
    
        public System.Guid OperatorLocationId { get; set; }
        public Nullable<System.Guid> OperatorId { get; set; }
        public string LocationName { get; set; }
        public string LocationVATid { get; set; }
        public string LocationMainContact { get; set; }
        public string LocationEmail { get; set; }
        public string LocationTelephone { get; set; }
        public string LocationCity { get; set; }
        public string LocationAddressLine1 { get; set; }
        public string LocationAddressLine2 { get; set; }
        public string LocationZIP { get; set; }
        public string LocationCountry { get; set; }
        public string LocationStateOrProvince { get; set; }
        public string LocationRegion { get; set; }
        public Nullable<int> LocationCategory { get; set; }
        public Nullable<int> LocationSubCategory { get; set; }
        public Nullable<int> LocationGhostlyClassification { get; set; }
        public Nullable<decimal> LocationStorageCapacityLiquids { get; set; }
        public Nullable<decimal> LocationStorageCapacityVolume { get; set; }
        public Nullable<decimal> LocationStorageUsedCapacityLiquids { get; set; }
        public Nullable<decimal> LocationStorageUsedCapacityVolume { get; set; }
        public System.DateTime date_creation { get; set; }
        public Nullable<System.Guid> created_by { get; set; }
        public Nullable<System.Guid> modified_by { get; set; }
        public Nullable<System.DateTime> date_modified { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImportProcess> ImportProcesses { get; set; }
        public virtual Operator Operator { get; set; }
    }
}
