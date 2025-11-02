import { error } from '@sveltejs/kit';
import type { RequestHandler } from './$types';

const BACKEND_URL = 'http://localhost:8080';

export const POST: RequestHandler = async ({ request }) => {
	try {
		const formData = await request.formData();
		const userId = request.headers.get('x-user-id');
		const sessionId = request.headers.get('x-session-id');

		const backendResponse = await fetch(`${BACKEND_URL}/api/stream/audio`, {
			method: 'POST',
			headers: {
				...(userId && { 'x-user-id': userId }),
				...(sessionId && { 'x-session-id': sessionId })
			},
			body: formData
		});

		if (!backendResponse.ok) {
			const errorText = await backendResponse.text();
			throw error(backendResponse.status, errorText);
		}

		const result = await backendResponse.json();
		return new Response(JSON.stringify(result), {
			status: 200,
			headers: {
				'Content-Type': 'application/json'
			}
		});

	} catch (err) {
		console.error('Proxy error:', err);
		throw error(500, `Backend error: ${err instanceof Error ? err.message : 'Unknown error'}`);
	}
};
