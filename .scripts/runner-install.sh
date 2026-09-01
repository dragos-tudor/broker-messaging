set -euo pipefail

TOKEN=${1:?missing token}

mkdir -p ./.runner && cd ./.runner

echo download the runner package
curl -o actions-runner-linux-x64-2.336.0.tar.gz -L https://github.com/actions/runner/releases/download/v2.336.0/actions-runner-linux-x64-2.336.0.tar.gz

echo extract the installer
tar xzf ./actions-runner-linux-x64-2.336.0.tar.gz

echo remove the installer
rm ./actions-runner-linux-x64-2.336.0.tar.gz

echo change mod for bash scripts
chmod u+x ./*.sh

echo create the runner and start the configuration experience
RUNNER_ALLOW_RUNASROOT=true ./config.sh --url https://github.com/dragos-tudor/backend-kafka --token ${TOKEN}

