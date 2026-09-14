FROM quay.io/podman/stable:v5.8.2

RUN dnf update -y && \
	  dnf install -y ca-certificates git curl nano unzip
RUN dnf install -y node

ENV SHELL="/bin/bash"

