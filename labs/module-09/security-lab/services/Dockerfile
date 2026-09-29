# Tiny Python service image for the lab data sources and the egress catcher. Stdlib only, no pip.
# Runs as root inside a throwaway container; the security boundary here is the internal Docker
# network (no internet egress), not the in-container user. These services never call the internet.
FROM python:3.12-alpine
WORKDIR /app
COPY *.py /app/
