namespace BitBeakAPI.Models
{
    public class ModelResetPassword
    {
        public string Token { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }
}
