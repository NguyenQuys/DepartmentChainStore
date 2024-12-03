namespace BranchService_5003.Request
{
    public class MReq_ExportProductFilter
    {
        public int? IdBranch { get; set; }
        public int? IdProduct { get; set; }
        public DateOnly? ExportTime { get; set; }
    }
}
