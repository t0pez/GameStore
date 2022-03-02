using System;

namespace GameStore.Web.Endpoints.Games
{
    public class UpdateRequest
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] File { get; set; }
    }
}
