namespace Models.Responses
{
    public class ServiceResponse
    {
        public bool Success { get; set; }

        public object? Content { get; set; }

        public bool TryRefreshToken { get; set; } = false;

        public ErrorCodeTypes? ErrorCode { get; set; }

        public string ErrorMessage => ErrorCode switch
        {
            ErrorCodeTypes.ServerUnavaliable => "Servidor indisponível",
            ErrorCodeTypes.InvalidUserPasswordLogin => "Senha ou email inválidos",
            null => string.Empty,
            _ => throw new NotImplementedException("Erro não mapeado")
        };
    }
}
