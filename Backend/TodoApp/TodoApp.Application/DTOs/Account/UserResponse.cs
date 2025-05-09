public class UserResponse
{
    public string Id { get; }
    public string Email { get; }
    public string UserName { get; }

    public UserResponse(string id, string email, string userName)
    {
        Id = id;
        Email = email;
        UserName = userName;
    }
}