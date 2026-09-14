#!/usr/bin/env bash
set -euo pipefail

if command -v dotnet >/dev/null 2>&1; then
    echo ".NET SDK already installed: $(dotnet --version)"
    exit 0
fi

dnf install -y libicu

DOTNET_VERSION=11.0.100-rc.1.26425.128
INSTALL_DIR="$WORKSPACE_ROOT/.install"
mkdir -p "$INSTALL_DIR"

curl -sSL https://dot.net/v1/dotnet-install.sh \
    -o "$INSTALL_DIR/dotnet-install.sh"

chmod +x "$INSTALL_DIR/dotnet-install.sh"

"$INSTALL_DIR/dotnet-install.sh" \
    --version "$DOTNET_VERSION" \
    --install-dir "$DOTNET_ROOT" \
    --no-path

rm -rf "$INSTALL_DIR"

echo "export PATH=\"$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH\"" >> $HOME/.bashrc
echo ".NET SDK installed: $("dotnet" --version)"

