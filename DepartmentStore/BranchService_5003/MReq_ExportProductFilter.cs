namespace BranchService_5003
{
    public class MReq_ExportProductFilter
    {
        public int? IdBranch { get; set; }
        public int? IdProduct { get; set; }
        public DateOnly? ExportTime { get; set; }
    }
}
