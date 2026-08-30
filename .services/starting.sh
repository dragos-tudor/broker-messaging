set -eo pipefail

echo "checking for stale podman runtime state..."
if ! timeout 10 podman info >/dev/null 2>&1; then
  echo "podman info failed or timed out (likely stale boot ID) — clearing runroot state"
  rm -rf /run/containers/storage /run/libpod
fi

echo "waiting for podman to be ready..."
until podman info >/dev/null 2>&1; do
  sleep 2
done

echo "clearing any stale crun-level state for cluster containers (no recreation)"
for name in coredns kafka-1 kafka-2 kafka-3; do
  cid=$(podman inspect --format '{{.Id}}' "$name" 2>/dev/null || true)
  if [ -n "$cid" ]; then
    crun --root /run/crun delete --force "$cid" >/dev/null 2>&1 || true
  fi
done

echo "starting containers"
podman start coredns kafka-1 kafka-2 kafka-3

$WORKSPACE_ROOT/.services/coredns/hosting.sh
