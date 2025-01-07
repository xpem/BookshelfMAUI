Caso o projeto dê o erro "Arquivo de ativo está faltando no destino" ao baixar, rode o comando no cmd: dotnet restore

O arquivo com as chaves do gitignore:

    public static class ApiKeys
    {
        public const string ApiAddress = "";

        public const string KEY32 = "";
        public const string IV16 = "";

        //google auth
        public static readonly string GoogleClientID = "";
        public static readonly string GoogleClientSecretKey = "";
    }
