static class InsurancePolicy
{
    public static string GenerateInsurancePolicy(UserSession session)
    {
        return
        $"""
        🧾 Страховий поліс

        👤 Паспортні дані:
        {session.PassportData}

        🚗 Дані авто:
        {session.RegistrationData}

        ✅ Поліс дійсний з {DateTime.Now:dd.MM.yyyy} по {DateTime.Now.AddYears(1):dd.MM.yyyy}
        """;
    }
}
