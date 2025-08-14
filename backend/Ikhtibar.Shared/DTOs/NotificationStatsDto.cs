using System.Collections.Generic;
using Ikhtibar.Shared.Enums;


namespace Ikhtibar.Shared.DTOs;

/// <summary>
/// Summary of notification preferences across all users for admin dashboard
/// </summary>
public class PreferenceStatsDto
{
    /// <summary>
    /// Total number of users with notification preferences configured
    /// </summary>
    public int TotalUsers { get; set; }
    
    /// <summary>
    /// Breakdown of preferences by notification type
    /// </summary>
    public Dictionary<NotificationType, ChannelPreferenceStatsDto> PreferencesByType { get; set; } = new();
}

/// <summary>
/// Statistics about channel preferences for a notification type
/// </summary>
public class ChannelPreferenceStatsDto
{
    /// <summary>
    /// Number of users who have email enabled for this notification type
    /// </summary>
    public int EmailEnabled { get; set; }
    
    /// <summary>
    /// Number of users who have SMS enabled for this notification type
    /// </summary>
    public int SmsEnabled { get; set; }
    
    /// <summary>
    /// Number of users who have in-app notifications enabled for this notification type
    /// </summary>
    public int InAppEnabled { get; set; }
    
    /// <summary>
    /// Number of users who have push notifications enabled for this notification type
    /// </summary>
    public int PushEnabled { get; set; }
    
    /// <summary>
    /// Total number of users with preferences set for this notification type
    /// </summary>
    public int TotalUsers { get; set; }
}
