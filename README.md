# core-dns-bouncer
An operator that attempts to resolve sporadic DNS failures on windows nodepools caused by an HNS failure.

### Workflow:
* Monitor detects DNS resolution errors for a specific node. 
* An action is triggered which invokes a webhook with the node name as payload. 
* KEDA scales up the operator.
* The operator queries for the name of the CoreDNS pod running on the affected node.
* The operator then deletes the CoreDNS pod on that node.
* CoreDNS is rescheduled on the node forcing HNS to reapply all network policies and restoring the node.

### Install
```bash
kubectl apply -f 1-rolesAndBindings.yaml -n kube-system
kubectl apply -f 2-deployment.yaml -n kube-system
```
*Note: currently these yaml files will pull the image published to this repo. You will likely want to use your own image.

### Run
Send an HTTP GET request to the service with the ```nodeName``` query parameter. 
