Caso o projeto dê erro ao baixar, rode um <dotnet restore> 

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
