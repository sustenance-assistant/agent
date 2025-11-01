# Back-End Service

Decoupled Gateway + Workflow architecture for food ordering with MCP (Multi-Client Protocol) support.

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Docker (optional, for containerized deployment)

### Development

```bash
# Navigate to Back-End directory
cd Back-End

# Restore packages
dotnet restore

# Build the solution
dotnet build

# Run the API
dotnet run --project src/BackEndService.API/BackEndService.API.csproj --urls http://localhost:5050

# Or run directly
cd src/BackEndService.API
dotnet run --urls http://localhost:5050
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/BackEndService.UnitTests
dotnet test tests/BackEndService.IntegrationTests
dotnet test tests/BackEndService.E2ETests

# Run with verbose output
dotnet test --verbosity normal

# Run with code coverage (requires coverlet)
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Building for Production

```bash
# Build Release configuration
dotnet build --configuration Release

# Publish API for deployment
dotnet publish src/BackEndService.API/BackEndService.API.csproj \
  --configuration Release \
  --output ./publish

# Build Docker image
cd docker
docker build -t backend-service:latest .
docker-compose up
```

## Project Structure

```
Back-End/
├── src/
│   ├── BackEndService.API/          # Web API layer
│   ├── BackEndService.Core/          # Core interfaces and models
│   ├── BackEndService.Gateway/       # Gateway services
│   ├── BackEndService.Workflows/     # Workflow modules
│   └── BackEndService.Infrastructure/# Infrastructure implementations
├── tests/
│   ├── BackEndService.UnitTests/     # Unit tests
│   ├── BackEndService.IntegrationTests/# Integration tests
│   └── BackEndService.E2ETests/      # End-to-end tests
├── docker/                           # Docker configuration
├── docs/                             # Documentation and Postman collections
└── BackEndService.sln               # Solution file
```

## Configuration

API keys are configured in `src/BackEndService.API/appsettings.json`:

```json
{
  "Services": {
    "STT": { "ApiKey": "sk-proj-..." },
    "TTS": { "ApiKey": "sk-proj-..." },
    "RAG": { "ApiKey": "sk-proj-..." },
    "Payment": { "ApiKey": "sk_test_..." }
  }
}
```

See `API_KEYS.md` for details on obtaining API keys.

## API Endpoints

- Swagger UI: http://localhost:5050/swagger
- Health: `GET /api/health`
- Auth: `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/api-key`
- Payment: `POST /api/payment/cards`, `POST /api/payment/bill`
- STT: `POST /api/stt/transcribe`
- TTS: `POST /api/tts/synthesize`
- RAG: `POST /api/rag/search`
- MCP: `POST /api/mcp/tools/list`, `POST /api/mcp/tools/call`

## Examples

### Run with API keys (real integrations)
```bash
# Set OpenAI API key
dotnet user-secrets set "Services:STT:ApiKey" "sk-proj-..." --project src/BackEndService.API
dotnet user-secrets set "Services:TTS:ApiKey" "sk-proj-..." --project src/BackEndService.API
dotnet user-secrets set "Services:RAG:ApiKey" "sk-proj-..." --project src/BackEndService.API

# Set Stripe key
dotnet user-secrets set "Services:Payment:ApiKey" "sk_test_..." --project src/BackEndService.API

# Run
dotnet run --project src/BackEndService.API
```

### Test specific workflow
```bash
# Test STT with dummy data
curl -X POST http://localhost:5050/api/stt/transcribe/dummy \
  -H "x-user-id: test-user" \
  -H "x-session-id: test-session"

# Test RAG search
curl -X POST http://localhost:5050/api/rag/search/dummy \
  -H "Content-Type: application/json" \
  -H "x-user-id: test-user" \
  -d '{"text": "What pizza do you offer?"}'
```

## Development Workflow

1. **Make changes** to source code
2. **Run tests** to verify: `dotnet test`
3. **Build** to check for errors: `dotnet build`
4. **Run locally** for testing: `dotnet run --project src/BackEndService.API`
5. **Test with Postman** using collection in `docs/postman/`

## Docker

```bash
# Build and run with Docker Compose
cd docker
docker-compose up --build

# Or build manually
docker build -f docker/Dockerfile -t backend-service .
docker run -p 5050:5050 backend-service
```

## Troubleshooting

- **Build errors**: Run `dotnet clean` then `dotnet restore`
- **Port in use**: Change port in `--urls` parameter or `docker-compose.yml`
- **Tests fail**: Ensure all workflows are registered (startup self-check will fail if not)
- **API keys not working**: Check `appsettings.json` or user secrets configuration

