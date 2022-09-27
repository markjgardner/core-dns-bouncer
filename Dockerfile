FROM bitnami/kubectl
RUN curl -L https://github.com/a8m/envsubst/releases/download/v1.2.0/envsubst-`uname -s`-`uname -m` -o envsubst && \
    chmod +x envsubst && \
    mv envsubst /usr/local/bin
WORKDIR /tmp/job
ADD ./job.yaml job.yaml
ADD ./operator.sh operator.sh
ENV NODENAME null
ENTRYPOINT [ "/bin/sh", "-c", "./operator.sh" ]