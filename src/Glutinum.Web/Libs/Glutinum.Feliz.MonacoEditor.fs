namespace rec Glutinum.Feliz.MonacoEditor

open Feliz
open Fable.Core
open Fable.Core.JsInterop

module Types =

    type IEditorProps = interface end

module Interop =

    let mkEditorProps (name: string) (value: obj) = unbox<Types.IEditorProps> (name, value)

[<Erase>]
type editor =

    /// Default value of the current model
    static member inline defaultValue(value: string) =
        Interop.mkEditorProps "defaultValue" value

    /// Default language of the current model
    static member inline defaultLanguage(value: string) =
        Interop.mkEditorProps "defaultLanguage" value

    /// <summary>
    /// Default path of the current model
    /// Will be passed as the third argument to <c>.createModel</c> method
    /// <c>monaco.editor.createModel(..., ..., monaco.Uri.parse(defaultPath))</c>
    /// </summary>
    static member inline defaultPath(value: string) =
        Interop.mkEditorProps "defaultPath" value

    /// Value of the current model
    static member inline value(value: string) = Interop.mkEditorProps "value" value

    /// Language of the current model
    static member inline language(value: string) = Interop.mkEditorProps "language" value

    /// <summary>
    /// Path of the current model
    /// Will be passed as the third argument to <c>.createModel</c> method
    /// <c>monaco.editor.createModel(..., ..., monaco.Uri.parse(defaultPath))</c>
    /// </summary>
    static member inline path(value: string) = Interop.mkEditorProps "path" value

    /// <summary>
    /// The theme for the monaco
    /// Available options "vs-dark" | "light"
    /// Define new themes by <c>monaco.editor.defineTheme</c>
    /// </summary>
    /// <default>"light"</default>
    static member inline theme(value: string) = Interop.mkEditorProps "theme" value

    /// <summary>
    /// The theme for the monaco
    /// Available options "vs-dark" | "light"
    /// Define new themes by <c>monaco.editor.defineTheme</c>
    /// </summary>
    /// <default>"light"</default>
    static member inline theme(value: Theme) = Interop.mkEditorProps "theme" value

    /// The line to jump on it
    static member inline line(value: float) = Interop.mkEditorProps "line" value

    /// <summary>The loading screen before the editor will be mounted</summary>
    /// <default>"Loading..."</default>
    static member inline loading(value: ReactElement) = Interop.mkEditorProps "loading" value

    /// IStandaloneEditorConstructionOptions
    static member inline options(value: Editor.IStandaloneEditorConstructionOptions) =
        Interop.mkEditorProps "options" value

    /// IEditorOverrideServices
    static member inline overrideServices(value: Editor.IEditorOverrideServices) =
        Interop.mkEditorProps "overrideServices" value

    /// Indicator whether to save the models' view states between model changes or not
    /// Defaults to true
    static member inline saveViewState(value: bool) =
        Interop.mkEditorProps "saveViewState" value

    /// <summary>Indicator whether to dispose the current model when the Editor is unmounted or not</summary>
    /// <default>false</default>
    static member inline keepCurrentModel(value: bool) =
        Interop.mkEditorProps "keepCurrentModel" value

    /// <summary>Width of the editor wrapper</summary>
    /// <default>"100%"</default>
    static member inline width(value: string) = Interop.mkEditorProps "width" value

    /// <summary>Width of the editor wrapper</summary>
    /// <default>"100%"</default>
    static member inline width(value: float) = Interop.mkEditorProps "width" value

    /// <summary>Height of the editor wrapper</summary>
    /// <default>"100%"</default>
    static member inline height(value: string) = Interop.mkEditorProps "height" value

    /// <summary>Height of the editor wrapper</summary>
    /// <default>"100%"</default>
    static member inline height(value: float) = Interop.mkEditorProps "height" value

    /// Class name for the editor container
    static member inline className(value: string) = Interop.mkEditorProps "className" value

    /// Props applied to the wrapper element
    static member inline wrapperProps(value: obj) =
        Interop.mkEditorProps "wrapperProps" value

    /// Signature: function(monaco: Monaco) => void
    /// An event is emitted before the editor is mounted
    /// It gets the monaco instance as a first argument
    /// Defaults to "noop"
    static member inline beforeMount(value: BeforeMount) =
        Interop.mkEditorProps "beforeMount" (System.Func<_, _> value)

    /// Signature: function(editor: monaco.editor.IStandaloneCodeEditor, monaco: Monaco) => void
    /// An event is emitted when the editor is mounted
    /// It gets the editor instance as a first argument and the monaco instance as a second
    /// Defaults to "noop"
    static member inline onMount(value: OnMount) =
        Interop.mkEditorProps "onMount" (System.Func<_, _, _> value)

    /// Signature: function(value: string | undefined, ev: monaco.editor.IModelContentChangedEvent) => void
    /// An event is emitted when the content of the current model is changed
    static member inline onChange(value: OnChange) =
        Interop.mkEditorProps "onChange" (System.Func<_, _, _> value)

    /// Signature: function(markers: monaco.editor.IMarker[]) => void
    /// An event is emitted when the content of the current model is changed
    /// and the current model markers are ready
    /// Defaults to "noop"
    static member inline onValidate(value: OnValidate) =
        Interop.mkEditorProps "onValidate" (System.Func<_, _> value)

[<RequireQualifiedAccess>]
[<StringEnum(CaseRules.None)>]
type Theme =
    | ``vs-dark``
    | light

type OnMount = Editor.IStandaloneCodeEditor -> Monaco -> unit

type BeforeMount = Monaco -> unit

type OnChange = string option -> Editor.IModelContentChangedEvent -> unit

type OnValidate = ResizeArray<Editor.IMarker> -> unit

type Monaco = obj

module Editor =

    type IStandaloneEditorConstructionOptions = obj
    type IEditorOverrideServices = interface end
    type IModelContentChangedEvent = interface end
    type IMarker = interface end
    type IStandaloneCodeEditor = interface end

[<Erase>]
type Exports =

    static member inline Editor(properties: Types.IEditorProps seq) =
        Interop.reactApi.createElement (
            importDefault "@monaco-editor/react",
            createObj !!properties
        )

    [<Import("useMonaco", "@monaco-editor/react")>]
    static member inline useMonaco() : Monaco option = nativeOnly
