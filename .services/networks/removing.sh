set -e

echo "removing ${DEV_NETWORK}"
podman network rm "${DEV_NETWORK}" 2>/dev/null || true