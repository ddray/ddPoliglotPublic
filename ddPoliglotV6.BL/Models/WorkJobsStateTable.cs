using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ddPoliglotV6.BL.Models
{
    public class WorkJobsStateEntity : TableEntity
    {
        public WorkJobsStateEntity() { }

        public int State { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
        public string Args { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}
