# kube-proxy-bouncer
An operator that attempts to resolve sporadic DNS failures on windows nodepools caused by an HNS failure.

### Workflow:
* Monitor detects DNS resolution errors for a specific node. 
* An action is triggered which invokes a web endpoint with the node name as payload. 
* The operator queries for the name of the kubeproxy pod running on the affected node.
* The operator then deletes the kubeproxy pod on that node.
* kubeproxy is rescheduled on the node forcing HNS to reapply all network policies and restoring the node.

### Install
```bash
kubectl apply -f 1-rolesAndBindings.yaml -n kube-system
kubectl apply -f 2-deployment.yaml -n kube-system
```
*Note: currently these yaml files will pull the image published to this repo. You will likely want to use your own image.

### Run
Send an HTTP POST request to the service with a [Log Alert](https://learn.microsoft.com/azure/azure-monitor/alerts/alerts-common-schema-definitions#log-alerts) payload containing the affected node name. 
