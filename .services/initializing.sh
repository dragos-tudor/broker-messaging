set -e

MESSAGING_NETWORK="messaging-network"
if ! (podman network ls | grep $MESSAGING_NETWORK > /dev/null); then
	echo "create ${MESSAGING_NETWORK}"
	podman network create --driver=bridge --disable-dns $MESSAGING_NETWORK
fi
