
echo "install bash completion package"
dnf install -y bash-completion

echo "install podman completions"
podman completion -f /etc/bash_completion.d/podman bash

echo "install dotnet completions"
dotnet completions script bash