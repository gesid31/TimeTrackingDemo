namespace TimeTrackingDemo.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TimeTrack")]
    public partial class TimeTrack
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TimeSheetID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TimeSheetIN { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TimeSheetOUT { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        public string TimeSheetText { get; set; }
    }
}
