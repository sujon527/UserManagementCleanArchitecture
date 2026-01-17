using System.Text.RegularExpressions;

namespace UserManagement.Domain.ValueObjects;

public record PhoneNumber
{
    public string Value { get; init; }

    public PhoneNumber(string value, string country = "Global")
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty");

        var normalized = Normalize(value, country);
        Value = normalized;
    }

    private static string Normalize(string phone, string country)
    {
        // Remove non-digits except +
        var cleaned = Regex.Replace(phone, @"[^\d+]", "");

        if (country == "Bangladesh")
        {
            if (cleaned.StartsWith("01") && cleaned.Length == 11)
                cleaned = "+88" + cleaned;
            else if (cleaned.StartsWith("8801") && cleaned.Length == 13)
                cleaned = "+" + cleaned;
        }

        return cleaned;
    }

    public override string ToString() => Value;
}
