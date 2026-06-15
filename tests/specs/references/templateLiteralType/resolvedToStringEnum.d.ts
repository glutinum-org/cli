type DevToolPosition = 'inline-' | 'hidden-' | 'eval-' | '';
type DevToolDebugIds = '-debugids' | '';
export type DevTool = 'eval' | `${DevToolPosition}source-map${DevToolDebugIds}`;
