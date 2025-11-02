# API Keys Configuration

This document lists all the API keys you need to configure for real integrations.

## Required API Keys

### 1. OpenAI API Key (for STT, TTS, and RAG)

**Where to get it:**
- Sign up at https://platform.openai.com/
- Go to API Keys section
- Create a new secret key

**Configuration location:** `appsettings.json`
```json
{
  "Services": {
    "STT": {
      "ApiKey": "sk-your-openai-api-key-here"
    },
    "TTS": {
      "ApiKey": "sk-your-openai-api-key-here"
    },
    "RAG": {
      "ApiKey": "sk-your-openai-api-key-here"
    }
  }
}
```

**Note:** You can use the same OpenAI API key for all three services (STT, TTS, RAG).

**Services using it:**
- **STT (Speech-to-Text)**: Uses OpenAI Whisper API (`whisper-1` model)
- **TTS (Text-to-Speech)**: Uses OpenAI TTS API (`tts-1` model)
- **RAG (Retrieval-Augmented Generation)**: Uses OpenAI Chat Completions API (`gpt-4` model)

### 2. Stripe API Key (for Payment Processing)

**Where to get it:**
- Sign up at https://stripe.com/
- Go to Developers > API keys
- Get your **Secret key** (starts with `sk_test_` for test mode)

**Configuration location:** `appsettings.json`
```json
{
  "Services": {
    "Payment": {
      "ApiKey": "sk_test_your-stripe-secret-key-here"
    }
  }
}
```

**Note:** Use `sk_test_...` for testing, `sk_live_...` for production.

**Services using it:**
- **Payment**: Stripe Payment Intents API for processing card payments

## Current Behavior

- **Without API keys**: All services return dummy/mock data for testing
- **With API keys**: Services make real API calls to OpenAI/Stripe

## Security Note

⚠️ **NEVER commit API keys to git!** Use:
- Environment variables
- Azure Key Vault / AWS Secrets Manager
- User secrets for local development: `dotnet user-secrets set "Services:STT:ApiKey" "sk-..."`
