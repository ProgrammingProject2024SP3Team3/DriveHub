
namespace DriveHubTests
{
    internal class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        public User(string id, string userName)
        {
            Id = id;
            UserName = userName;
        }
    }
}
