using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace CapstoneII_InfoScraps.Models.DB;

public class Users
{
    // pk id
    [Required]
    public int Id { get; set; }
    // fk accountid
    public int AccountId { get; set; }
    public Accounts Account { get; set; } = null!;
    // username
    [Required]
    public string Username { get; set; }
    // email address
    [Required]
    public string Email { get; set; }
    // phone number
    public string? Phone_Number { get; set; }
    // password
    [Required]
    public string Password { get; set; }
}