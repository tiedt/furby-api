namespace Project.WebAPI.Dtos
{
    public class AlterPasswordDto
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
