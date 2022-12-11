namespace Registration.Models
{
    public class RegistrUser
    {
        public int Id { get; set; }
        public string UserNickName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }  
        public byte[] PasswordSalt { get; set; }  

    }
}

