﻿namespace DotAPNS.Models;

public enum ApplePushType
{
    // PushTypeAlert is used for notifications that trigger a user interaction —
    // for example, an alert, badge, or sound. If you set this push type, the
    // apns-topic header field must use your app’s bundle ID as the topic. The
    // alert push type is required on watchOS 6 and later. It is recommended on
    // macOS, iOS, tvOS, and iPadOS.
    Alert,

    // PushTypeBackground is used for notifications that deliver content in the
    // background, and don’t trigger any user interactions. If you set this push
    // type, the apns-topic header field must use your app’s bundle ID as the
    // topic. The background push type is required on watchOS 6 and later. It is
    // recommended on macOS, iOS, tvOS, and iPadOS.
    Background,

    // PushTypeVOIP is used for notifications that provide information about an
    // incoming Voice-over-IP (VoIP) call. If you set this push type, the
    // apns-topic header field must use your app’s bundle ID with .voip appended
    // to the end. If you’re using certificate-based authentication, you must
    // also register the certificate for VoIP services. The voip push type is
    // not available on watchOS. It is recommended on macOS, iOS, tvOS, and
    // iPadOS.
    Voip,

    // PushTypeComplication is used for notifications that contain update
    // information for a watchOS app’s complications. If you set this push type,
    // the apns-topic header field must use your app’s bundle ID with
    // .complication appended to the end. If you’re using certificate-based
    // authentication, you must also register the certificate for WatchKit
    // services. The complication push type is recommended for watchOS and iOS.
    // It is not available on macOS, tvOS, and iPadOS.
    Complication,

    // PushTypeFileProvider is used to signal changes to a File Provider
    // extension. If you set this push type, the apns-topic header field must
    // use your app’s bundle ID with .pushkit.fileprovider appended to the end.
    // The fileprovider push type is not available on watchOS. It is recommended
    // on macOS, iOS, tvOS, and iPadOS.
    Fileprovider,

    // PushTypeMDM is used for notifications that tell managed devices to
    // contact the MDM server. If you set this push type, you must use the topic
    // from the UID attribute in the subject of your MDM push certificate.
    Mdm
}
