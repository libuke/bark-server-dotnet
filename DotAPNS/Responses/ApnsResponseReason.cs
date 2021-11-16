namespace DotAPNS.Responses;

public enum ApnsResponseReason
{
    Unknown,

    // 200
    Success,

    // 400 The collapse identifier exceeds the maximum allowed size
    BadCollapseId,

    // 400 The specified device token was bad. Verify that the request contains a
    // valid token and that the token matches the environment.
    BadDeviceToken,

    // 400 The apns-expiration value is bad.
    BadExpirationDate,

    // 400 The apns-id value is bad.
    BadMessageId,

    // 400 The apns-priority value is bad.
    BadPriority,

    // 400 The apns-topic was invalid.
    BadTopic,

    // 400 The device token does not match the specified topic.
    DeviceTokenNotForTopic,

    // 400 One or more headers were repeated.
    DuplicateHeaders,

    // 400 Idle time out.
    IdleTimeout,

    // 400 The device token is not specified in the request :path. Verify that the
    // :path header contains the device token.
    MissingDeviceToken,

    // 400 The apns-topic header of the request was not specified and was
    // required. The apns-topic header is mandatory when the client is connected
    // using a certificate that supports multiple topics.
    MissingTopic,

    // 400 The message payload was empty.
    PayloadEmpty,

    // 400 Pushing to this topic is not allowed.
    TopicDisallowed,

    // 403 The certificate was bad.
    BadCertificate,

    // 403 The client certificate was for the wrong environment.
    BadCertificateEnvironment,

    // 403 The provider token is stale and a new token should be generated.
    ExpiredProviderToken,

    // 403 The specified action is not allowed.
    Forbidden,

    // 403 The provider token is not valid or the token signature could not be
    // verified.
    InvalidProviderToken,

    // 403 No provider certificate was used to connect to APNs and Authorization
    // header was missing or no provider token was specified.
    MissingProviderToken,

    // 404 The request contained a bad :path value.
    BadPath,

    // 405 The specified :method was not POST.
    MethodNotAllowed,

    // 410 The device token is inactive for the specified topic.
    Unregistered,

    // 413 The message payload was too large. See Creating the Remote Notification
    // Payload in the Apple Local and Remote Notification Programming Guide for
    // details on maximum payload size.
    PayloadTooLarge,

    // 429 The provider token is being updated too often.
    TooManyProviderTokenUpdates,

    // 429 Too many requests were made consecutively to the same device token.
    TooManyRequests,

    // 500 An internal server error occurred.
    InternalServerError,

    // 503 The service is unavailable.
    ServiceUnavailable,

    // 503 The server is shutting down.
    Shutdown
}
