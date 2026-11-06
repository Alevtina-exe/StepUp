using System.Text.RegularExpressions;

namespace StepUp.Application.Services;

public static class PasswordPolicy
{
    public const int MinLength = 8;

    public static List<string> Validate(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password) || password.Length < MinLength)
            errors.Add($"Пароль должен содержать минимум {MinLength} символов.");

        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("Пароль должен содержать хотя бы одну строчную букву.");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("Пароль должен содержать хотя бы одну заглавную букву.");

        if (!Regex.IsMatch(password, @"\d"))
            errors.Add("Пароль должен содержать хотя бы одну цифру.");

        if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
            errors.Add("Пароль должен содержать хотя бы один спецсимвол (!@#$%^&* и т.д.).");

        return errors;
    }

    public static bool IsValid(string password) => Validate(password).Count == 0;
}
