# core-dns-bouncer
An operator that attempts to resolve sporadic DNS failures on windows nodepools caused by an HNS failure.

Workflow:
* Monitor detects DNS resolution errors for a specific node. 
* An action is triggered which invokes a webhook with the node name as payload. 
* KEDA scales up the operator.
* The operator queries for the name of the CoreDNS pod running on the affected node.
* The operator creates a "DeleteCoreDNS" job specifying the pod name.
* A job is created which deletes the CoreDNS pod on that node.
* CoreDNS is rescheduled on the node forcing HNS to reapply all network policies and restoring the node.