export type Content =
  | { kind: "html"; html: string }
  | { type: "markdown"; markdown: string };
