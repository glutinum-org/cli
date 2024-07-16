export interface WebviewOptions {
	readonly enableScripts?: boolean;
}

export interface WebviewPanelOptions {
	readonly enableFindWidget?: boolean;
}

declare function createWebviewPanel(options?: WebviewPanelOptions & WebviewOptions);
