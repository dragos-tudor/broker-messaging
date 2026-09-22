#!/usr/bin/env bash
set -euo pipefail

if [ "${1:-}" = "" ]; then
  echo "Usage: $0 <folder-path>"
  exit 2
fi

root="$1"

find "$root" -type f -name '*States.cs' | sort | while IFS= read -r f; do
  op=$(basename "$(dirname "$f")")
  echo "Operation: $op ($f)"
  awk '
    /internal enum/ { in_enum=1; next }
    in_enum && /{/ { next }
    in_enum && /}/ { in_enum=0; next }
    in_enum {
      gsub(/^[[:space:]]+|[[:space:],]+$/, "")
      if (length)
        print " - " $0
    }
  ' "$f"
done
