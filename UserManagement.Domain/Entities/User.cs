using UserManagement.Domain.Common;
using UserManagement.Domain.Enums;
using UserManagement.Domain.ValueObjects;

namespace UserManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; }
    public Email Email { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public string FullName { get; private set; }
    public string PasswordHash { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public List<string> Roles { get; private set; } = new();
    public List<string> PasswordHistory { get; private set; } = new();
    public bool MarketingConsent { get; private set; }
    public int TermsVersionAccepted { get; private set; }

    private User() { } // For deserialization

    public User(string username, Email email, PhoneNumber phone, string fullName, string passwordHash, int termsVersion, bool marketingConsent)
    {
        Username = username;
        Email = email;
        Phone = phone;
        FullName = fullName;
        PasswordHash = passwordHash;
        TermsVersionAccepted = termsVersion;
        MarketingConsent = marketingConsent;
        Status = UserStatus.PendingVerification;
        PasswordHistory.Add(passwordHash);
    }

    public void UpdateProfile(string fullName, PhoneNumber phone)
    {
        FullName = fullName;
        Phone = phone;
        SetUpdatedAt();
        IncrementVersion();
    }

    public void ChangePassword(string newHash)
    {
        PasswordHistory.Add(PasswordHash);
        if (PasswordHistory.Count > 5) PasswordHistory.RemoveAt(0);
        PasswordHash = newHash;
        SetUpdatedAt();
        IncrementVersion();
    }

    public void Deactivate(string reason)
    {
        Status = UserStatus.Deactivated;
        SetUpdatedAt();
        IncrementVersion();
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        SetUpdatedAt();
        IncrementVersion();
    }

    public void AddRole(string role)
    {
        if (!Roles.Contains(role)) Roles.Add(role);
        SetUpdatedAt();
        IncrementVersion();
    }

    public void RemoveRole(string role)
    {
        Roles.Remove(role);
        SetUpdatedAt();
        IncrementVersion();
    }
}
