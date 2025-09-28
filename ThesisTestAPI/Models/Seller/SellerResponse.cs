
using NetTopologySuite.Geometries;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Models.Producer
{
    public class SellerResponse
    {
        public Guid SellerId { get; set; }
        public string? SellerName { get; set; }
        public UserResponse? Owner { get; set; }
        public Geometry? Location { get; set; }
        public string? Banner {  get; set; }
        public string? SellerPicture { get; set; }
    }
}
