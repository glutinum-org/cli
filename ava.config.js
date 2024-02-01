export default {
    files: [
        "tests/**/*",
        "!tests/fable_modules/**/*"
    ],
    watchMode: {
        ignoreChanges: [
            "tests/**/*.fsproj",
            "tests/**/*.fs"
        ]
    },
    timeout: "3m"
}
