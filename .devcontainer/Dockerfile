FROM quay.io/podman/stable:v5.8.2

ARG DOTNET_SDK_VERSION=dotnet-sdk-10.0

RUN dnf update -y && \
	  dnf install -y ca-certificates git curl nano unzip
RUN dnf install -y $DOTNET_SDK_VERSION
RUN dnf install -y node

ENV SHELL="/bin/bash"

