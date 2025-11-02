<script lang="ts">
	import { getEndpoint, USER_CONFIG } from '$lib/config';

	let isRecording = $state(false);
	let status = $state('Ready to order');
	let mediaRecorder: MediaRecorder | null = null;
	let audioStream: MediaStream | null = null;
	let audioChunks: Blob[] = [];

	async function toggleRecording() {
		if (!isRecording) {
			await startRecording();
		} else {
			await stopRecording();
		}
	}

	async function startRecording() {
		try {
			audioStream = await navigator.mediaDevices.getUserMedia({ audio: true });
			audioChunks = [];

			mediaRecorder = new MediaRecorder(audioStream, {
				mimeType: 'audio/webm;codecs=opus'
			});

			mediaRecorder.ondataavailable = (event) => {
				if (event.data.size > 0) {
					audioChunks.push(event.data);
				}
			};

			mediaRecorder.onstop = async () => {
				await sendAudioToBackend();
			};

			mediaRecorder.start(100);
			isRecording = true;
			status = 'Listening... speak your order';
		} catch (error) {
			status = `Error: ${error instanceof Error ? error.message : 'Could not access microphone'}`;
			console.error('Recording error:', error);
		}
	}

	async function stopRecording() {
		if (mediaRecorder && mediaRecorder.state !== 'inactive') {
			mediaRecorder.stop();
		}
		if (audioStream) {
			audioStream.getTracks().forEach(track => track.stop());
		}
		isRecording = false;
		status = 'Processing...';
	}

	async function sendAudioToBackend() {
		try {
			const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
			const formData = new FormData();
			formData.append('File', audioBlob, 'recording.webm');

			const response = await fetch(getEndpoint('streamAudio'), {
				method: 'POST',
				headers: {
					'x-user-id': USER_CONFIG.userId
				},
				body: formData
			});

			if (!response.ok) {
				throw new Error(`Server error: ${response.status} ${response.statusText}`);
			}

			const result = await response.json();
			status = 'Order processed ✓';
			console.log('Backend response:', result);

			setTimeout(() => {
				status = 'Ready to order';
			}, 2000);

		} catch (error) {
			console.error('Upload error:', error);
			status = `Error: ${error instanceof Error ? error.message : 'Failed to process'}`;
			setTimeout(() => {
				status = 'Ready to order';
			}, 3000);
		}
	}
</script>

<div class="container">
	<h1>🍔 Food Order Assistant</h1>

	<div class="status">{status}</div>

	<button
		class="record-button"
		class:recording={isRecording}
		onclick={toggleRecording}
	>
		{isRecording ? '🔴 Stop' : '🎤 Start Order'}
	</button>

	<p class="hint">
		{isRecording
			? 'Speak naturally: "I want a pizza with pepperoni"'
			: 'Click to start voice ordering'}
	</p>
</div>

<style>
	.container {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		min-height: 100vh;
		padding: 2rem;
		gap: 2rem;
	}

	h1 {
		font-size: 2.5rem;
		margin: 0;
	}

	.status {
		font-size: 1.2rem;
		color: #666;
		min-height: 1.5rem;
	}

	.record-button {
		font-size: 1.5rem;
		padding: 2rem 3rem;
		border: 3px solid #333;
		border-radius: 1rem;
		background: white;
		cursor: pointer;
		transition: all 0.2s;
		font-weight: bold;
	}

	.record-button:hover {
		transform: scale(1.05);
		box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
	}

	.record-button.recording {
		background: #ff4444;
		color: white;
		border-color: #cc0000;
		animation: pulse 1.5s ease-in-out infinite;
	}

	@keyframes pulse {
		0%, 100% { opacity: 1; }
		50% { opacity: 0.7; }
	}

	.hint {
		color: #999;
		font-size: 0.9rem;
		text-align: center;
		max-width: 400px;
	}
</style>
