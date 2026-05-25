# .NET Microservice CI/CD Workflows

Reusable GitHub Actions workflows for .NET microservices with support for:

- build and test
- Docker image build and push
- OpenAPI artifact generation
- GitHub release orchestration

These workflows are designed to be called from repository-level pipelines.

## Available Workflows

- .github/workflows/microservice-cicd.yml
  Main reusable orchestrator. It wires build, optional Docker delivery, optional OpenAPI generation, and release.
- .github/workflows/build.yml
  Build/test reusable workflow. Produces semantic-version outputs and a published build artifact.
- .github/workflows/docker-delivery.yml
  Optional Docker image build and push with semantic tags.
- .github/workflows/openapi.yml
  Optional OpenAPI artifact generation by executing a provided command.
- .github/workflows/github-delivery.yml
  GitHub Release publication.

## Quick Start

Example caller:

```yaml
jobs:
  ci:
    uses: EasyDesk/workflows/.github/workflows/microservice-cicd.yml@master
    with:
      dotnet-version: 10.x
      main-project: src/MyService/MyService.csproj
      tests: |
        [
          "tests/MyService.UnitTests/MyService.UnitTests.csproj",
          "tests/MyService.IntegrationTests/MyService.IntegrationTests.csproj --filter \"Category=Integration\""
        ]
      dockerfile: Dockerfile
      image-name: my-service
      openapi-command: cat openapi.json
```

## Core Inputs (microservice-cicd.yml)

- dotnet-version (required)
  SDK version accepted by actions/setup-dotnet (for example 10.x).
- main-project (required)
  Path to the main .csproj.
- tests (optional, default [])
  JSON array of test definitions.
- warnings-as-errors (optional, default false)
- artifact-name (optional, default build-artifacts)
- dockerfile, docker-stage, docker-build-args, image-name (optional)
- openapi-command (optional)
  Command that prints OpenAPI JSON to stdout.
- openapi-file (optional, default openapi.json)
- openapi-artifact-name (optional, default openapi)
- submodules (optional, default false)

## Tests Input Format

Use a JSON array of strings. Each string is:

<project_path> [optional_args]

Use [] to skip tests.

Example with spaces in paths:

```yaml
tests: |
  [
    "tests/Project.UnitTests/Project.UnitTests.csproj",
    "\"tests/My Test Project/Project.IntegrationTests.csproj\" --filter \"Category=Integration\""
  ]
```

## Outputs

microservice-cicd.yml exposes:

- version
- released

If your own wrapper workflow calls microservice-cicd.yml, it can forward these outputs through workflow_call.outputs and use them to gate downstream jobs, for example:

```yaml
on:
  workflow_call:
    outputs:
      released:
        value: ${{ jobs.cicd.outputs.released }}
      version:
        value: ${{ jobs.cicd.outputs.version }}

jobs:
  cicd:
    uses: EasyDesk/workflows/.github/workflows/microservice-cicd.yml@master
```

This makes it possible to gate release jobs using needs.<job>.outputs.released.

## Build-time OpenAPI (Optional)

If your application generates OpenAPI at build/publish time, you can still use openapi.yml by providing a command that outputs the generated JSON file.

Example:

```yaml
openapi-command: cat openapi.json
```

Ensure your command writes JSON to stdout because openapi.yml redirects command output to the configured artifact file.

## Notes

- Keep dotnet-version major-train style (for example 10.x) in workflow consumers.
- TargetFramework values remain standard TFMs (for example net10.0).
- OpenAPI generation in openapi.yml expects a command that writes JSON to stdout.
