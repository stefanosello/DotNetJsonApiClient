namespace JsonApiClient;

/// <summary>
/// Configuration options for the JsonApiClient.
/// </summary>
public class JsonApiClientOptions
{
    /// <summary>
    /// Maximum number of retry attempts for failed requests.
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Delay between retry attempts.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    
    /// <summary>
    /// Maximum timeout for HTTP requests.
    /// </summary>
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Whether to enable automatic retry for transient failures.
    /// </summary>
    public bool EnableRetry { get; set; } = true;
    
    /// <summary>
    /// HTTP status codes that should trigger a retry.
    /// </summary>
    public int[] RetryableStatusCodes { get; set; } = [408, 429, 500, 502, 503, 504];
} 