namespace AssessmentAPI_Xunit.model
{
    public class BrandDTO
    {
        public int BrandId { get; set; }
        public int VehicleTypeId { get; set; }
        public string? BrandName { get; set; }
        public string? Description { get; set; }
        public int? SortOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
