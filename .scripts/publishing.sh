set -eu

PACKAGE=${1:?missing package}
GITHUB_TOKEN=${2:?missing GitHub token}
GITHUB_OWNER=${3:?missing GitHub owner}

dotnet nuget push "${PACKAGE}" \
  --source "https://nuget.pkg.github.com/${GITHUB_OWNER}/index.json" \
  --api-key "${GITHUB_TOKEN}" \
  --skip-duplicate