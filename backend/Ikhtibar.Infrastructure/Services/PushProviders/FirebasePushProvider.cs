using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Ikhtibar.Core.Integrations.PushProviders;
using Ikhtibar.Shared.Enums;
using System.Text.Json;

// Explicitly using Firebase notification to avoid ambiguity
using FirebaseNotification = FirebaseAdmin.Messaging.Notification;
// Using the PushPlatform enum from Core.Integrations namespace
using CorePushPlatform = Ikhtibar.Core.Integrations.PushProviders.PushPlatform;

namespace Ikhtibar.Infrastructure.Services.PushProviders;

/// <summary>
/// Firebase Cloud Messaging (FCM) implementation of IPushProvider.
/// Provides push notification delivery through Google's Firebase platform.
/// Supports iOS, Android, and web push notifications with advanced targeting.
/// </summary>
public class FirebasePushProvider : IPushProvider
{
    private readonly FirebaseOptions _options;
    private readonly ILogger<FirebasePushProvider> _logger;
    private readonly FirebaseMessaging? _messaging;

    public string ProviderName => "Firebase";

    public bool IsAvailable => _messaging != null && !string.IsNullOrEmpty(_options.ProjectId);

    public FirebasePushProvider(
        IOptions<FirebaseOptions> options,
        ILogger<FirebasePushProvider> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        try
        {
            if (!string.IsNullOrEmpty(_options.ServiceAccountKeyPath))
            {
                var credential = GoogleCredential.FromFile(_options.ServiceAccountKeyPath);
                var app = FirebaseApp.Create(new FirebaseAdmin.AppOptions
                {
                    Credential = credential,
                    ProjectId = _options.ProjectId
                });
                _messaging = FirebaseMessaging.GetMessaging(app);
            }
            else if (!string.IsNullOrEmpty(_options.ServiceAccountKeyJson))
            {
                var credential = GoogleCredential.FromJson(_options.ServiceAccountKeyJson);
                var app = FirebaseApp.Create(new FirebaseAdmin.AppOptions
                {
                    Credential = credential,
                    ProjectId = _options.ProjectId
                });
                _messaging = FirebaseMessaging.GetMessaging(app);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize Firebase messaging");
        }
    }

    public async Task<PushDeliveryResult> SendPushAsync(PushRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Sending push notification via Firebase to device {DeviceToken}", 
            request.DeviceToken);

        try
        {
            ValidatePushRequest(request);

            var message = BuildFirebaseMessage(request);
            
            if (_messaging == null)
            {
                throw new InvalidOperationException("Firebase messaging is not initialized");
            }
            
            var response = await _messaging.SendAsync(message, cancellationToken);

            _logger.LogInformation("Push notification sent successfully via Firebase to device {DeviceToken} with message ID {MessageId}", 
                request.DeviceToken, response);

            return new PushDeliveryResult
            {
                Success = true,
                MessageId = response,
                SentAt = DateTime.UtcNow,
                DevicesReached = 1,
                Metadata = new Dictionary<string, object>
                {
                    { "firebaseMessageId", response },
                    { "platform", request.Platform.ToString() }
                }
            };
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, "Firebase messaging error sending push to device {DeviceToken}: {ErrorCode} - {ErrorMessage}", 
                request.DeviceToken, ex.ErrorCode, ex.Message);
            
            return new PushDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = ex.ErrorCode.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send push notification via Firebase to device {DeviceToken}", request.DeviceToken);
            return new PushDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "FIREBASE_ERROR"
            };
        }
    }

    public async Task<BulkPushDeliveryResult> SendBulkPushAsync(IEnumerable<PushRequest> requests, CancellationToken cancellationToken = default)
    {
        var requestList = requests.ToList();
        using var scope = _logger.BeginScope("Sending bulk push notifications via Firebase to {Count} devices", requestList.Count);

        var result = new BulkPushDeliveryResult
        {
            TotalProcessed = requestList.Count
        };

        try
        {
            // Group requests by similar configuration for efficient batching
            var messageGroups = GroupMessagesByConfiguration(requestList);

            foreach (var group in messageGroups)
            {
                try
                {
                    var messages = group.Select(BuildFirebaseMessage).ToList();
                    
                    if (_messaging == null)
                    {
                        throw new InvalidOperationException("Firebase messaging is not initialized");
                    }
                    
                    var batchResponse = await _messaging.SendEachAsync(messages, cancellationToken);

                    // Process individual results
                    for (int i = 0; i < batchResponse.Responses.Count; i++)
                    {
                        var response = batchResponse.Responses[i];
                        var originalRequest = group[i];

                        var deliveryResult = new PushDeliveryResult
                        {
                            Success = response.IsSuccess,
                            MessageId = response.MessageId,
                            ErrorMessage = response.Exception?.Message,
                            ErrorCode = response.Exception is FirebaseMessagingException fme ? fme.ErrorCode.ToString() : null,
                            SentAt = DateTime.UtcNow,
                            DevicesReached = response.IsSuccess ? 1 : 0,
                            Metadata = new Dictionary<string, object>
                            {
                                { "deviceToken", originalRequest.DeviceToken },
                                { "platform", originalRequest.Platform.ToString() }
                            }
                        };

                        result.Results.Add(deliveryResult);

                        if (deliveryResult.Success)
                            result.SuccessCount++;
                        else
                            result.FailureCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send batch of {Count} push notifications", group.Count);
                    
                    // Mark all in this group as failed
                    foreach (var req in group)
                    {
                        result.Results.Add(new PushDeliveryResult
                        {
                            Success = false,
                            ErrorMessage = ex.Message,
                            ErrorCode = "BATCH_ERROR"
                        });
                        result.FailureCount++;
                    }
                }
            }

            _logger.LogInformation("Bulk push notifications completed via Firebase: {SuccessCount} successful, {FailureCount} failed out of {TotalProcessed}",
                result.SuccessCount, result.FailureCount, result.TotalProcessed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bulk push notification operation failed via Firebase");
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public async Task<PushDeliveryResult> SendTopicPushAsync(TopicPushRequest request, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Sending topic push notification via Firebase to topic {Topic}", request.Topic);

        try
        {
            ValidateTopicPushRequest(request);

            var message = BuildFirebaseTopicMessage(request);
            
            if (_messaging == null)
            {
                throw new InvalidOperationException("Firebase messaging is not initialized");
            }
            
            var response = await _messaging.SendAsync(message, cancellationToken);

            _logger.LogInformation("Topic push notification sent successfully via Firebase to topic {Topic} with message ID {MessageId}", 
                request.Topic, response);

            return new PushDeliveryResult
            {
                Success = true,
                MessageId = response,
                SentAt = DateTime.UtcNow,
                Metadata = new Dictionary<string, object>
                {
                    { "firebaseMessageId", response },
                    { "topic", request.Topic },
                    { "condition", request.Condition ?? "" }
                }
            };
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, "Firebase messaging error sending topic push to {Topic}: {ErrorCode} - {ErrorMessage}", 
                request.Topic, ex.ErrorCode, ex.Message);
            
            return new PushDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = ex.ErrorCode.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send topic push notification via Firebase to topic {Topic}", request.Topic);
            return new PushDeliveryResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "FIREBASE_TOPIC_ERROR"
            };
        }
    }

    public async Task<TopicSubscriptionResult> SubscribeToTopicAsync(string deviceToken, string topic, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Subscribing device {DeviceToken} to topic {Topic}", deviceToken, topic);

        try
        {
            if (_messaging == null)
            {
                throw new InvalidOperationException("Firebase messaging is not initialized");
            }
            
            var response = await _messaging.SubscribeToTopicAsync(new[] { deviceToken }, topic);

            _logger.LogInformation("Device subscribed to topic successfully: {SuccessCount} successful, {FailureCount} failed", 
                response.SuccessCount, response.FailureCount);

            return new TopicSubscriptionResult
            {
                Success = response.SuccessCount > 0,
                ProcessedTokens = response.SuccessCount,
                ErrorMessage = response.FailureCount > 0 ? $"{response.FailureCount} tokens failed to subscribe" : null,
                Metadata = new Dictionary<string, object>
                {
                    { "successCount", response.SuccessCount },
                    { "failureCount", response.FailureCount }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe device {DeviceToken} to topic {Topic}", deviceToken, topic);
            return new TopicSubscriptionResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "FIREBASE_SUBSCRIPTION_ERROR"
            };
        }
    }

    public async Task<TopicSubscriptionResult> UnsubscribeFromTopicAsync(string deviceToken, string topic, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Unsubscribing device {DeviceToken} from topic {Topic}", deviceToken, topic);

        try
        {
            if (_messaging == null)
            {
                throw new InvalidOperationException("Firebase messaging is not initialized");
            }
            
            var response = await _messaging.UnsubscribeFromTopicAsync(new[] { deviceToken }, topic);

            _logger.LogInformation("Device unsubscribed from topic successfully: {SuccessCount} successful, {FailureCount} failed", 
                response.SuccessCount, response.FailureCount);

            return new TopicSubscriptionResult
            {
                Success = response.SuccessCount > 0,
                ProcessedTokens = response.SuccessCount,
                ErrorMessage = response.FailureCount > 0 ? $"{response.FailureCount} tokens failed to unsubscribe" : null,
                Metadata = new Dictionary<string, object>
                {
                    { "successCount", response.SuccessCount },
                    { "failureCount", response.FailureCount }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe device {DeviceToken} from topic {Topic}", deviceToken, topic);
            return new TopicSubscriptionResult
            {
                Success = false,
                ErrorMessage = ex.Message,
                ErrorCode = "FIREBASE_UNSUBSCRIPTION_ERROR"
            };
        }
    }

    public Task<DeviceTokenValidationResult> ValidateDeviceTokenAsync(string deviceToken, CorePushPlatform platform, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope("Validating device token {DeviceToken} for platform {Platform}", deviceToken, platform);

        try
        {
            // Basic validation - Firebase doesn't provide a direct validation API
            if (string.IsNullOrEmpty(deviceToken))
            {
                return Task.FromResult(new DeviceTokenValidationResult
                {
                    IsValid = false,
                    Reason = "Device token is required"
                });
            }

            // Basic format validation based on platform
            var isValidFormat = ValidateTokenFormat(deviceToken, platform);

            return Task.FromResult(new DeviceTokenValidationResult
            {
                IsValid = isValidFormat,
                Reason = isValidFormat ? "Valid format" : "Invalid token format",
                IsActive = isValidFormat, // Can't verify without sending
                DetectedPlatform = platform
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Device token validation failed for {DeviceToken}", deviceToken);
            return Task.FromResult(new DeviceTokenValidationResult
            {
                IsValid = false,
                Reason = ex.Message
            });
        }
    }

    public Task<PushDeliveryStatus> GetDeliveryStatusAsync(string messageId, CancellationToken cancellationToken = default)
    {
        // Firebase doesn't provide direct delivery status API
        // Status would typically come through webhooks or client-side reporting
        return Task.FromResult(new PushDeliveryStatus
        {
            Status = NotificationDeliveryStatus.Sent, // Best we can do is "sent"
            LastUpdated = DateTime.UtcNow,
            Events = new List<PushDeliveryEvent>
            {
                new()
                {
                    EventType = "sent",
                    Timestamp = DateTime.UtcNow,
                    Description = "Push notification sent via Firebase",
                    Data = new Dictionary<string, object> { { "messageId", messageId } }
                }
            }
        });
    }

    private Message BuildFirebaseMessage(PushRequest request)
    {
        var notification = new FirebaseNotification
        {
            Title = request.Title,
            Body = request.Body,
            ImageUrl = request.ImageUrl
        };

        var messageBuilder = new Message
        {
            Token = request.DeviceToken,
            Notification = notification,
            Data = request.Data.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? string.Empty)
        };

        // Add platform-specific options
        if (request.PlatformOptions != null)
        {
            if (request.Platform == CorePushPlatform.Android && request.PlatformOptions.Android != null)
            {
                messageBuilder.Android = BuildAndroidConfig(request.PlatformOptions.Android);
            }
            else if (request.Platform == CorePushPlatform.Ios && request.PlatformOptions.Ios != null)
            {
                messageBuilder.Apns = BuildApnsConfig(request.PlatformOptions.Ios);
            }
            else if (request.Platform == CorePushPlatform.Web && request.PlatformOptions.WebPush != null)
            {
                messageBuilder.Webpush = BuildWebpushConfig(request.PlatformOptions.WebPush);
            }
        }

        // Set TTL if specified
        if (request.TimeToLiveSeconds.HasValue)
        {
            messageBuilder.Android ??= new AndroidConfig();
            // Note: TtlDuration property may not be available in this version of FirebaseAdmin
            // This would need to be set via headers or other configuration
        }

        return messageBuilder;
    }

    private Message BuildFirebaseTopicMessage(TopicPushRequest request)
    {
        var notification = new FirebaseNotification
        {
            Title = request.Title,
            Body = request.Body,
            ImageUrl = request.ImageUrl
        };

        var message = new Message
        {
            Notification = notification,
            Data = request.Data.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? string.Empty)
        };

        // Set topic or condition
        if (!string.IsNullOrEmpty(request.Condition))
        {
            message.Condition = request.Condition;
        }
        else
        {
            message.Topic = request.Topic;
        }

        return message;
    }

    private static AndroidConfig BuildAndroidConfig(AndroidOptions options)
    {
        var config = new AndroidConfig();

        if (!string.IsNullOrEmpty(options.ChannelId) || !string.IsNullOrEmpty(options.Icon) || 
            !string.IsNullOrEmpty(options.Color) || !string.IsNullOrEmpty(options.Sound))
        {
            config.Notification = new AndroidNotification
            {
                ChannelId = options.ChannelId,
                Icon = options.Icon,
                Color = options.Color,
                Sound = options.Sound,
                Tag = options.Tag,
                ClickAction = options.ClickAction
            };
        }

        if (!string.IsNullOrEmpty(options.Priority))
        {
            config.Priority = options.Priority.ToLower() == "high" ? Priority.High : Priority.Normal;
        }

        if (!string.IsNullOrEmpty(options.RestrictedPackageName))
        {
            config.RestrictedPackageName = options.RestrictedPackageName;
        }

        return config;
    }

    private static ApnsConfig BuildApnsConfig(IosOptions options)
    {
        var config = new ApnsConfig();
        var aps = new Aps();

        if (!string.IsNullOrEmpty(options.Sound))
        {
            aps.Sound = options.Sound;
        }

        if (options.Badge.HasValue)
        {
            aps.Badge = options.Badge.Value;
        }

        if (options.ContentAvailable.HasValue && options.ContentAvailable.Value)
        {
            aps.ContentAvailable = true;
        }

        if (options.MutableContent.HasValue && options.MutableContent.Value)
        {
            aps.MutableContent = true;
        }

        if (!string.IsNullOrEmpty(options.Category))
        {
            aps.Category = options.Category;
        }

        if (!string.IsNullOrEmpty(options.ThreadId))
        {
            aps.ThreadId = options.ThreadId;
        }

        config.Aps = aps;
        return config;
    }

    private static WebpushConfig BuildWebpushConfig(WebPushOptions options)
    {
        var config = new WebpushConfig();

        if (!string.IsNullOrEmpty(options.Icon) || !string.IsNullOrEmpty(options.Badge) ||
            !string.IsNullOrEmpty(options.Image) || options.RequireInteraction.HasValue)
        {
            var notification = new WebpushNotification
            {
                Title = null, // Will be set from main notification
                Body = null   // Will be set from main notification
            };

            // Set custom properties via data payload instead
            var data = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(options.Icon))
                data["icon"] = options.Icon;

            if (!string.IsNullOrEmpty(options.Badge))
                data["badge"] = options.Badge;

            if (!string.IsNullOrEmpty(options.Image))
                data["image"] = options.Image;

            if (!string.IsNullOrEmpty(options.Tag))
                data["tag"] = options.Tag;

            if (options.RequireInteraction.HasValue)
                data["requireInteraction"] = options.RequireInteraction.Value;

            if (!string.IsNullOrEmpty(options.Language))
                data["lang"] = options.Language;

            if (options.Vibrate != null && options.Vibrate.Length > 0)
                data["vibrate"] = options.Vibrate;

            config.Notification = notification;
            config.Data = data.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString() ?? string.Empty);
        }

        var headers = new Dictionary<string, string>();

        if (options.Ttl.HasValue)
            headers["TTL"] = options.Ttl.Value.ToString();

        if (!string.IsNullOrEmpty(options.Urgency))
            headers["Urgency"] = options.Urgency;

        if (headers.Any())
            config.Headers = headers;

        return config;
    }

    private static List<List<PushRequest>> GroupMessagesByConfiguration(List<PushRequest> requests)
    {
        // Group by platform for efficient batching
        return requests
            .GroupBy(r => new { r.Platform, HasPlatformOptions = r.PlatformOptions != null })
            .Select(g => g.ToList())
            .ToList();
    }

    private static void ValidatePushRequest(PushRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrEmpty(request.DeviceToken))
            throw new ArgumentException("DeviceToken is required", nameof(request));

        if (string.IsNullOrEmpty(request.Title) && string.IsNullOrEmpty(request.Body))
            throw new ArgumentException("Either Title or Body is required", nameof(request));
    }

    private static void ValidateTopicPushRequest(TopicPushRequest request)
    {
        ValidatePushRequest(request);

        if (string.IsNullOrEmpty(request.Topic) && string.IsNullOrEmpty(request.Condition))
            throw new ArgumentException("Either Topic or Condition is required", nameof(request));
    }

    private static bool ValidateTokenFormat(string token, CorePushPlatform platform)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        return platform switch
        {
            CorePushPlatform.Ios => token.Length > 50, // iOS tokens are typically 64 characters
            CorePushPlatform.Android => token.Length > 100, // FCM tokens are typically 140+ characters
            CorePushPlatform.Web => token.Length > 50, // Web push tokens vary
            _ => token.Length > 10
        };
    }
}

/// <summary>
/// Configuration options for Firebase push provider.
/// </summary>
public class FirebaseOptions
{
    public const string ConfigSection = "Firebase";

    /// <summary>
    /// Firebase project ID.
    /// </summary>
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// Path to service account key file.
    /// </summary>
    public string? ServiceAccountKeyPath { get; set; }

    /// <summary>
    /// Service account key JSON content.
    /// </summary>
    public string? ServiceAccountKeyJson { get; set; }

    /// <summary>
    /// Default notification icon URL.
    /// </summary>
    public string? DefaultIcon { get; set; }

    /// <summary>
    /// Default click action URL.
    /// </summary>
    public string? DefaultClickAction { get; set; }

    /// <summary>
    /// Maximum batch size for bulk operations.
    /// </summary>
    public int MaxBatchSize { get; set; } = 500;

    /// <summary>
    /// Enable detailed logging for debugging.
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;
}
