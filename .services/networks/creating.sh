set -eo pipefail

if ! (podman network ls | grep "$DEV_NETWORK" > /dev/null); then
	echo "creating ${DEV_NETWORK}"
	podman network create --driver=bridge --subnet $DEV_NETWORK_SUBNET --disable-dns "$DEV_NETWORK" > /dev/null
fi