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

## microservice-cicd.yml Reference

### Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `dotnet-version` | yes | | .NET SDK version accepted by `actions/setup-dotnet` (e.g. `10.x`). |
| `main-project` | yes | | Path to the main `.csproj` file. |
| `tests` | no | `[]` | JSON array of test definitions. See [Tests Input Format](#tests-input-format). |
| `warnings-as-errors` | no | `false` | Treat build warnings as errors. |
| `artifact-name` | no | `build-artifacts` | Name of the shared build artifact passed between jobs. |
| `release-prefix` | no | `${{ github.repository }}` | Prefix used when naming the GitHub release and its zip archive. |
| `submodules` | no | `false` | Checkout Git submodules: `false`, `true`, or `recursive`. |
| `dockerfile` | no | | Path to the Dockerfile. When provided, enables the Docker delivery job. |
| `docker-stage` | no | | Docker build target stage. |
| `docker-build-args` | no | | Additional Docker build arguments. |
| `image-name` | no | `${{ github.repository }}` | Docker image name. |
| `openapi-command` | no | | Command that prints OpenAPI JSON to stdout. When provided, enables OpenAPI generation. |
| `openapi-file` | no | `openapi.json` | Output file name for the OpenAPI document. |
| `openapi-artifact-name` | no | `openapi` | Name of the uploaded OpenAPI artifact. |

### Secrets

| Name | Required | Description |
|------|----------|-------------|
| `registry-url` | no | Docker registry URL. Defaults to a local registry; set to `ghcr.io` for GitHub Container Registry. |
| `docker-username` | no | Registry login username. Defaults to `github.repository_owner`. |
| `docker-password` | no | Registry password or access token. |

### Outputs

| Name | Description |
|------|-------------|
| `version` | Semantic version derived by `EasyDesk/action-semver-checkout`. |
| `released` | `'true'` if a GitHub release was created, `'false'` otherwise. |

If your own wrapper workflow calls microservice-cicd.yml, it can forward these outputs and use them to gate downstream jobs, for example:

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

This makes it possible to gate release jobs using `needs.<job>.outputs.released`.

## build.yml Reference

Build, publish, and optionally test a .NET project. Produces a build artifact consumed by downstream jobs.

### Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `dotnet-version` | yes | | .NET SDK version accepted by `actions/setup-dotnet` (e.g. `10.x`). |
| `project` | yes | | Path to the `.csproj` to build and publish. |
| `build-artifact-name` | yes | | Name for the uploaded build artifact. |
| `warnings-as-errors` | no | `false` | Treat build warnings as errors. |
| `tests` | no | `[]` | JSON array of test definitions. See [Tests Input Format](#tests-input-format). |
| `submodules` | no | `false` | Checkout Git submodules: `false`, `true`, or `recursive`. |

### Outputs

| Name | Description |
|------|-------------|
| `version` | Full semantic version string. |
| `should-release` | `'true'` if the commit should produce a release. |
| `is-prerelease` | `'true'` if this is a pre-release version. |
| `major` | Major version number. |
| `minor` | Minor version number. |
| `patch` | Patch version number. |

## docker-delivery.yml Reference

Build and push a Docker image with semantic version tags.

### Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `dockerfile` | no | `Dockerfile` | Path to the Dockerfile. |
| `image-name` | no | `${{ github.repository }}` | Docker image name. |
| `stage` | no | | Docker build target stage. |
| `build-args` | no | | Additional Docker build arguments. |
| `tag-suffix` | no | `''` | Suffix appended to all image tags. |
| `submodules` | no | `false` | Checkout Git submodules: `false`, `true`, or `recursive`. |

### Secrets

| Name | Required | Description |
|------|----------|-------------|
| `registry-url` | no | Docker registry URL. Defaults to a local registry. |
| `docker-username` | no | Registry login username. Defaults to `github.repository_owner`. |
| `docker-password` | no | Registry password or access token. |

## openapi.yml Reference

Run a command and upload its stdout as an OpenAPI artifact.

### Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `openapi-command` | yes | | Command that prints OpenAPI JSON to stdout. |
| `dotnet-version` | yes | | .NET SDK version used to set up the runtime environment. |
| `build-artifact-name` | yes | | Name of the build artifact to download before running the command. |
| `openapi-artifact-name` | yes | | Name for the uploaded OpenAPI artifact. |
| `output-file` | no | `openapi.json` | File name to write the OpenAPI output to before uploading. |

## github-delivery.yml Reference

Publish a GitHub release with all workflow artifacts zipped.

### Inputs

| Name | Required | Default | Description |
|------|----------|---------|-------------|
| `release-prefix` | no | `${{ github.repository }}` | Prefix for the release name and archive file name. |
| `submodules` | no | `false` | Checkout Git submodules: `false`, `true`, or `recursive`. |

### Outputs

| Name | Description |
|------|-------------|
| `released` | `'true'` if a release was successfully created. |

## Tests Input Format

Use a JSON array of strings. Each string is:

`<project_path> [optional_args]`

Use `[]` to skip tests.

Example with spaces in paths:

```yaml
tests: |
  [
    "tests/Project.UnitTests/Project.UnitTests.csproj",
    "\"tests/My Test Project/Project.IntegrationTests.csproj\" --filter \"Category=Integration\""
  ]
```

## Build-time OpenAPI (Optional)

If your application generates OpenAPI at build/publish time, you can still use openapi.yml by providing a command that outputs the generated JSON file.

Example:

```yaml
openapi-command: cat openapi.json
```

Ensure your command writes JSON to stdout because openapi.yml redirects command output to the configured artifact file.

## Notes

- Keep `dotnet-version` major-train style (for example `10.x`) in workflow consumers.
- `TargetFramework` values remain standard TFMs (for example `net10.0`).
- OpenAPI generation in openapi.yml expects a command that writes JSON to stdout.
