using System;

using Relationships.Package;

using Skyline.AppInstaller;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.AppPackages;

/// <summary>
/// DataMiner Script Class.
/// </summary>
internal class Script
{
	public const string CatalogIdentifier = "06e517f4-5041-41a1-a186-86cdbf0410b9";
	public const string RegistrationScriptName = "Relationships.Package.Registration";

	/// <summary>
	/// The script entry point.
	/// </summary>
	/// <param name="engine">Provides access to the Automation engine.</param>
	/// <param name="context">Provides access to the installation context.</param>
	[AutomationEntryPoint(AutomationEntryPointType.Types.InstallAppPackage)]
	public void Install(IEngine engine, AppInstallContext context)
	{
		try
		{
			engine.Timeout = new TimeSpan(0, 10, 0);
			engine.GenerateInformation("Starting installation");
			var installer = new AppInstaller(Engine.SLNetRaw, context);
			installer.InstallDefaultContent();

			// Register the Relationships solution in SDM
			Register(engine, context, installer.Log);

			// Remove the subscripts
			engine.GetUserConnection().TryDeleteScript(RegistrationScriptName, installer.Log);
		}
		catch (Exception e)
		{
			engine.ExitFail($"Exception encountered during installation: {e}");
		}
	}

	private void Register(IEngine engine, AppInstallContext context, Action<string> logMethod = null)
	{
		try
		{
			logMethod?.Invoke($"Registering Relationships solution [{CatalogIdentifier}] with version {context.AppInfo.Version} in SDM..");

			var subScript = engine.PrepareSubScript(RegistrationScriptName);
			subScript.SelectScriptParam("version", context.AppInfo.Version);
			subScript.Synchronous = true;
			subScript.ExtendedErrorInfo = true;
			subScript.InheritScriptOutput = true;
			subScript.StartScript();
		}
		catch
		{
			logMethod?.Invoke("Failed to register the solution in SDM.");
		}
	}
}