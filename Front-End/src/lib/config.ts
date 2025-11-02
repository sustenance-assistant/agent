export const API_CONFIG = {
	baseUrl: '',

	endpoints: {
		streamAudio: '/api/stream/audio',
		streamText: '/api/stream/text',
		register: '/api/auth/register',
		login: '/api/auth/login',
		createApiKey: '/api/auth/api-key',
		addCard: '/api/payment/cards',
		getCard: '/api/payment/cards',
		bill: '/api/payment/bill',
		mcpListTools: '/api/mcp/tools/list',
		mcpCallTool: '/api/mcp/tools/call',
		health: '/api/health',
		context: '/api/context'
	}
};

export function getEndpoint(name: keyof typeof API_CONFIG.endpoints): string {
	return `${API_CONFIG.baseUrl}${API_CONFIG.endpoints[name]}`;
}

export const USER_CONFIG = {
	userId: 'user-123',
	sessionId: `session-${Date.now()}`
};
