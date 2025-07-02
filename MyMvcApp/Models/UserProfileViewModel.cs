namespace MyMvcApp.Models
{
    public class UserProfileViewModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }

    }
}