namespace Registration
{
    public class UserDTOReg
    {
        public string UserNickName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserDTOLog
    {
        public string UserNickName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
