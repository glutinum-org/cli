/**
 * @deprecated use `isInlineTag`
 * Schedules the localNotification for immediate presentation.
 * details is an object containing:
 * alertBody : The message displayed in the notification alert.
 * alertAction : The "action" displayed beneath an actionable notification. Defaults to "view";
 * soundName : The sound played when the notification is fired (optional). The file should be added in the ios project from Xcode, on your target, so that it is bundled in the final app. For more details see the example app.
 * category : The category of this notification, required for actionable notifications (optional).
 * userInfo : An optional object containing additional notification data.
 * applicationIconBadgeNumber (optional) : The number to display as the app's icon badge. The default value of this property is 0, which means that no badge is displayed.
 */
declare function isInlineTag(tagName: string): boolean;
