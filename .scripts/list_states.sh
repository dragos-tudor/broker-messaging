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
  grep -E 'internal const string' "$f" \
    | sed -n 's/.*internal const string \([A-Za-z0-9_]*\) = "[^"]*".*/  - \1/p'
  echo
done
