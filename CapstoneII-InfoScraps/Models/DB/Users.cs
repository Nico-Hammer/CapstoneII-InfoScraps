using System.ComponentModel.DataAnnotations;

namespace CapstoneII_InfoScraps.Models.DB;

public class User
{
    // pk id
    [Required]
    public int Id { get; set; }
    // fk accountid
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;
    // username
    [Required(ErrorMessage = "Enter a username")]
    public string Username { get; set; }
    // email address
    [Required(ErrorMessage = "Enter an email address")]
    [DataType(DataType.EmailAddress)] 
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; }
    // phone number
    [DataType(DataType.PhoneNumber)]
    [Phone(ErrorMessage = "Enter  a valid phone number")]
    public string? Phone_Number { get; set; }
    // password
    // regex from https://full-time.learnhowtoprogram.com/c-and-net/authentication-with-identity/authentication-with-identity-user-registration-viewmodel-validation-and-views
    [Required(ErrorMessage = "Enter a password")]
    [DataType(DataType.Password)]
    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", 
        ErrorMessage = "Your password must contain at least 8 characters, a capital letter, a lowercase letter, a number, and a special character.")]
    public string Password { get; set; }
}