namespace Models.Responses
{
    public enum ErrorCodeTypes
    {
        UserEmailPasswordLoginType = 0,
        GoogleAuthNullEmailOrName = 1,
        InvalidObject = 2,
        TryCreateExistingUser = 3,
        SendEmailError = 4,
        InvalidUserPasswordLogin = 5,
        InvalidPasswordConfirmation = 6,
        ExistingObject = 7,
        ErrorCreatingObject = 8,
        TryDeleteSystemDefaultObject = 9,
        InvalidId = 10,
        TryDeleteObjectWithDependencies = 11,
        ErrorUpdatingObject = 12,
        ErrorDeletingObject = 13,
        InvalidPage = 14,
        ExistingIndex = 15,

        ServerUnavaliable = 16,
        Unknown = 17,
        Unauthorized = 18, 
    }
}
