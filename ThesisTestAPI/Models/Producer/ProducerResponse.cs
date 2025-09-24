
using NetTopologySuite.Geometries;
using ThesisTestAPI.Models.User;

namespace ThesisTestAPI.Models.Producer
{
    public class ProducerResponse
    {
        public Guid ProducerId { get; set; }
        public string? ProducerName { get; set; }
        public UserResponse? Owner { get; set; }
        public Geometry? Location { get; set; }
        public string? Banner {  get; set; }
        public string? ProducerPicture { get; set; }
    }
}
