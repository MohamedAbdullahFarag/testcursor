using Ikhtibar.Shared.DTOs;
using Ikhtibar.Shared.Entities;
using Ikhtibar.Shared.Enums;
using Ikhtibar.Shared.Models;
namespace Ikhtibar.Core.Repositories.Interfaces;

/// <summary>
/// Repository interface for notification preference entities
/// Manages user notification delivery preferences
/// </summary>
public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
{
    /// <summary>
    /// Gets all preferences for a specific user
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <returns>List of notification preferences for the user</returns>
    Task<IEnumerable<NotificationPreference>> GetUserPreferencesAsync(int userId);

    /// <summary>
    /// Gets preference for a specific user and notification type
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notificationType">Type of notification</param>
    /// <returns>Preference if found, null otherwise</returns>
    Task<NotificationPreference?> GetUserPreferenceAsync(int userId, NotificationType notificationType);

    /// <summary>
    /// Creates or updates a user's preference for a notification type
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="emailEnabled">Email notification enabled</param>
    /// <param name="smsEnabled">SMS notification enabled</param>
    /// <param name="inAppEnabled">In-app notification enabled</param>
    /// <param name="pushEnabled">Push notification enabled</param>
    /// <returns>Updated or created preference</returns>
    Task<NotificationPreference> UpsertPreferenceAsync(
        int userId,
        NotificationType notificationType,
        bool emailEnabled,
        bool smsEnabled,
        bool inAppEnabled,
        bool pushEnabled);

    /// <summary>
    /// Sets default preferences for a new user
    /// </summary>
    /// <param name="userId">New user ID</param>
    /// <returns>List of created default preferences</returns>
    Task<IEnumerable<NotificationPreference>> CreateDefaultPreferencesAsync(int userId);

    /// <summary>
    /// Gets users who have enabled a specific notification channel for a notification type
    /// </summary>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="channel">Notification channel</param>
    /// <returns>List of user IDs who should receive notifications via this channel</returns>
    Task<IEnumerable<int>> GetUsersWithChannelEnabledAsync(NotificationType notificationType, NotificationChannel channel);

    /// <summary>
    /// Updates quiet hours for a user across all notification types
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="quietHoursStart">Start time for quiet hours (HH:mm format)</param>
    /// <param name="quietHoursEnd">End time for quiet hours (HH:mm format)</param>
    /// <param name="timeZone">User's time zone</param>
    /// <returns>Number of preferences updated</returns>
    Task<int> UpdateQuietHoursAsync(int userId, string? quietHoursStart, string? quietHoursEnd, string? timeZone);

    /// <summary>
    /// Checks if a user is within their quiet hours
    /// </summary>
    /// <param name="userId">Target user ID</param>
    /// <param name="checkTime">Time to check (defaults to current time)</param>
    /// <returns>True if user is within quiet hours</returns>
    Task<bool> IsWithinQuietHoursAsync(int userId, DateTime? checkTime = null);

    /// <summary>
    /// Gets preference summary for admin dashboard
    /// </summary>
    /// <returns>Statistics about user notification preferences</returns>
    Task<PreferenceStatsDto> GetPreferenceSummaryAsync();

    /// <summary>
    /// Bulk updates preferences for multiple users
    /// </summary>
    /// <param name="userIds">List of user IDs</param>
    /// <param name="notificationType">Type of notification</param>
    /// <param name="channelUpdates">Dictionary of channel updates</param>
    /// <returns>Number of preferences updated</returns>
    Task<int> BulkUpdatePreferencesAsync(
        IEnumerable<int> userIds,
        NotificationType notificationType,
        Dictionary<NotificationChannel, bool> channelUpdates);
}
