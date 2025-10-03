namespace WhotSiv.Data.Entities;


public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public bool Admin { get; set; }
    public bool TempPassword { get; set; }
    public long PasswordExpires { get; set; }
}