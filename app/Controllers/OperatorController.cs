using Microsoft.AspNetCore.Mvc;
using k8s;
using k8s.Models;
using System.Dynamic;
using Newtonsoft.Json;

namespace kubeproxyOperator.Controllers;

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
        var pods = _kubernetes.CoreV1.ListNamespacedPod("kube-system", labelSelector: "component=kube-proxy", fieldSelector: $"spec.nodeName={nodeName}");
        _logger.LogInformation($"Found {pods.Items.Count()} pods");
        return pods.Items.First().Metadata.Name;
    }

    // Post nodeName
    [HttpPost(Name = "DeletekubeproxyOnNode")]
    public IActionResult Deletekubeproxy(Object alertData)
    {
        dynamic alert = JsonConvert.DeserializeObject(alertData.ToString());
        //assumes the node name is the first returned field in the result set
        string nodeName = alert.alertContext.SearchResults.tables[0].rows[0][0];
        _logger.LogInformation($"nodeName: {nodeName}");
        var podName = GetPodName(nodeName);
        _logger.LogInformation($"podName: {podName}");
        
        var pod = _kubernetes.CoreV1.DeleteNamespacedPodAsync(podName, "kube-system");

        return Ok();
    }
}
