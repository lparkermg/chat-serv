namespace ChatServ.Api.Models.Requests
{
    public class CreateRoomRequest
    {
        public required string Id { get; set; }

        public required string Name { get; set; }
    }
}
