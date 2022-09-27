using Microsoft.AspNetCore.Mvc;
using k8s;
using k8s.Models;

namespace corednsOperator.Controllers;

[ApiController]
[Route("[controller]")]
public class OperatorController : ControllerBase
{
    private readonly ILogger<OperatorController> _logger;
    private readonly IKubernetes _kubernetes;

    public OperatorController(ILogger<OperatorController> logger, IKubernetes kubernetes)
    {
        _logger = logger;
        _kubernetes = kubernetes;
    }

    private string GetPodName(string nodeName) 
    {
        var pods = _kubernetes.CoreV1.ListNamespacedPod("kube-system", labelSelector: "k8s-app=kube-dns", fieldSelector: $"spec.nodeName={nodeName}");
        _logger.LogInformation($"Found {pods.Items.Count()} pods");
        return pods.Items.First().Metadata.Name;
    }

    // Post nodeName
    [HttpGet(Name = "DeleteCoreDNSOnNode")]
    public IActionResult CreateJob([FromQuery] string nodeName)
    {
        _logger.LogInformation($"nodeName: {nodeName}");
        var podName = GetPodName(nodeName);
        _logger.LogInformation($"podName: {podName}");

        var ns = new V1Namespace
        {
            Metadata = new V1ObjectMeta
            {
                Name = "kube-system"
            }
        };

        var job = _kubernetes.BatchV1.CreateNamespacedJob(new V1Job
        {
            Metadata = new V1ObjectMeta
            {
                Name = "delete-coredns",
            },
            Spec = new V1JobSpec
            {
                Template = new V1PodTemplateSpec
                {
                    Spec = new V1PodSpec
                    {
                        ServiceAccountName = "corednsdeleter",
                        Containers = new List<V1Container>
                        {
                            new V1Container
                            {
                                Name = "delete-coredns",
                                Image = "bitnami/kubectl:latest",
                                Command = new List<string>
                                {
                                    "sh", 
                                    "-c", 
                                    $"kubectl delete pod -n kube-system {podName}"
                                }
                            }
                        },
                        RestartPolicy = "Never"
                    }
                },
                BackoffLimit = 4
            }
        },
        "kube-system");

        return Ok();
    }
}
