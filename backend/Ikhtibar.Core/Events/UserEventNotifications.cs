using Ikhtibar.Core.Services.Interfaces;

namespace Ikhtibar.Core.Events;

/// <summary>
/// User-related notification events
/// </summary>
public class UserEventNotifications
{
    public class UserWelcomeEvent
    {
        public int UserId { get; }
        public string UserName { get; }
        public string UserEmail { get; }

        public UserWelcomeEvent(int userId, string userName, string userEmail)
        {
            UserId = userId;
            UserName = userName;
            UserEmail = userEmail;
        }
    }

    public class PasswordResetEvent
    {
        public int UserId { get; }
        public string ResetToken { get; }
        public string UserEmail { get; }

        public PasswordResetEvent(int userId, string resetToken, string userEmail)
        {
            UserId = userId;
            ResetToken = resetToken;
            UserEmail = userEmail;
        }
    }

    public class RoleAssignmentEvent
    {
        public int UserId { get; }
        public string RoleName { get; }
        public string AssignedBy { get; }

        public RoleAssignmentEvent(int userId, string roleName, string assignedBy)
        {
            UserId = userId;
            RoleName = roleName;
            AssignedBy = assignedBy;
        }
    }
}
