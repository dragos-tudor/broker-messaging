#!/bin/bash
set -euo pipefail

HOSTS_FILE="$WORKSPACE_ROOT/.services/coredns/dynamic-hosts"

echo "# dinamically generated kafka containers hosts $(date)" > "${HOSTS_FILE}"
podman ps -a --format '{{.Names}}' | while read -r name; do
  [ "${name}" = "coredns" ] && continue

  ip=$(podman exec "${name}" hostname -i)
  [ -n "${ip}" ] && echo "${ip} ${name}" >> "${HOSTS_FILE}"
done

