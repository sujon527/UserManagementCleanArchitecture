using System.Text.RegularExpressions;

namespace UserManagement.Domain.ValueObjects;

public record Email
{
    public string Value { get; init; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");

        var normalized = Normalize(value);
        if (!IsValid(normalized))
            throw new ArgumentException("Invalid email format");

        Value = normalized;
    }

    private static string Normalize(string email)
    {
        email = email.Trim().ToLowerInvariant();
        
        // Gmail normalization (conceptual)
        if (email.Contains("@gmail.com"))
        {
            var parts = email.Split('@');
            var localPart = parts[0].Split('+')[0].Replace(".", "");
            return $"{localPart}@{parts[1]}";
        }

        return email;
    }

    private static bool IsValid(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public override string ToString() => Value;
}
