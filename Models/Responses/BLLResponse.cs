namespace Models.Responses
{
    public class BLLResponse
    {
        public bool Success { get; set; }

        public object? Content { get; set; }

        public bool TryRefreshToken { get; set; } = false;

        public ErrorTypes? Error { get; set; }

        public string ErrorMessage => Error switch
        {
            ErrorTypes.ServerUnavaliable => "Servidor indisponível",
            ErrorTypes.WrongEmailOrPassword => "Senha ou email inválidos",
            null => string.Empty,
            _ => throw new NotImplementedException("Erro não mapeado")
        };
    }
}
