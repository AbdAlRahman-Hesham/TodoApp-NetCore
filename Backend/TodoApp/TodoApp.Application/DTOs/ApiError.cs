using System.Text.Json.Serialization;

namespace TodoApp.Application.Errors;

public class ApiError
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Details { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string[]>? Errors { get; set; }

    public ApiError(int statusCode, string message, string? details = null)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }

    public ApiError(int statusCode, string message, Dictionary<string, string[]> errors)
    {
        StatusCode = statusCode;
        Message = message;
        Errors = errors;
    } 
    public ApiError(int statusCode)
    {
        StatusCode = statusCode;
        Message = GetDefaultMessageForStatusCode(statusCode);
    }
    private string GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            100 => "Continue - The client should continue with its request.",
            101 => "Switching Protocols - The server is switching protocols as requested.",
            102 => "Processing - The server has received and is processing the request.",
            103 => "Early Hints - The server is likely to send a final response with the header hints included.",

            200 => "OK - The request has succeeded.",
            201 => "Created - The request has been fulfilled, resulting in the creation of a new resource.",
            202 => "Accepted - The request has been accepted for processing, but the processing has not been completed.",
            203 => "Non-Authoritative Information - The request was successful, but the returned information may be modified by a transforming proxy.",
            204 => "No Content - The server successfully processed the request but is not returning any content.",
            205 => "Reset Content - The server successfully processed the request, but requires the client to reset the document view.",
            206 => "Partial Content - The server is delivering only part of the resource due to a range request.",
            207 => "Multi-Status - A response for multiple independent operations.",
            208 => "Already Reported - The members of a DAV binding have already been enumerated.",
            226 => "IM Used - The server has fulfilled a request for the resource, and the response is a representation of the result of one or more instance-manipulations applied to the current instance.",

            300 => "Multiple Choices - The request has multiple possible responses.",
            301 => "Moved Permanently - The requested resource has been permanently moved to a new URL.",
            302 => "Found - The requested resource has been temporarily moved to a different URL.",
            303 => "See Other - The response to the request can be found under a different URL.",
            304 => "Not Modified - The requested resource has not been modified since the last request.",
            305 => "Use Proxy - The requested resource must be accessed through a proxy.",
            306 => "Switch Proxy - This code was used in previous versions and is no longer used.",
            307 => "Temporary Redirect - The requested resource is temporarily located at a different URL.",
            308 => "Permanent Redirect - The requested resource is permanently located at a different URL.",

            400 => "Bad Request - The server could not understand the request due to invalid syntax.",
            401 => "Unauthorized - The client must authenticate itself to get the requested response.",
            402 => "Payment Required - Reserved for future use.",
            403 => "Forbidden - The client does not have access rights to the content.",
            404 => "Not Found - The server can’t find the requested resource.",
            405 => "Method Not Allowed - The request method is not supported for the resource.",
            406 => "Not Acceptable - The server cannot produce a response matching the list of acceptable values.",
            407 => "Proxy Authentication Required - The client must authenticate with a proxy server first.",
            408 => "Request Timeout - The server timed out waiting for the request.",
            409 => "Conflict - The request conflicts with the current state of the resource.",
            410 => "Gone - The requested resource is no longer available.",
            411 => "Length Required - The server requires a valid Content-Length header.",
            412 => "Precondition Failed - One or more conditions given in the request headers evaluated to false.",
            413 => "Payload Too Large - The request entity is too large for the server to process.",
            414 => "URI Too Long - The requested URL is too long for the server to handle.",
            415 => "Unsupported Media Type - The request media format is not supported by the server.",
            416 => "Range Not Satisfiable - The requested range is not available.",
            417 => "Expectation Failed - The server cannot meet the expectation given in the Expect request header.",
            418 => "I'm a teapot - A joke response from an April Fools' RFC.",
            421 => "Misdirected Request - The request was directed at a server that is not able to produce a response.",
            422 => "Unprocessable Entity - The request was well-formed but unable to be followed due to semantic errors.",
            423 => "Locked - The resource being accessed is locked.",
            424 => "Failed Dependency - The request failed due to a failure of a previous request.",
            425 => "Too Early - The server is unwilling to process the request because it might be replayed.",
            426 => "Upgrade Required - The client should switch to a different protocol.",
            428 => "Precondition Required - The server requires the request to be conditional.",
            429 => "Too Many Requests - The user has sent too many requests in a given time frame.",
            431 => "Request Header Fields Too Large - The server will not process the request because its header fields are too large.",
            451 => "Unavailable For Legal Reasons - The server is refusing to serve the request due to legal reasons.",

            500 => "Internal Server Error - The server encountered an unexpected condition.",
            501 => "Not Implemented - The server does not support the request method.",
            502 => "Bad Gateway - The server received an invalid response from the upstream server.",
            503 => "Service Unavailable - The server is not ready to handle the request.",
            504 => "Gateway Timeout - The upstream server did not respond in time.",
            505 => "HTTP Version Not Supported - The server does not support the HTTP version used in the request.",
            506 => "Variant Also Negotiates - The server has an internal configuration error.",
            507 => "Insufficient Storage - The server is unable to store the representation needed to complete the request.",
            508 => "Loop Detected - The server detected an infinite loop in request processing.",
            510 => "Not Extended - Further extensions to the request are required for the server to fulfill it.",
            511 => "Network Authentication Required - The client needs to authenticate to gain network access.",

            _ => "Unknown Status Code - No message available for this status."
        };
    }
}