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
    public IActionResult DeleteCoreDNS([FromQuery] string nodeName)
    {
        _logger.LogInformation($"nodeName: {nodeName}");
        var podName = GetPodName(nodeName);
        _logger.LogInformation($"podName: {podName}");
        
        var pod = _kubernetes.CoreV1.DeleteNamespacedPodAsync(podName, "kube-system");

        return Ok();
    }
}
