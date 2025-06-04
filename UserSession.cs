enum UserStep
{
    AwaitingPassportPhoto,
    ConfirmPassportData,
    AwaitingRegistrationPhoto,
    ConfirmRegistrationData,
    ConfirmPrice,
    Completed
}

class UserSession
{
    public UserStep Step { get; set; } = UserStep.AwaitingPassportPhoto;
    public string PassportData { get; set; } = "";
    public string RegistrationData { get; set; } = "";
}
