# Agent Monorepo

A monorepo containing Back-End and Front-End services for a sustenance assistant agent system.

## Repository Structure

```
bytewars/
├── Back-End/          # .NET 8.0 API service with decoupled Gateway + Workflow architecture
├── Front-End/         # Frontend service (to be implemented)
└── .github/           # CI/CD workflows
```

## Back-End Service

- **Framework**: .NET 8.0
- **Architecture**: Decoupled Gateway + Workflow pattern
- **Features**:
  - MCP (Multi-Client Protocol) support
  - Workflow orchestration (STT, TTS, RAG, Order Processing, Payment, Auth)
  - JSON-based data storage
  - API key authentication
  - Serilog logging
  - Swagger/OpenAPI documentation
  - Docker support

See `Back-End/README.md` for detailed documentation.

## Front-End Service

Frontend service to be implemented.

## CI/CD

- GitHub Actions workflows for both Back-End and Front-End
- CodeRabbit integration (automatic on PRs)
- SonarCloud integration
- Tests run on push to `main` and `develop` branches

## Development Workflow

1. Work on `develop` branch
2. Create PR: `develop` → `main`
3. Review and merge

## Getting Started

See `Back-End/README.md` for Back-End setup instructions.
