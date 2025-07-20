namespace Orchestrix.Mediator.Diagnostics;

public sealed class HookConfiguration
{
    /// <summary>
    /// Indicates whether send hooks are enabled or disabled within the hook execution process.
    /// Send hooks are executed during operations related to sending commands or requests.
    /// When set to <c>true</c>, the send hooks will be triggered at the start and completion
    /// of a send operation, as well as in the event of an error. If set to <c>false</c>,
    /// the send hooks are bypassed entirely during the execution process.
    /// </summary>
    public bool EnableSendHooks { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether publish hooks are enabled within the system.
    /// Publish hooks provide an extension point for executing custom logic
    /// before and after publish operations, enabling enhanced diagnostics and functionality
    /// for handling notifications or events.
    /// </summary>
    public bool EnablePublishHooks { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether stream hooks are enabled.
    /// When set to true, hooks related to streaming operations will be triggered.
    /// </summary>
    public bool EnableStreamHooks { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether exceptions thrown by hooks should propagate outward.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, any exceptions thrown within hook implementations will escalate and
    /// interrupt the current operation. When set to <c>false</c>, hook errors are logged or handled
    /// silently, allowing the operation to continue unaffected.
    /// </remarks>
    public bool ThrowOnHookError { get; set; } = false;
  //  public bool EnableDiagnosticsLogging { get; set; } = false; // in future versions
}