#!/bin/bash
# query kube-apiserver for the core-dns pod name on the provided node
PODNAME = $(kubectl get po -n kube-system -l k8s-app=kube-dns --field-selector spec.nodeName=$NODENAME -o jsonpath='{.items[0].metadata.name}')
# create the job to delete the coredns pod
envsubst < job.yaml | kubectl apply -f --overwrite=true -
# return success
exit 0