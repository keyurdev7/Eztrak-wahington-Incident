namespace ViewModels.Authentication
{
    public class UserVM : CommonSignUpVM
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }
        public string Role { get; set; } 
        public string Designation { get; set; }
    }


}
