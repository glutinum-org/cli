module rec Glutinum

open Fable.Core
open Fable.Core.JsInterop
open System

[<AbstractClass>]
[<Erase>]
type Exports =
    [<Import("isInlineTag", "REPLACE_ME_WITH_MODULE_NAME"); Obsolete("use `isInlineTag`\nSchedules the localNotification for immediate presentation.\ndetails is an object containing:\nalertBody : The message displayed in the notification alert.\nalertAction : The \"action\" displayed beneath an actionable notification. Defaults to \"view\";\nsoundName : The sound played when the notification is fired (optional). The file should be added in the ios project from Xcode, on your target, so that it is bundled in the final app. For more details see the example app.\ncategory : The category of this notification, required for actionable notifications (optional).\nuserInfo : An optional object containing additional notification data.\napplicationIconBadgeNumber (optional) : The number to display as the app's icon badge. The default value of this property is 0, which means that no badge is displayed.")>]
    static member isInlineTag (tagName: string) : bool = nativeOnly

(***)
#r "nuget: Fable.Core"
#r "nuget: Glutinum.Types"
(***)
