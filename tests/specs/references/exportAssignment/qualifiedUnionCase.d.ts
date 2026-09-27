declare namespace ts {
    namespace server {
        namespace protocol {
            type ScriptKindName = "TS" | "JS";
        }
        function tryConvertScriptKindName(scriptKindName: protocol.ScriptKindName | ScriptKind): ScriptKind;
        function convertScriptKindName(scriptKindName: protocol.ScriptKindName): ScriptKind;
    }
    enum ScriptKind {
        JS = 1,
        TS = 3
    }
}
export = ts;
